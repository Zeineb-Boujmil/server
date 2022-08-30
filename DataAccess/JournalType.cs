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
    
    public partial class JournalType : Interfaces.IIdentifiable<String>, Interfaces.ITrackable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public JournalType()
        {
            this.Journals = new HashSet<Journal>();
        }
    
        public string Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public Nullable<short> ExactType { get; set; }
        public bool Inactive { get; set; }
        public Nullable<System.Guid> LocalizableEntryId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Journal> Journals { get; set; }
        public virtual LocalizableEntry LocalizableEntry { get; set; }
    }
}
