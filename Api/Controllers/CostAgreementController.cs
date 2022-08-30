using System;
using System.Data.Entity.Migrations;
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
    public class CostAgreementController : BaseController<CostAgreement>
    {
        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<CostAgreement> Get()
        {
            return Context.Set<CostAgreement>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<CostAgreement> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<CostAgreement>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.OrganizationUnit)]
        public override async Task<IHttpActionResult> Post(CostAgreement costAgreement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            costAgreement.Id = new Guid();


            var fixedCostAgreement = new FixedCostAgreement
            {
                Id = costAgreement.Id,
                FixedCostAmount = costAgreement.FixedCostAgreement.FixedCostAmount
            };

            costAgreement.FixedCostAgreement = null;
            Context.CostAgreements.Add(costAgreement);
            await Context.SaveChangesAsync();

            fixedCostAgreement.CostAgreement = costAgreement;
            Context.FixedCostAgreements.Add(fixedCostAgreement);
            await Context.SaveChangesAsync();

            return Created(costAgreement);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.OrganizationUnit)]
        public override async Task<IHttpActionResult> Put(Guid key, CostAgreement costAgreement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (costAgreement.FixedCostAgreement != null)
            {
                costAgreement.FixedCostAgreement.Id = costAgreement.Id;
                Context.FixedCostAgreements.AddOrUpdate(costAgreement.FixedCostAgreement);
            }

            Context.CostAgreements.AddOrUpdate(costAgreement);
            await Context.SaveChangesAsync();

            return Updated(costAgreement);
        }
    }
}