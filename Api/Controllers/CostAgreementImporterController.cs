using Api.Importing;
using DataAccess;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Api.Attributes;
using Api.Constants;

namespace Api.Controllers
{
    [Route("api/CostAgreementImporter")]
    public class CostAgreementImporterController : ApiController
    {
        private readonly MasterDataContext _context = new MasterDataContext();

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthActionTypes.Update, AuthRoles.OrganizationUnit)]
        public async Task<IHttpActionResult> Post()
        {
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            var fileStream = provider.Contents.First().ReadAsStreamAsync().Result;

            var importedCostAgreements = CostAgreementImportedFromCsv
                .FromStream(fileStream)
                .ToList();

            for (var i = 0; i < importedCostAgreements.Count; i++)
                if (!importedCostAgreements[i].IsValid(out var validationErrors))
                    validationErrors.ForEach(e => ModelState.AddModelError("csv", $"(row: {i}, {e.Key}) {e.Value}"));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var costAgreements = importedCostAgreements
                .Select(c => c.ToCostAgreement())
                .ToList();

            var importedIds = costAgreements.Select(s => s.Id).ToList();

            var existingCostAgreements = _context.CostAgreements
                .AsNoTracking()
                .Include(x => x.CostAgreementType)
                .Where(ca => importedIds.Contains(ca.Id))
                .ToList();

            return Ok(new
            {
                importedCostAgreements = costAgreements.Where(c => existingCostAgreements.All(e => !e.IsEqualTo(c)))
            });
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