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
    
    public partial class Model : Interfaces.IIdentifiable<Guid>, Interfaces.ITrackable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Model()
        {
            this.ModelProducts = new HashSet<ModelProduct>();
            this.ModelSpecifications = new HashSet<ModelSpecification>();
        }
    
        public System.Guid Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public string Code { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public System.Guid BrandId { get; set; }
        public Nullable<System.Guid> LocalizableEntryId { get; set; }
        public bool Inactive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public Nullable<System.Guid> InsuranceObjectId { get; set; }
    
        public virtual LocalizableEntry LocalizableEntry { get; set; }
        public virtual Brand Brand { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModelProduct> ModelProducts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModelSpecification> ModelSpecifications { get; set; }
        public virtual InsuranceObject InsuranceObject { get; set; }
    }
}
