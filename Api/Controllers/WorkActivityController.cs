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
    public class WorkActivityController : BaseController<WorkActivity>
    {
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<WorkActivity> Get()
        {
            return Context.Set<WorkActivity>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<WorkActivity> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<WorkActivity>().Where(e => e.Id == key));
        }

        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(WorkActivity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            entity.Id = Guid.NewGuid();

            if (entity.WorkActivityOptions != null)
            {
                foreach (var option in entity.WorkActivityOptions)
                {
                    option.WorkActivityId = entity.Id;
                    Context.WorkActivityOptions.Add(option);

                    if (option.ActivityOption != null)
                    {
                        Context.Entry(option.ActivityOption).State = EntityState.Detached;
                    }
                }
            }

            if (entity.RelationsAsParent != null)
            {
                foreach (var relation in entity.RelationsAsParent)
                {
                    relation.Id = Guid.NewGuid();
                    relation.Child = null;
                    relation.Parent = null;
                    relation.ParentWorkActivityId = entity.Id;

                    Context.WorkActivityRelations.Add(relation);

                    if (relation.Child != null)
                    {
                        Context.Entry(relation.Child).State = EntityState.Detached;
                    }
                }
            }

            Context.WorkActivities.Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put(Guid key, WorkActivity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            #region Options

            var oldOptions = await Context.WorkActivityOptions.AsNoTracking().Where(e => e.WorkActivityId == key).ToListAsync();
            var newOptions = entity.WorkActivityOptions;

            foreach (var option in newOptions.Where(n => n.Id == Guid.Empty).ToList())
            {
                option.WorkActivityId = key;
                Context.WorkActivityOptions.Add(option);

                if (option.ActivityOption != null)
                {
                    Context.Entry(option.ActivityOption).State = EntityState.Detached;
                }
            }

            foreach (var option in oldOptions.Where(o => newOptions.All(n => n.Id != o.Id)).ToList())
            {
                Context.WorkActivityOptions.Remove(option);
            }

            #endregion

            #region Relations as parent

            var oldRelations = await Context.WorkActivityRelations.AsNoTracking().Where(r => r.ParentWorkActivityId == key).ToListAsync();
            var newRelations = entity.RelationsAsParent;

            foreach (var relation in newRelations)
            {
                relation.Child = null;
                relation.Parent = null;
            }

            foreach (var newRelation in newRelations.Where(r => r.Id == Guid.Empty))
            {
                newRelation.Id = Guid.NewGuid();
                newRelation.ParentWorkActivityId = key;

                Context.WorkActivityRelations.Add(newRelation);
            }

            foreach (var relation in newRelations.Where(nr => oldRelations.Any(or => or.Id == nr.Id)).ToList())
            {
                Context.Entry(relation).State = EntityState.Modified;
            }

            #endregion

            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();

            return Updated(entity);
        }
    }
}