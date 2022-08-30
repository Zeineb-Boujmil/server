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
    public class EmployeeController : BaseController<Employee>
    {
        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.UserConfiguration, AuthRoles.EmployeeAccess)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<Employee> Get()
        {
            return Context.Set<Employee>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.UserConfiguration, AuthRoles.EmployeeAccess)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<Employee> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<Employee>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator, AuthRoles.UserConfiguration, AuthRoles.EmployeeAccess)]
        public override async Task<IHttpActionResult> Post(Employee entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Context.Set<Employee>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator, AuthRoles.UserConfiguration, AuthRoles.EmployeeAccess)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, Employee entity)
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