using System;
using System.Runtime.Serialization;

namespace MasterDataService.DTO.Entities
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class DocumentUpdate
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string Id { get; set; }

        [DataMember(Order = 1, IsRequired = true)]
        public string LocalReference { get; set; }

        [DataMember(Order = 2, IsRequired = false)]
        public DateTime? IndexedDate { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public DocumentUpdateKey[] Keys { get; set; }

        [DataMember(Order = 4, IsRequired = false)]
        public DocumentUpdatePage[] Pages { get; set; }
    }
}