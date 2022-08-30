using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MasterDataService.DTO.Entities
{
	[DataContract(Namespace = Constants.DataContractNamespace)]
	public class BankAccountDto
	{
		[DataMember(Order = 0, IsRequired = true)]
		public Guid Id { get; set; }

		[DataMember(Order = 1, IsRequired = true)]
		public string AccountNumber { get; set; }

		[DataMember(Order = 2, IsRequired = true)]
		public string AccountName { get; set; }

		[DataMember(Order = 3, IsRequired = false)]
		public string AccountAddressLine1 { get; set; }

		[DataMember(Order = 4, IsRequired = false)]
		public string AccountAddressLine2 { get; set; }

		[DataMember(Order = 5, IsRequired = false)]
		public string BankName { get; set; }

		[DataMember(Order = 6, IsRequired = false)]
		public string IBAN { get; set; }

		[DataMember(Order = 7, IsRequired = false)]
		public string BIC { get; set; }

		[DataMember(Order = 8, IsRequired = true)]
		public string CurrencyCode { get; set; }

		[DataMember(Order = 9, IsRequired = true)]
		public string CountryCode { get; set; }

		[DataMember(Order = 10, IsRequired = false)]
		public string AccountIdentifier { get; set; }

		[DataMember(Order = 11, IsRequired = true)]
		public bool IsSepaAccount { get; set; }

		[DataMember(Order = 12, IsRequired = true)]
		public bool IsBlockedAccount { get; set; }

		[DataMember(Order = 13, IsRequired = true)]
		public bool Inactive { get; set; }

		[DataMember(Order = 14, IsRequired = false)]
		public DateTime? CreatedDate { get; set; }

		[DataMember(Order = 15, IsRequired = false)]
		public string CreatedBy { get; set; }

		[DataMember(Order = 16, IsRequired = false)]
		public DateTime? LastModifiedDate { get; set; }

		[DataMember(Order = 17, IsRequired = false)]
		public string LastModifiedBy { get; set; }

		[DataMember(Order = 18, IsRequired = false)]
		public string AccountAbbreviation { get; set; }

		[DataMember(Order = 19, IsRequired = true)]
		public bool IsValidated { get; set; }

		[DataMember(Order = 20, IsRequired = true)]
		public bool IsApproved { get; set; }

		[DataMember(Order = 21, IsRequired = true)]
		public List<OrganizationAccountDto> OrganizationAccounts { get; set; }
	}
}