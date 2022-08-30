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
    
    public partial class AuthorizationRole : Interfaces.IIdentifiable<String>, Interfaces.ITrackable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AuthorizationRole()
        {
            this.AuthorizationRoleInheritances = new HashSet<AuthorizationRoleInheritance>();
            this.EmployeeAuthorizations = new HashSet<EmployeeAuthorization>();
        }
    
        public string Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public string AuthorizationRoleTypeId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    
        public virtual AuthorizationGroupRole AuthorizationGroupRole { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AuthorizationRoleInheritance> AuthorizationRoleInheritances { get; set; }
        public virtual AuthorizationRoleType AuthorizationRoleType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeAuthorization> EmployeeAuthorizations { get; set; }
    }
}
