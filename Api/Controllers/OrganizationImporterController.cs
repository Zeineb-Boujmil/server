using Api.Attributes;
using Api.Constants;
using Api.Importing;
using Api.ViewModels;
using CsvHelper;
using CsvHelper.Configuration;
using DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Api.Extensions;
using Newtonsoft.Json.Serialization;

namespace Api.Controllers
{
    [Route("api/OrganizationImporter")]
    public class OrganizationImporterController : ApiController
    {
        private readonly MasterDataContext _context;

        public OrganizationImporterController()
        {
            _context = new MasterDataContext();
            _context.Configuration.ProxyCreationEnabled = false;
        }

        [HttpPost]
       // [Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public async Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            Task<string> configJsonTask = null;
            Task<Stream> fileStreamTask = null;

            foreach (var content in provider.Contents)
            {
                switch (content.Headers.ContentDisposition.Name)
                {
                    case "\"config\"":
                        configJsonTask = content.ReadAsStringAsync();
                        break;
                    case "\"file\"":
                        fileStreamTask = content.ReadAsStreamAsync();
                        break;
                }
            }

            if (fileStreamTask == null) ModelState.AddModelError("file", "Not recieved.");
            if (configJsonTask == null) ModelState.AddModelError("config", "Not recieved.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var importedOrganizationUnitsFromCSV = new CsvReader(new StreamReader(fileStreamTask.Result), new Configuration { Delimiter = ";", HeaderValidated = null, MissingFieldFound = null })
                .GetRecords<OrganizationUnitImportedFromCSV>()
                .ToList();

            const int chunkSize = 4000;
            var importedOrganizationUnitChunks = importedOrganizationUnitsFromCSV
                .Select((x, i) => new {Index = i, Value = x})
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

            var contactTypes = _context.ContactTypes.ToList();

            var config = JsonConvert.DeserializeObject<UploadConfiguration>(await configJsonTask);

            var applications = new List<Application>();
            if (config.OrganizationApplicationIds != null)
                applications = _context.Applications
                    .AsNoTracking()
                    .Where(x => config.OrganizationApplicationIds.Contains(x.Id.ToString()))
                    .ToList();

            var result = new
            {
                NewOrganizations = new List<OrganizationUnit>(),
                MutatedOrganizations = new List<(OrganizationUnit ExistingOrganizationUnit, OrganizationUnit importedOrganization)>(),
                ContactTypes = contactTypes.Select(c => new { c.Id, c.Code, c.ShortName, c.LongName })
            };

            foreach (var importedOrganizationUnits in importedOrganizationUnitChunks)
            {
                var importedOrganizationIds = importedOrganizationUnits.Where(o => !o.Id.IsNullOrEmpty()).Select(o => Guid.Parse(o.Id)).ToList();

                var existingOrganizations = await _context.OrganizationUnits
                    .AsNoTracking()
                    .Include(ou => ou.OrganizationCodes)
                    .Include(ou => ou.OrganizationAddresses)
                    .Include(ou => ou.OrganizationApplications.Select(a => a.Application))
                    .Include(ou => ou.OrganizationAddresses.Select(a => a.Address))
                    .Include(ou => ou.OrganizationAddresses.Select(a => a.PostOfficeBox))
                    .Include(ou => ou.Agent)
                    .Include(ou => ou.Client)
                    .Include(ou => ou.Client.Mandate)
                    .Include(ou => ou.OrganizationContacts)
                    .Include(ou => ou.Repairer)
                    .Include(ou => ou.Repairer.RepairerSkills)
                    .Include(ou => ou.Partner)
                    .Include(ou => ou.Supplier)
                    .Include(ou => ou.AlarmCenter)
                    .Include(ou => ou.InternationalAssistanceGroup)
                    .Include(ou => ou.OrganizationNotes)
                    .Include(ou => ou.OrganizationLabels)
                    .Where(ou => importedOrganizationIds.Contains(ou.Id))
                    .ToListAsync();

                var addressTypes = _context.AddressTypes.ToList();
                var labelTypes = _context.OrganizationLabelTypes.ToList();
                var internationalAssistanceGroupTypes = _context.InternationalAssistanceGroupTypes.ToList();

                for (var i = 0; i < importedOrganizationUnits.Count; i++)
                    if (!importedOrganizationUnits[i].IsValid(out var validationErrors, existingOrganizations, addressTypes, labelTypes, internationalAssistanceGroupTypes))
                        validationErrors.ForEach(e => ModelState.AddModelError("csv", $"(row: {i}, {e.Key}) {e.Value}"));
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                foreach (var importedOrganizationUnit in importedOrganizationUnits)
                {
                    var importedOrganization = importedOrganizationUnit.ToOrganizationUnit(config, contactTypes, addressTypes, labelTypes, internationalAssistanceGroupTypes, applications);

                    var matchedExistingOrganization = existingOrganizations.FirstOrDefault(ou => ou.Id == importedOrganization.Id);
                    if (matchedExistingOrganization == null)
                        result.NewOrganizations.Add(importedOrganization);
                    else
                    {
                        if (!importedOrganization.IsEqualTo(matchedExistingOrganization))
                        {
                            if (config.IsSupplier)
                            {
                                matchedExistingOrganization.Supplier = matchedExistingOrganization.Supplier ?? new Supplier();

                                matchedExistingOrganization.Supplier.SupplierServices = matchedExistingOrganization.Supplier.SupplierServices ?? new List<SupplierService>();

                                matchedExistingOrganization.Supplier.SupplierServices = config.ServiceIds?
                                    .Where(id => matchedExistingOrganization.Supplier.SupplierServices.All(ss => ss.ServiceId != new Guid(id)))
                                    .Select(id => new SupplierService
                                    {
                                        ServiceId = new Guid(id),
                                        EffectiveDate = DateTime.UtcNow
                                    })
                                    .ToList();
                            }
                            result.MutatedOrganizations.Add((matchedExistingOrganization, importedOrganization));
                        }
                    }
                }
            }

            return Ok(result);
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