using System;
using System.Runtime.Serialization;

namespace MasterDataService.DTO.Entities
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class OrganizationUnitDto
    {
        [DataMember(Order = 0, IsRequired = true)]
        public Guid Id { get; set; }

        [DataMember(Order = 1, IsRequired = true)]
        public string Code { get; set; }

        [DataMember(Order = 2)]
        public string ShortName { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public string LongName { get; set; }

        [DataMember(Order = 4)]
        public string PorCode { get; set; }

        [DataMember(Order = 5)]
        public DateTime? StartDate { get; set; }

        [DataMember(Order = 5)]
        public DateTime? EndDate { get; set; }

        [DataMember(Order = 6, IsRequired = true)]
        public bool Inactive { get; set; }

        [DataMember(Order = 7)]
        public Guid? OrganizationTypeId { get; set; }

        [DataMember(Order = 8)]
        public Guid? HierarchyTypeId { get; set; }

        [DataMember(Order = 9)]
        public Guid? OrganizationGroupId { get; set; }

        [DataMember(Order = 10)]
        public Guid? CollectionCodeId { get; set; }

        [DataMember(Order = 11)]
        public string Website { get; set; }

        [DataMember(Order = 12)]
        public string Email { get; set; }

        [DataMember(Order = 13)]
        public string VatNumber { get; set; }

        [DataMember(Order = 14)]
        public string ChamberOfCommerceNumber { get; set; }

        [DataMember(Order = 15, IsRequired = true)]
        public DateTime CreatedDate { get; set; }

        [DataMember(Order = 16)]
        public string CreatedBy { get; set; }

        [DataMember(Order = 17, IsRequired = true)]
        public DateTime LastModifiedDate { get; set; }

        [DataMember(Order = 18)]
        public string LastModifiedBy { get; set; }

        [DataMember(Order = 19)]
        public string Phone { get; set; }

        [DataMember(Order = 20)]
        public Guid? TenantId { get; set; }

        [DataMember(Order = 21)]
        public string SalesTaxCodeId { get; set; }

        [DataMember(Order = 22)]
        public string PurchaseTaxCodeId { get; set; }

        [DataMember(Order = 23)]
        public AddressDto Address { get; set; }

        [DataMember(Order = 24)]
        public PostOfficeBoxDto PostOfficeBox { get; set; }

        [DataMember(Order = 25)]
        public SupplierDto Supplier{ get; set; }
    }
}