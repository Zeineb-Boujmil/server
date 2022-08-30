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
    public class TransmissionController : BaseController<Transmission>
    {
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<Transmission> Get()
        {
            return Context.Set<Transmission>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<Transmission> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<Transmission>().Where(e => e.Id == key));
        }

        [HttpPost]
       // [Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(Transmission entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Context.Set<Transmission>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, Transmission entity)
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