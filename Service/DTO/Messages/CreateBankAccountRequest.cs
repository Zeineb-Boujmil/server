using System;
using System.Runtime.Serialization;
using CED.Framework.Wcf;
using MasterDataService.DTO.Entities;

namespace MasterDataService.DTO.Messages
{
	[DataContract(Namespace = Constants.DataContractNamespace)]
	public class CreateBankAccountRequest : RequestBase
	{
		[DataMember(Order = 0, IsRequired = true)]
		public BankAccountDto BankAccount { get; set; }

	    [DataMember(Order = 1, IsRequired = false)]
	    public DocumentDto Document { get; set; }

	    [DataMember(Order = 2, IsRequired = false)]
	    public Guid OrganizationUnitId { get; set; }

        public override void Validate()
		{
			
		}
	}
}