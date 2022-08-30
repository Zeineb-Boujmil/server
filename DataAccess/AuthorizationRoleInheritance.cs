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
    
    public partial class AuthorizationRoleInheritance : Interfaces.IIdentifiable<Guid>, Interfaces.ITrackable
    {
        public System.Guid Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public string AuthorizationGroupRoleId { get; set; }
        public string AuthorizationInheritsRoleId { get; set; }
        public bool OperationCreate { get; set; }
        public bool OperationRead { get; set; }
        public bool OperationUpdate { get; set; }
        public bool OperationDelete { get; set; }
        public bool OperationExecute { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public Nullable<System.DateTime> TerminationDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    
        public virtual AuthorizationGroupRole AuthorizationGroupRole { get; set; }
        public virtual AuthorizationRole AuthorizationRole { get; set; }
    }
}
