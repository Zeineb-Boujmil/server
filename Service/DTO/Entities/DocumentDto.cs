using System;
using System.Runtime.Serialization;

namespace MasterDataService.DTO.Entities
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class DocumentDto
    {
        [DataMember(Order = 0, IsRequired = true)]
        public Guid Id { get; set; }

        [DataMember(Order = 1)]
        public string DocumentNumber { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public DateTime DocumentDate { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public Guid DocumentTypeId { get; set; }

        [DataMember(Order = 4)]
        public string Description { get; set; }

        [DataMember(Order = 5, IsRequired = true)]
        public bool HasSignature { get; set; }

        [DataMember(Order = 6)]
        public DateTime? SignatureDate { get; set; }

        [DataMember(Order = 7)]
        public DateTime CreatedDate { get; set; }

        [DataMember(Order = 8)]
        public string CreatedBy { get; set; }

        [DataMember(Order = 9)]
        public DateTime LastModifiedDate { get; set; }
        
        [DataMember(Order = 10)]
        public string LastModifiedBy { get; set; }
    }
}