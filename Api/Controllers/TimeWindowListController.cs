using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Api.Attributes;
using Api.Constants;
using Api.Controllers.Abstract;
using DataAccess;

namespace Api.Controllers
{
    public class TimeWindowListController : BaseController<TimeWindowList>
    {
        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<TimeWindowList> Get()
        {
            return Context.Set<TimeWindowList>();
        }

        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<TimeWindowList> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<TimeWindowList>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override Task<IHttpActionResult> Post(TimeWindowList entity)
        {
            entity.Id = Guid.NewGuid();

            if (entity.TimeWindows != null && entity.TimeWindows.Any())
            {
                foreach (var timeWindow in entity.TimeWindows)
                {
                    timeWindow.TimeWindowListId = entity.Id;
                    Context.Entry(timeWindow).State = EntityState.Added;
                }
            }

            return base.Post(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override Task<IHttpActionResult> Put(Guid key, TimeWindowList entity)
        {
            CRUD(entity.TimeWindows,
                Context.TimeWindows.AsNoTracking().Where(tw => tw.TimeWindowListId == key).ToList(),
                timeWindow =>
                {
                    Context.Entry(timeWindow).State = EntityState.Added;
                },
                timeWindow =>
                {
                    Context.Entry(timeWindow).State = EntityState.Modified;
                },
                timeWindow =>
                {
                    Context.Entry(timeWindow).State = EntityState.Deleted;
                });

            return base.Put(key, entity);
        }
    }
}