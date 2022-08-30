using System;
using System.Runtime.Serialization;

namespace MasterDataService.DTO
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class PostOfficeBoxDto
    {
        [DataMember(Order = 1, IsRequired = true)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string BoxNo { get; set; }

        [DataMember(Order = 3)]
        public string PostalCode { get; set; }

        [DataMember(Order = 4)]
        public string City { get; set; }

        [DataMember(Order = 5)]
        public string Province { get; set; }

        [DataMember(Order = 6)]
        public string CountryCode { get; set; }

        [DataMember(Order = 7, IsRequired = true)]
        public DateTime CreatedDate { get; set; }

        [DataMember(Order = 8)]
        public string CreatedBy { get; set; }

        [DataMember(Order = 9,IsRequired = true)]
        public DateTime LastModifiedDate { get; set; }

        [DataMember(Order = 10)]
        public string LastModifiedBy { get; set; }
    }
}