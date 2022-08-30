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
    public class InsuranceObjectController : BaseController<InsuranceObject>
    {
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<InsuranceObject> Get()
        {
            return Context.Set<InsuranceObject>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<InsuranceObject> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<InsuranceObject>().Where(e => e.Id == key));
        }

        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(InsuranceObject entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            entity.Id = Guid.NewGuid();

            foreach (var insuranceObjectHierarchy in entity.HierarchiesAsParent)
            {
                insuranceObjectHierarchy.ParentInsuranceObjectId = entity.Id;
                insuranceObjectHierarchy.EndDate = entity.Inactive ? DateTime.Now : DateTime.MaxValue;
                insuranceObjectHierarchy.Child = null;
                insuranceObjectHierarchy.Parent = null;
                Context.InsuranceObjectHierarchies.Add(insuranceObjectHierarchy);
            }

            if (entity.InsuranceObjectDamageLocations != null)
            {
                foreach (var location in entity.InsuranceObjectDamageLocations)
                {
                    location.ObjectId = entity.Id;

                    Context.InsuranceObjectDamageLocations.Add(location);
                    Context.Entry(location.DamageLocation).State = EntityState.Detached;
                }
            }

            return await base.Post(entity);
        }

        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, InsuranceObject entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var insuranceObjectHierarchy in entity.HierarchiesAsParent.Where(x => x.ParentInsuranceObjectId == Guid.Empty).ToList())
            {
                insuranceObjectHierarchy.ParentInsuranceObjectId = entity.Id;
                insuranceObjectHierarchy.EndDate = entity.Inactive ? DateTime.Now : DateTime.MaxValue;
                insuranceObjectHierarchy.Child = null;
                insuranceObjectHierarchy.Parent = null;
                Context.Entry(insuranceObjectHierarchy).State = EntityState.Added;
            }

            var existingEntites = Context.InsuranceObjectHierarchies.AsNoTracking()
                .Where(x => x.ParentInsuranceObjectId == entity.Id).ToList();

            foreach (var insuranceObjectHierarchy in existingEntites)
            {
                if (entity.HierarchiesAsParent.All(x => x.Id != insuranceObjectHierarchy.Id))
                    Context.Entry(insuranceObjectHierarchy).State = EntityState.Deleted;
            }

            CRUD(entity.InsuranceObjectDamageLocations,
                Context.InsuranceObjectDamageLocations.AsNoTracking().Where(e => e.ObjectId == entity.Id).ToList(),
                insuranceObjectDamageLocation =>
                {
                    insuranceObjectDamageLocation.ObjectId = entity.Id;
                    Context.Entry(insuranceObjectDamageLocation).State = EntityState.Added;
                    Context.Entry(insuranceObjectDamageLocation.DamageLocation).State = EntityState.Detached;
                },
                insuranceObjectDamageLocation => { Context.Entry(insuranceObjectDamageLocation).State = EntityState.Modified; },
                insuranceObjectDamageLocation => { Context.Entry(insuranceObjectDamageLocation).State = EntityState.Deleted; });

            return await base.Put(key, entity);
        }

        [HttpGet]
        public async Task<IEnumerable<DropDownViewModel>> FillHierarchyDropdown(Guid? objectId, string filter)
        {
            async Task<List<Guid>> GetParentIds(List<Guid> list, Guid id)
            {
                foreach (var parentId in await Context.InsuranceObjectHierarchies.Where(h => h.Id == id).Select(h => h.ParentInsuranceObjectId).ToListAsync())
                {
                    list.Add(parentId);
                    list.AddRange(await GetParentIds(list, parentId));
                }

                return list;
            }

            var childrenIds = Context.InsuranceObjectHierarchies.Select(x => x.Id).ToList();

            var query = Context.InsuranceObjects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(o => o.Code.Contains(filter) || o.LongName.Contains(filter));
            }

            if (objectId != null)
            {
                var id = (Guid)objectId;

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