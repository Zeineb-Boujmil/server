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
    
    public partial class Office : Interfaces.IIdentifiable<Guid>, Interfaces.ITrackable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Office()
        {
            this.DepartmentOffices = new HashSet<DepartmentOffice>();
        }
    
        public System.Guid Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public string Name { get; set; }
        public System.Guid LegalEntityId { get; set; }
        public Nullable<System.Guid> AddressId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DepartmentOffice> DepartmentOffices { get; set; }
        public virtual LegalEntity LegalEntity { get; set; }
        public virtual Address Address { get; set; }
    }
}