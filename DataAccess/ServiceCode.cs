//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class ServiceCode : Interfaces.IIdentifiable<Guid>, Interfaces.ITrackable
    {
        public System.Guid Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public System.Guid ServiceId { get; set; }
        public System.Guid ServiceCodeTypeId { get; set; }
        public string Code { get; set; }
        public Nullable<System.DateTime> ValidFromDate { get; set; }
        public Nullable<System.DateTime> ValidUntilDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    
        public virtual ServiceCodeType ServiceCodeType { get; set; }
        public virtual Service Service { get; set; }
    }
}