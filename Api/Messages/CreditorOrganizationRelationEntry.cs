using System;
using System.Runtime.Serialization;

namespace Api.Messages
{
    [DataContract(Namespace = "http://ced.com/Messaging/data")]
    public class CreditorOrganizationRelationEntry
    {
        [DataMember(Order = 0, IsRequired = true)]
        public Guid CreditorId { get; set; }

        [DataMember(Order = 1, IsRequired = true)]
        public Guid OrganizationUnitId { get; set; }
    }
}