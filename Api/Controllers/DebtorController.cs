using Api.Attributes;
using Api.Constants;
using DataAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace Api.Controllers
{
    [Route("api/Debtor")]
    public class DebtorController : ApiController
    {
        private readonly MasterDataContext _context;

        public DebtorController()
        {
            _context = new MasterDataContext();
            _context.Configuration.ProxyCreationEnabled = false;
        }

        [HttpGet]
        [Route("api/Debtor/{key}")]
       // [Auth(AuthActionTypes.Read, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        public IHttpActionResult Get([FromUri]Guid key)
        {
            var response = _context
                .Debtors
                .Include(x => x.Address)
                .Include(x => x.PostOfficeBox)
                .Include(x => x.DebtorOrganizationRelations.Select(y => y.OrganizationUnit.OrganizationPaymentConditions.Select(z => z.PaymentCondition)))
                .Include(x => x.DebtorOrganizationRelations.Select(y => y.OrganizationUnit.OrganizationPaymentMethods))
                .Include(x => x.DebtorAccounts.Select(y => y.BankAccount))
                .Include(x => x.DebtorAttributes)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == key);

            if (response != null && response.DebtorOrganizationRelations.Any())
            {
                foreach (var debtorOrganizationRelation in response.DebtorOrganizationRelations)
                {
                    if (debtorOrganizationRelation.OrganizationUnit.OrganizationPaymentConditions.Any())
                        debtorOrganizationRelation.OrganizationUnit.OrganizationPaymentConditions =
                            debtorOrganizationRelation.OrganizationUnit.OrganizationPaymentConditions
                                .Where(x => x.PaymentCondition.LegalEntityId == response.LegalEntityId).ToList();

                    if (debtorOrganizationRelation.OrganizationUnit.OrganizationPaymentMethods.Any())
                        debtorOrganizationRelation.OrganizationUnit.OrganizationPaymentMethods =
                            debtorOrganizationRelation.OrganizationUnit.OrganizationPaymentMethods
                                .Where(x => x.LegalEntityId == response.LegalEntityId).ToList();
                }
            }

            return Ok(response);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}
