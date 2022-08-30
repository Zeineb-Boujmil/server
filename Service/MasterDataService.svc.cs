using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CED.Framework.Interfaces;
using CED.Framework.Logging;
using CED.Framework.Wcf;
using DataAccess;
using MasterDataService.DTO.Messages;

namespace MasterDataService
{
	public class MasterDataService : ServiceBase, IMasterDataService
	{
		private readonly IMapper _mapper;

		public MasterDataService(ILogger log, IMapper mapper, IPingRepository pingRepository)
            : base(pingRepository, log)
		{
			_mapper = mapper;
		}

		public CreateOrganizationUnitResponse CreateOrganization(CreateOrganizationUnitRequest request)
		{
			return HandleRequest(request, CreateOrganizationImpl);
		}

		public CreateBankAccountResponse CreateBankAccount(CreateBankAccountRequest request)
		{
			return HandleRequest(request, CreateBankAccountImpl);
		}

	    public UpdateDocumentResponse UpdateDocument(UpdateDocumentRequest request)
	    {
	        return HandleRequest(request, UpdateDocumentImp);
	    }

		private CreateBankAccountResponse CreateBankAccountImpl(CreateBankAccountRequest request)
		{
			var entity = _mapper.Map<BankAccount>(request.BankAccount);            

		    var organizationAccountId = Guid.NewGuid();
		    var organizationAccountAttachmentId = Guid.NewGuid();
		    var organizationAccountValidationId = Guid.NewGuid();

            using (var ctx = new MasterDataContext())
			{
			    var bankAccount = ctx.BankAccounts.Add(entity);

			    if (request.OrganizationUnitId != Guid.Empty && request.Document != null)
			    {
			        var document = _mapper.Map<Document>(request.Document);
			        ctx.Documents.Add(document);
			        var organizationAccount = new OrganizationAccount
			        {
			            Id = organizationAccountId,
			            OrganizationUnitId = request.OrganizationUnitId,
			            BankAccountId = bankAccount.Id
			        };
			        ctx.OrganizationAccounts.Add(organizationAccount);

			        var organizationAccountAttachment = new OrganizationAccountAttachment
			        {
			            Id = organizationAccountAttachmentId,
			            OrganizationAccountId = organizationAccount.Id,
			            DocumentId = request.Document.Id,
			        };
			        ctx.OrganizationAccountAttachments.Add(organizationAccountAttachment);

			        var organizationAccountValidation = new OrganizationAccountValidation
			        {
                        Id = organizationAccountValidationId,
			            StartDate = DateTime.UtcNow,
			            OrganizationAccountId = organizationAccount.Id,
			            OrganizationAccountAttachmentId = organizationAccountAttachment.Id
			        };
			        ctx.OrganizationAccountValidations.Add(organizationAccountValidation);
                }
                else
			    {
			        return new CreateBankAccountResponse
			        {
			            ExceptionMessage = request.OrganizationUnitId == Guid.Empty ? "OrganizationUnitId is missing" : "Document is missing",
			            ExceptionOccurred = true
			        };
			    }

                ctx.SaveChanges();
            }

		    return new CreateBankAccountResponse
		    {
		        OrganizationAccountId = organizationAccountId,
		        OrganizationAccountAttachmentId = organizationAccountAttachmentId,
		        OrganizationAccountValidationId = organizationAccountValidationId
		    };
        }

		private CreateOrganizationUnitResponse CreateOrganizationImpl(CreateOrganizationUnitRequest request)
		{
			var entity = _mapper.Map<OrganizationUnit>(request.OrganizationUnit);
			

			entity.ValidationStatusHistories = new List<OrganizationUnitValidationStatusHistory>()
				{
					new OrganizationUnitValidationStatusHistory()
					{
						Id = Guid.NewGuid(),
						OrganizationUnitId = entity.Id,
						ValidationStatusId = new Guid("13773DE4-8332-4066-9148-108E889F6ADC"),
						Reason = "Created using the service.",
					}
				};

			using (var context = new MasterDataContext())
			{
				if (entity.Address != null)
				{
					var existingAddress = context.Addresses.SingleOrDefault(a => a.Id == entity.Address.Id);

					if (existingAddress != null)
					{
						entity.Address = existingAddress;
					}

				}

				if (entity.PostOfficeBox != null)
				{
					var existingPostOfficeBox = context.PostOfficeBoxes.SingleOrDefault(p => p.Id == entity.PostOfficeBox.Id);

					if (existingPostOfficeBox != null)
					{
						entity.PostOfficeBox = existingPostOfficeBox;
					}
				}

				context.OrganizationUnits.Add(entity);
				context.SaveChanges();
			}

			return new CreateOrganizationUnitResponse();
		}

	    private UpdateDocumentResponse UpdateDocumentImp(UpdateDocumentRequest request)
	    {
	        return new UpdateDocumentResponse();
	    }
	}
}