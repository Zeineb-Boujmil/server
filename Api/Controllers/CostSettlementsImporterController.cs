using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Api.Attributes;
using Api.Constants;
using Api.Importing;
using DataAccess;

namespace Api.Controllers
{
    [Route("api/CostSettlementImporter")]
    public class CostSettlementImporterController : ApiController
    {
        private readonly MasterDataContext _context;

        public CostSettlementImporterController()
        {
            _context = new MasterDataContext();
            _context.Configuration.ProxyCreationEnabled = false;
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Update, AuthActionTypes.Create, AuthRoles.OrganizationUnit)]
        public async Task<IHttpActionResult> Post()
        {
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            var fileStream = provider.Contents.First().ReadAsStreamAsync().Result;

            var importedCostSettlements = CostSettlementImportedFromCsv
                .FromStream(fileStream)
                .ToList();

            for (var i = 0; i < importedCostSettlements.Count; i++)
                if (!importedCostSettlements[i].IsValid(out var validationErrors))
                    validationErrors.ForEach(e => ModelState.AddModelError("csv", $"(row: {i}, {e.Key}) {e.Value}"));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var costAgreementCodeIds = importedCostSettlements.SelectMany(x => x.AgreementCodes.Split('|'));
            var costAgreements = _context.CostAgreements
                .AsNoTracking()
                .Where(x => costAgreementCodeIds.Contains(x.AgreementCode))
                .ToList();


            var costSettlements = importedCostSettlements
                .Select(c => c.ToCostSettlement(costAgreements))
                .ToList();

            var importedIds = costSettlements.Select(s => s.Id).ToList();

            var existingCostSettlements = _context.CostSettlements
                .AsNoTracking()
                .Include(cs => cs.CostSettlementLines)
                .Include(cs => cs.CostSettlementLines.Select(x => x.CostAgreement))
                .Where(cs => importedIds.Contains(cs.Id))
                .ToList();

            return Ok(new
            {
                importedCostSettlements = costSettlements.Where(c => existingCostSettlements.All(e => !e.IsEqualTo(c)))
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