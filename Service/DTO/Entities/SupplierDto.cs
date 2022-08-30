using System;
using System.Runtime.Serialization;

namespace MasterDataService.DTO
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class SupplierDto
    {
        [DataMember(Order = 0)]
        public string ExternalCode { get; set; }

        [DataMember(Order = 1)]
        public Guid? PartnerId { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public DateTime CreatedDate { get; set; }

        [DataMember(Order = 3)]
        public string CreatedBy { get; set; }

        [DataMember(Order = 4, IsRequired = true)]
        public DateTime LastModifiedDate { get; set; }

        [DataMember(Order = 5)]
        public string LastModifiedBy { get; set; }
    }
}