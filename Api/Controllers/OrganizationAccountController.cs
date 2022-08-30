using Api.Attributes;
using Api.Constants;
using Api.DmsService;
using DataAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Api.ViewModels;

namespace Api.Controllers
{
    [Route("api/OrganizationAccount")]
    public class OrganizationAccountController : ApiController
    {
        private readonly MasterDataContext Context = new MasterDataContext();
        protected readonly DmsServiceClient _dmsServiceClient = new DmsServiceClient();


        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        public IHttpActionResult Get([FromUri] Guid key)
        {
            var organizationAccount = Context
                .OrganizationAccounts
                .Include(oa => oa.BankAccount)
                .AsNoTracking()
                .FirstOrDefault(oa => oa.Id == key);
            if (organizationAccount == null)
                return NotFound();
            return Ok(organizationAccount);
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Update, AuthRoles.Finance)]
        public async Task<IHttpActionResult> Post([FromBody] OrganizationAccount organizationAccount)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await Context.Set<OrganizationAccount>().FindAsync(organizationAccount.Id);


            if (entity == null)
            {
                return NotFound();
            }

            if (entity.IsApproved != null)
            {
                return Content(HttpStatusCode.BadRequest, "Already approved/disapproved");
            }

            var organizationAccountAttachment = Context.OrganizationAccountAttachments
                .Where(oa => oa.OrganizationAccountId == organizationAccount.Id)
                .OrderByDescending(oa => oa.CreatedDate)
                .FirstOrDefault();

            var organizationAccountValidation = new OrganizationAccountValidation
            {
                StartDate = DateTime.UtcNow,
                OrganizationAccountId = entity.Id,
                OrganizationAccountAttachmentId = organizationAccountAttachment?.Id,
                IsApproved = organizationAccount.IsApproved ?? false
            };

            Context.OrganizationAccountValidations.Add(organizationAccountValidation);
            entity.IsApproved = organizationAccount.IsApproved;
            var accountHolderName = organizationAccount.BankAccount?.AccountName;
            if (!string.IsNullOrWhiteSpace(accountHolderName))
            {
                entity.BankAccount.AccountName = accountHolderName;
            }

            await Context.SaveChangesAsync();

            return Ok(organizationAccount);
        }

	    [HttpPut]
	   // [Auth(AuthActionTypes.Update, AuthRoles.Finance)]
		[Route("api/OrganizationAccount/approval")]
	    public async Task<IHttpActionResult> ApproveOrganizationAccounts(OrganizationAccountsApprovalRequest model)
	    {
		    if (model.OrganizationAccountIds == null || !model.OrganizationAccountIds.Any())
			    return BadRequest("No OrganizationAccountId provided");

		    var organizationAccounts = await Context.OrganizationAccounts
			    .Where(a => model.OrganizationAccountIds.Contains(a.Id)).ToListAsync();

		    foreach (OrganizationAccount organizationAccount in organizationAccounts)
		    {
			    organizationAccount.IsApproved = model.IsApproved;
		    }

		    await Context.SaveChangesAsync();
		    return Ok();
	    }

        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.Finance)]
        [Route("api/OrganizationAccount/document/{organizationAccountId}")]
        public async Task<HttpResponseMessage> GetDocuments([FromUri]Guid organizationAccountId)
        {

            var organizationAccountAttachment = Context.OrganizationAccountAttachments
                .Where(oaa => oaa.OrganizationAccountId == organizationAccountId)
                .OrderByDescending(oaa => oaa.CreatedDate)
                .FirstOrDefault();

            if (organizationAccountAttachment == null)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("Document for this organization account doesn’t exist")
                };
            }
           
            var document = await _dmsServiceClient.GetDocumentByIdAsync(new GetDocumentByIdRequest
            {
                Id = organizationAccountAttachment.DocumentId
            });

            if (document.Document == null)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent($"Document with id {organizationAccountAttachment.DocumentId} not found in the DMS at {_dmsServiceClient.Endpoint.Address.Uri}.")
                };
            }

            var fileName = document.Document.Pages[0].FileName.Split('/').Last();

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(document.Document.Pages[0].Content)
            };
            result.Content.Headers.Add("x-filename", fileName);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(document.Document.Pages[0].Mime);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            result.Content.Headers.ContentLength = document.Document.Pages[0].Content.Length;

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }

            base.Dispose(disposing);
        }
    }

}