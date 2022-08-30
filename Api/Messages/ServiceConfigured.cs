using System;
using System.Runtime.Serialization;

namespace Api.Messages
{
    [DataContract(Namespace = "http://ced.com/Messaging/data")]
    public class ServiceConfigured
    {
        [DataMember(Order = 0)]
        public Guid ServiceId { get; set; }
    }
}