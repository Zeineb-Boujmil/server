using Api.Attributes;
using Api.Constants;
using Api.DmsService;
using DataAccess;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{
    public class OrganizationAccountODataController : ODataController
    {
        protected readonly MasterDataContext Context = new MasterDataContext();
        private readonly DmsServiceClient _dmsServiceClient;

        public OrganizationAccountODataController()
        {
            _dmsServiceClient = new DmsServiceClient();
        }

        [HttpPost]
       // [Auth(AuthActionTypes.Create, AuthRoles.Finance)]
        public async Task<IHttpActionResult> Post()
        {
            var requestData = await ReadRequestContentAndExtractData(Request);

            if (requestData.FileStream == null) ModelState.AddModelError("document", "Not received.");
            if (requestData.OrganizationAccountJson == null) ModelState.AddModelError("organizationAccount", "Not received.");
            if (requestData.FileName == null) ModelState.AddModelError("fileName", "Not received.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var organizationAccount = JsonConvert.DeserializeObject<OrganizationAccount>(requestData.OrganizationAccountJson);
            organizationAccount.Id = Guid.NewGuid();

            await CreateOrganizationAccountValidation(organizationAccount, requestData.FileName, requestData.FileStream);

            Context.Set<OrganizationAccount>().Add(organizationAccount);

            await Context.SaveChangesAsync();
            return Created(organizationAccount);
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public virtual IQueryable<OrganizationAccount> Get()
        {
            return Context.Set<OrganizationAccount>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Finance)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public SingleResult<OrganizationAccount> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<OrganizationAccount>().Where(e => e.Id == key));
        }

        [HttpPatch]
        //[Auth(AuthActionTypes.Update, AuthRoles.Finance)]
        [Auth(AuthRoles.Administrator, AuthActionTypes.Update)]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<OrganizationAccount> delta)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await Context.Set<OrganizationAccount>().FindAsync(key);

            if (entity == null)
            {
                return NotFound();
            }

            delta.Patch(entity);

            await Context.SaveChangesAsync();

            return Updated(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Finance)]
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key)
        {

            var requestData = await ReadRequestContentAndExtractData(Request);
            
            if (requestData.OrganizationAccountJson == null) ModelState.AddModelError("organizationAccount", "Not received.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var organizationAccount = JsonConvert.DeserializeObject<OrganizationAccount>(requestData.OrganizationAccountJson);

            var dbEntry = Context.OrganizationAccounts.First(e => e.Id == key);
            if (organizationAccount.IsApproved != null && dbEntry.IsApproved != organizationAccount.IsApproved)
            {
                var organizationAccountValidation = dbEntry.OrganizationAccountValidations.OrderByDescending(e => e.CreatedDate).First();
                organizationAccountValidation.IsApproved = (bool)organizationAccount.IsApproved;
            }

            if (requestData.FileName != null && requestData.FileStream != null)
            {
                await CreateOrganizationAccountValidation(organizationAccount, requestData.FileName, requestData.FileStream);
                organizationAccount.IsApproved = null;
            }
            Context.Entry(dbEntry).State = EntityState.Detached;
            Context.Entry(organizationAccount).State = EntityState.Modified;

            await Context.SaveChangesAsync();

            return Updated(organizationAccount);
        }

        [HttpDelete]
       // [Auth(AuthActionTypes.Delete, AuthRoles.Finance)]
        public async Task<IHttpActionResult> Delete([FromODataUri]Guid key)
        {
            var entity = new OrganizationAccount()
            {
                Id = key
            };

            var organizationAccountValidations = Context.OrganizationAccountValidations.Where(oav => oav.OrganizationAccountId == key);
            Context.OrganizationAccountValidations.RemoveRange(organizationAccountValidations);

            var organizationAccountAttachments = Context.OrganizationAccountAttachments.Where(oaa => oaa.OrganizationAccountId == key);
            Context.OrganizationAccountAttachments.RemoveRange(organizationAccountAttachments);

            Context.Entry(entity).State = EntityState.Deleted;
            await Context.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }

            base.Dispose(disposing);
        }

        private async Task<Guid> SendDocumentToDms(DataAccess.Document document, string organizationUnitId, string bankAccountId, string fileName, Stream fileStream)
        {
            var fileMemoryStream = new MemoryStream();
            fileStream.CopyTo(fileMemoryStream);

            var saveDocument = new SaveDocumentRequest
            {
                Document = new DmsService.Document
                {
                    Id = document.Id,
                    Indexes = new[]
                    {
                        new ArchiveIndex { Key = "organizationId", Value = organizationUnitId},
                        new ArchiveIndex { Key = "bankAccountId", Value = bankAccountId},
                    },
                    Pages = new[] {
                        new Page
                        {
                            Content = fileMemoryStream.ToArray(),
                            FileName = fileName,
                            Mime = MimeMapping.GetMimeMapping(fileName)
                        }
                    }
                }
            };

            var response = await _dmsServiceClient.SaveDocumentAsync(saveDocument);
            return response.Id;
        }

        private async Task CreateOrganizationAccountValidation(OrganizationAccount organizationAccount, string fileName, Stream fileStream)
        {
            var document = new DataAccess.Document
            {
                Id = Guid.NewGuid(),
                Description = $"IBAN validation {DateTime.Today:dd-MM-yyyy}",
                DocumentTypeId = Context.DocumentTypes.First(dt => dt.Code == "PDF").Id,
                DocumentDate = DateTime.UtcNow
            };
            Context.Documents.Add(document);

            var organizationAccountAttachment = new OrganizationAccountAttachment
            {
                Id = Guid.NewGuid(),
                OrganizationAccountId = organizationAccount.Id,
                DocumentId = document.Id,
            };
            Context.OrganizationAccountAttachments.Add(organizationAccountAttachment);

            var organizationAccountValidation = new OrganizationAccountValidation
            {
                StartDate = DateTime.UtcNow,
                OrganizationAccountId = organizationAccount.Id,
                OrganizationAccountAttachmentId = organizationAccountAttachment.Id
            };
            Context.OrganizationAccountValidations.Add(organizationAccountValidation);

            var result = await SendDocumentToDms(document, organizationAccount.OrganizationUnitId.ToString(),
                organizationAccount.BankAccountId.ToString(), fileName, fileStream);
            if (result == Guid.Empty)
            {
                throw new Exception($"DMS Service: Failed to save document with id ${document.Id} for organization account with id ${organizationAccount.Id}");
            }
        }

        private async Task<RequestData> ReadRequestContentAndExtractData(HttpRequestMessage request)
        {
            if (!request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await request.Content.ReadAsMultipartAsync(provider);

            var requestData = new RequestData();

            foreach (var content in provider.Contents)
            {
                switch (content.Headers.ContentDisposition.Name)
                {
                    case "\"organizationAccount\"":
                        requestData.OrganizationAccountJson = await content.ReadAsStringAsync();
                        break;
                    case "\"file\"":
                        requestData.FileName = content.Headers.ContentDisposition.FileName.Trim('\"');
                        requestData.FileStream = await content.ReadAsStreamAsync();
                        break;
                }
            }

            return requestData;
        }

        private class RequestData
        {
            public string OrganizationAccountJson { get; set; }
            public Stream FileStream { get; set; }
            public string FileName { get; set; }
        }
    }
}