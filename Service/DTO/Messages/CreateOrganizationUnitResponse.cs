using System.Runtime.Serialization;
using CED.Framework.Wcf;

namespace MasterDataService.DTO.Messages
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class CreateOrganizationUnitResponse : ResponseBase
    {
    }
}