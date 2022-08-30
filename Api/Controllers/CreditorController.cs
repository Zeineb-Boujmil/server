using DataAccess;
using System;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using Api.Attributes;
using Api.Constants;

namespace Api.Controllers
{
    [Route("api/Creditor")]
    public class CreditorController : ApiController
    {
        private readonly MasterDataContext _context;

        public CreditorController()
        {
            _context = new MasterDataContext();
            _context.Configuration.ProxyCreationEnabled = false;
        }

        [HttpGet]
        [Route("api/Creditor/{key}")]
        //[Auth(AuthActionTypes.Read, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        public IHttpActionResult Get([FromUri]Guid key)
        {
            var response = _context
                .Creditors
                .Include(x => x.Address)
                .Include(x => x.PostOfficeBox)
                .Include(x => x.CreditorOrganizationRelations.Select(y => y.OrganizationUnit.OrganizationPaymentConditions.Select(z => z.PaymentCondition)))
                .Include(x => x.CreditorOrganizationRelations.Select(y => y.OrganizationUnit.OrganizationPaymentMethods))
                .Include(x => x.CreditorAccounts.Select(y => y.BankAccount))
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == key);

            if (response != null && response.CreditorOrganizationRelations.Any())
            {
                foreach (var creditorOrganizationRelation in response.CreditorOrganizationRelations)
                {
                    if (creditorOrganizationRelation.OrganizationUnit.OrganizationPaymentConditions.Any())
                        creditorOrganizationRelation.OrganizationUnit.OrganizationPaymentConditions =
                            creditorOrganizationRelation.OrganizationUnit.OrganizationPaymentConditions
                                .Where(x => x.PaymentCondition.LegalEntityId == response.LegalEntityId).ToList();

                    if (creditorOrganizationRelation.OrganizationUnit.OrganizationPaymentMethods.Any())
                        creditorOrganizationRelation.OrganizationUnit.OrganizationPaymentMethods =
                            creditorOrganizationRelation.OrganizationUnit.OrganizationPaymentMethods
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
