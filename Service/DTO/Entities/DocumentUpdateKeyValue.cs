using System.Runtime.Serialization;

namespace MasterDataService.DTO.Entities
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class DocumentUpdateKeyValue
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string Value { get; set; }
    }
}