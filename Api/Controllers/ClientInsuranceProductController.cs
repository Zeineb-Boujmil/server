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
    public class ClientInsuranceProductController : BaseController<ClientInsuranceProduct>
    {
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.OrganizationUnit)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<ClientInsuranceProduct> Get()
        {
            return Context.Set<ClientInsuranceProduct>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.OrganizationUnit)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<ClientInsuranceProduct> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<ClientInsuranceProduct>().Where(e => e.Id == key));
        }

        [HttpPost]
       // [Auth]
        public override async Task<IHttpActionResult> Post(ClientInsuranceProduct entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Context.Set<ClientInsuranceProduct>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
       // [Auth]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, ClientInsuranceProduct entity)
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