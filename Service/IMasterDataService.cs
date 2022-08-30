using System.ServiceModel;
using CED.Framework.Wcf;
using MasterDataService.DTO.Messages;

namespace MasterDataService
{
    [ServiceContract(Namespace = Constants.ServiceContractNamespace)]
    public interface IMasterDataService : IPingService
    {
        [OperationContract(Action = nameof(CreateOrganization))]
        CreateOrganizationUnitResponse CreateOrganization(CreateOrganizationUnitRequest request);

		[OperationContract(Action = nameof(CreateBankAccount))]
	    CreateBankAccountResponse CreateBankAccount(CreateBankAccountRequest request);

        [OperationContract(Action = nameof(UpdateDocument))]
        UpdateDocumentResponse UpdateDocument(UpdateDocumentRequest request);
    }
}