using DataAccess;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.Controllers
{
    [RoutePrefix("api/OrganizationUnit")]
    public class OrganizationUnitController : ApiController
    {
        private readonly MasterDataContext _context;

        public OrganizationUnitController()
        {
            _context = new MasterDataContext();
        }

        [HttpPut]
        [Route("approval-state")]
        public async Task<IHttpActionResult> ChnageApprovalState(ApprovalStateBody body)
        {
            var organizationUnit = _context.OrganizationUnits.AsNoTracking().FirstOrDefault(x => x.Id == body.Id);
            if (organizationUnit == null)
                return NotFound();

            var validationStatus = body.IsApproved ? "Approved" : "Disapproved";
            var organizationUnitValidationStatus = _context.OrganizationUnitValidationStatuses.AsNoTracking()
                .FirstOrDefault(x => x.ShortName == validationStatus);

            var organizationValidationStatusHistory = new OrganizationUnitValidationStatusHistory
            {
                OrganizationUnitId = body.Id,
                ValidationStatusId = organizationUnitValidationStatus.Id,
                Reason = body.Reason
            };

            _context.OrganizationUnitValidationStatusHistories.Add(organizationValidationStatusHistory);
            await _context.SaveChangesAsync();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }

        public class ApprovalStateBody
        {
            public Guid Id { get; set; }
            public bool IsApproved { get; set; }
            public string Reason { get; set; }
        }
    }
}
