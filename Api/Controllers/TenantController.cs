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
    public class TenantController : BaseController<Tenant>
    {

        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<Tenant> Get()
        {
            return Context.Set<Tenant>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<Tenant> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<Tenant>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        public override async Task<IHttpActionResult> Post(Tenant entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Context.Set<Tenant>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, Tenant entity)
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