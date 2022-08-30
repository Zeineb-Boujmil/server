using Api.Attributes;
using Api.Constants;
using Api.Controllers.Abstract;
using Api.ViewModels;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{
    public class InsuranceCoverageController : BaseController<InsuranceCoverage>
    {
        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<InsuranceCoverage> Get()
        {
            return Context.Set<InsuranceCoverage>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<InsuranceCoverage> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<InsuranceCoverage>().Where(e => e.Id == key));
        }

        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(InsuranceCoverage entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            entity.Id = Guid.NewGuid();

            foreach (var insuranceCoverageHierarchy in entity.HierarchiesAsParent)
            {
                insuranceCoverageHierarchy.ParentId = entity.Id;
                insuranceCoverageHierarchy.EndDate = entity.Inactive ? DateTime.Now : DateTime.MaxValue;
                insuranceCoverageHierarchy.Child = null;
                insuranceCoverageHierarchy.Parent = null;
                Context.InsuranceCoverageHierarchies.Add(insuranceCoverageHierarchy);
            }

            return await base.Post(entity);
        }

        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, InsuranceCoverage entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var insuranceCoverageHierarchy in entity.HierarchiesAsParent.Where(x => x.ParentId == Guid.Empty).ToList())
            {
                insuranceCoverageHierarchy.ParentId = entity.Id;
                insuranceCoverageHierarchy.EndDate = entity.Inactive ? DateTime.Now : DateTime.MaxValue;
                insuranceCoverageHierarchy.Child = null;
                insuranceCoverageHierarchy.Parent = null;
                Context.Entry(insuranceCoverageHierarchy).State = EntityState.Added;
            }

            var existingEntites = Context.InsuranceCoverageHierarchies.AsNoTracking()
                .Where(x => x.ParentId == entity.Id).ToList();

            foreach (var insuranceCoverageHierarchy in existingEntites)
            {
                if (entity.HierarchiesAsParent.All(x => x.Id != insuranceCoverageHierarchy.Id))
                    Context.Entry(insuranceCoverageHierarchy).State = EntityState.Deleted;
            }

            return await base.Put(key, entity);
        }

        [HttpGet]
        public async Task<IEnumerable<DropDownViewModel>> FillHierarchyDropdown(Guid? coverageId, string filter)
        {
            async Task<List<Guid>> GetParentIds(List<Guid> list, Guid id)
            {
                foreach (var parentId in await Context.InsuranceCoverageHierarchies.Where(h => h.Id == id).Select(h => h.ParentId).ToListAsync())
                {
                    list.Add(parentId);
                    list.AddRange(await GetParentIds(list, parentId));
                }

                return list;
            }

            var childrenIds = Context.InsuranceCoverageHierarchies.Select(x => x.Id).ToList();

            var query = Context.InsuranceCoverages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(o => o.Code.Contains(filter) || o.LongName.Contains(filter));
            }

            if (coverageId != null)
            {
                var id = (Guid)coverageId;

                query = query.Where(o => o.Id != id);

                foreach (var parentId in await GetParentIds(new List<Guid>(), id))
                {
                    childrenIds.Add(parentId);
                }
            }

            childrenIds = childrenIds.Distinct().ToList();
            var result = await query.Where(x => !childrenIds.Contains(x.Id)).OrderBy(x => x.LongName).Take(25).ToListAsync();

            return result.Select(obj => new DropDownViewModel() { Value = obj.Id, Text = $"{obj.Code} - {obj.LongName}" });
        }
    }
}