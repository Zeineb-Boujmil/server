using System;
using Api.Exporting;
using CsvHelper;
using CsvHelper.Configuration;
using DataAccess;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Api.Attributes;
using Api.Constants;

namespace Api.Controllers
{
    [RoutePrefix("api/OrganizationExporter")]
    public class OrganizationExporterController : ApiController
    {
        private readonly MasterDataContext _context;

        public OrganizationExporterController()
        {
            _context = new MasterDataContext();
            _context.Configuration.ProxyCreationEnabled = false;
            _context.Database.CommandTimeout = 300;
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [Route("ExportSuppliers")]
        public async Task<HttpResponseMessage> ExportSuppliers()
        {
            var contactTypes = _context.ContactTypes.ToList();
            var suppliers = await _context.Suppliers
                .AsNoTracking()
                .Select(s => s.OrganizationUnit)
                .Include(s => s.OrganizationCodes)
                .Include(ou => ou.OrganizationAddresses)
                .Include(ou => ou.OrganizationAddresses.Select(a => a.AddressType))
                .Include(ou => ou.OrganizationAddresses.Select(a => a.Address))
                .Include(ou => ou.OrganizationAddresses.Select(a => a.PostOfficeBox))
                .Include(ou => ou.OrganizationContacts.Select(c => c.ContactType))
                .Include(ou => ou.Supplier)
                .Include(ou => ou.InternationalAssistanceGroup)
                .Include(ou => ou.InternationalAssistanceGroup.InternationalAssistanceGroupType)
                .Include(ou => ou.OrganizationNotes)
                .Include(ou => ou.OrganizationLabels)
                .Include(ou => ou.OrganizationLabels.Select(l => l.OrganizationLabelType))
                .ToListAsync();


            var csvSuppliers = suppliers.Select(ou => OrganizationUnitExportedFromCsv.FromOrganizationUnit(ou, contactTypes));

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    var csv = new CsvWriter(writer, new Configuration { Delimiter = ";", HeaderValidated = null, MissingFieldFound = null });
                    csv.WriteRecords(csvSuppliers);
                    writer.Flush();

                    var fileBytes = stream.ToArray();
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };
                    result.Content.Headers.Add("x-filename", "Suppliers.csv");
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "Suppliers.csv"
                    };
                    result.Content.Headers.ContentLength = fileBytes.Length;
                    return result;
                }
            }
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
