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
    
    public partial class InsuranceObjectType : Interfaces.IIdentifiable<Guid>, Interfaces.ITrackable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InsuranceObjectType()
        {
            this.InsuranceObjects = new HashSet<InsuranceObject>();
        }
    
        public System.Guid Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public string Code { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public bool Inactive { get; set; }
        public Nullable<System.Guid> LocalizableEntryId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    
        public virtual LocalizableEntry LocalizableEntry { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InsuranceObject> InsuranceObjects { get; set; }
    }
}