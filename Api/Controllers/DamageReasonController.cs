using Api.Attributes;
using Api.Constants;
using Api.Controllers.Abstract;
using DataAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{
    public class DamageReasonController : BaseController<DamageReason>
    {
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<DamageReason> Get()
        {
            return Context.Set<DamageReason>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<DamageReason> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<DamageReason>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(DamageReason entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            entity.Id = Guid.NewGuid();

            if (entity.RelationsAsParent != null && entity.RelationsAsParent.Count > 0)
            {
                foreach (var relation in entity.RelationsAsParent)
                {
                    relation.Id = Guid.NewGuid();
                    relation.Child = null;
                    relation.Parent = null;
                    relation.ParentDamageReasonId = entity.Id;

                    Context.DamageReasonRelations.Add(relation);

                    if (relation.Child != null)
                    {
                        Context.Entry(relation.Child).State = EntityState.Detached;
                    }
                }
            }

            Context.DamageReasons.Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, DamageReason entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            #region Relations as parent

            var oldRelations = await Context.DamageReasonRelations.Where(r => r.ParentDamageReasonId == key).AsNoTracking().ToListAsync();
            var newRelations = entity.RelationsAsParent.ToList();

            foreach (var relation in newRelations)
            {
                relation.Child = null;
                relation.Parent = null;
            }

            // Add
            foreach (var relation in newRelations.Where(r => r.Id == Guid.Empty))
            {
                relation.Id = Guid.NewGuid();
                relation.ParentDamageReasonId = key;

                Context.DamageReasonRelations.Add(relation);

                if (relation.Child != null)
                {
                    Context.Entry(relation.Child).State = EntityState.Detached;
                }
            }

            // Update
            foreach (var relation in newRelations.Where(n => oldRelations.Any(o => o.Id == n.Id)))
            {
                if (!relation.Equals(oldRelations.Single(r => r.Id == relation.Id)))
                {
                    Context.Entry(relation).State = EntityState.Modified;
                }
            }

            #endregion

            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();

            return Updated(entity);
        }
    }
}