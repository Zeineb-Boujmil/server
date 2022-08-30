using System.Runtime.Serialization;

namespace MasterDataService.DTO.Entities
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class DocumentUpdateKey
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string Key { get; set; }

        [DataMember(Order = 1, IsRequired = true)]
        public DocumentUpdateKeyValue[] Values { get; set; }
    }
}