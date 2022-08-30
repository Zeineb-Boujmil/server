using System;
using System.Runtime.Serialization;

namespace MasterDataService.DTO
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class AddressDto
    {
        [DataMember(Order = 0, IsRequired = true)]
        public Guid Id { get; set; }

        [DataMember(Order = 1)]
        public string StreetName { get; set; }
        
        [DataMember(Order = 2)]
        public string HouseNo { get; set; }

        [DataMember(Order = 2)]
        public string HouseNoAddition { get; set; }

        [DataMember(Order = 3)]
        public string PostalCode { get; set; }

        [DataMember(Order = 4)]
        public string City { get; set; }

        [DataMember(Order = 5)]
        public string Province { get; set; }

        [DataMember(Order = 6, IsRequired = true)]
        public string CountryCode { get; set; }

        [DataMember(Order = 7,IsRequired = true)]
        public DateTime CreatedDate { get; set; }

        [DataMember(Order = 8)]
        public string CreatedBy { get; set; }

        [DataMember(Order = 9, IsRequired = true)]
        public DateTime LastModifiedDate { get; set; }

        [DataMember(Order = 10)]
        public string LastModifiedBy { get; set; }
    }
}