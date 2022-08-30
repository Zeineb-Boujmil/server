using System.Runtime.Serialization;
using CED.Framework.Wcf;
using MasterDataService.DTO.Entities;

namespace MasterDataService.DTO.Messages
{
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class CreateOrganizationUnitRequest : RequestBase
    {
        [DataMember(IsRequired = true, Order = 0)]
        public OrganizationUnitDto OrganizationUnit { get; set; }

        public override void Validate()
        {
            
        }
    }
}