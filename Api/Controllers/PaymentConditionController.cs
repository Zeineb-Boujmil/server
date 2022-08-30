using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Api.Controllers.Abstract;
using DataAccess;
using System.Web.Http;
using System.Web.OData;
using Api.Attributes;
using Api.Constants;

namespace Api.Controllers
{
    public class PaymentConditionController : BaseController<PaymentCondition>
    {
        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<PaymentCondition> Get()
        {
            return Context.Set<PaymentCondition>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<PaymentCondition> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<PaymentCondition>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Finance)]
        public override async Task<IHttpActionResult> Post(PaymentCondition entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Context.Set<PaymentCondition>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
       // [Auth(AuthActionTypes.Update, AuthRoles.Finance)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, PaymentCondition entity)
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