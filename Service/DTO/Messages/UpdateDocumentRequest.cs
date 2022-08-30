using System.Runtime.Serialization;
using CED.Framework.Wcf;
using MasterDataService.DTO.Entities;

namespace MasterDataService.DTO.Messages
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class UpdateDocumentRequest : RequestBase
    {
        [DataMember(IsRequired = true, Order = 1)]
        public bool Success { get; set; }

        [DataMember(IsRequired = false, Order = 2)]
        public string Message { get; set; }

        [DataMember(IsRequired = false, Order = 3)]
        public DocumentUpdate DocumentUpdate { get; set; }

        public override void Validate() { }
    }
}