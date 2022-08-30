using System;
using System.Runtime.Serialization;
using CED.Framework.Wcf;

namespace MasterDataService.DTO.Messages
{
	[DataContract(Namespace = Constants.DataContractNamespace)]
	public class CreateBankAccountResponse : ResponseBase
	{
	    [DataMember]
	    public Guid OrganizationAccountId { get; set; }

	    [DataMember]
	    public Guid OrganizationAccountAttachmentId { get; set; }

	    [DataMember]
	    public Guid OrganizationAccountValidationId { get; set; }
    }
}