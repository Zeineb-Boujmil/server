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
    
    public partial class OrganizationAccountValidation : Interfaces.IIdentifiable<Guid>, Interfaces.ITrackable
    {
        public System.Guid Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public System.Guid OrganizationAccountId { get; set; }
        public Nullable<System.Guid> OrganizationAccountAttachmentId { get; set; }
        public Nullable<System.Guid> EmployeeId { get; set; }
        public System.DateTime StartDate { get; set; }
        public bool IsApproved { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual OrganizationAccountAttachment OrganizationAccountAttachment { get; set; }
        public virtual OrganizationAccount OrganizationAccount { get; set; }
    }
}