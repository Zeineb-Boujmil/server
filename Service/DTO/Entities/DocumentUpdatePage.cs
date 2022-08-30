using System.Runtime.Serialization;

namespace MasterDataService.DTO.Entities
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class DocumentUpdatePage
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string FileName { get; set; }

        [DataMember(Order = 1, IsRequired = false)]
        public string DocumentType { get; set; }
    }
}