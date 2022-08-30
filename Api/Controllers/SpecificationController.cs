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
    public class SpecificationController : BaseController<Specification>
    {
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<Specification> Get()
        {
            return Context.Set<Specification>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<Specification> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<Specification>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(Specification entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Context.Set<Specification>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, Specification entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Context.Entry(entity).State = EntityState.Modified;

            await Context.SaveChangesAsync();

            return Updated(entity);
        }
    }
}