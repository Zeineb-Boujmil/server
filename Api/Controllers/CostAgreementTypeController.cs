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
    public class CostAgreementTypeController : BaseController<CostAgreementType>
    {
        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<CostAgreementType> Get()
        {
            return Context.Set<CostAgreementType>();
        }

        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<CostAgreementType> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<CostAgreementType>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(CostAgreementType entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Context.Set<CostAgreementType>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
       // [Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, CostAgreementType entity)
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
