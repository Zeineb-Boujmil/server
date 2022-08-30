using System;
using System.Runtime.Serialization;

namespace MasterDataService.DTO.Entities
{
	[DataContract(Namespace = Constants.DataContractNamespace)]
	public class OrganizationAccountDto
	{
		[DataMember(Order = 0, IsRequired = true)]
		public Guid Id { get; set; }

		[DataMember(Order = 1, IsRequired = true)]
		public Guid OrganizationUnitId { get; set; }

		[DataMember(Order = 2, IsRequired = true)]
		public Guid BankAccountId { get; set; }

		[DataMember(Order = 3, IsRequired = true)]
		public string AccountCode { get; set; }

		[DataMember(Order = 4, IsRequired = true)]
		public string AccountName { get; set; }

		[DataMember(Order = 5, IsRequired = false)]
		public DateTime? CreatedDate { get; set; }

		[DataMember(Order = 6, IsRequired = false)]
		public string CreatedBy { get; set; }

		[DataMember(Order = 7, IsRequired = false)]
		public DateTime? LastModifiedDate { get; set; }

		[DataMember(Order = 8, IsRequired = false)]
		public string LastModifiedBy { get; set; }
	}
}