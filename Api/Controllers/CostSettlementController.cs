using System;
using System.Data.Entity;
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
    public class CostSettlementController : BaseController<CostSettlement>
    {
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<CostSettlement> Get()
        {
            return Context.Set<CostSettlement>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<CostSettlement> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<CostSettlement>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.OrganizationUnit)]
        public override async Task<IHttpActionResult> Post(CostSettlement entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Context.Set<CostSettlement>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.OrganizationUnit)]
        public override async Task<IHttpActionResult> Put(Guid key, CostSettlement costSettlement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (costSettlement.CostSettlementLines != null && costSettlement.CostSettlementLines.Count > 0)
            {
                var costSettlementLineIds = costSettlement.CostSettlementLines.Select(e => e.Id).ToList();
                var costSettlementLinesForRemove = Context.CostSettlementLines
                    .Where(e => e.CostSettlementId == costSettlement.Id)
                    .Where(e => !costSettlementLineIds.Contains(e.Id));
                Context.CostSettlementLines.RemoveRange(costSettlementLinesForRemove);
                foreach (var costSettlementLine in costSettlement.CostSettlementLines)
                {
                    costSettlementLine.CostSettlementId = costSettlement.Id;
                    if(costSettlementLine.CostAgreement != null)
                        Context.Entry(costSettlementLine.CostAgreement).State = EntityState.Unchanged;
                    Context.CostSettlementLines.AddOrUpdate(costSettlementLine);
                }
            }
            else
            {
                var costSettlementLinesForRemove = Context.CostSettlementLines.Where(e => e.CostSettlementId == costSettlement.Id);
                Context.CostSettlementLines.RemoveRange(costSettlementLinesForRemove);
            }

            Context.CostSettlements.AddOrUpdate(costSettlement);
            await Context.SaveChangesAsync();

            return Updated(costSettlement);
        }
    }
}