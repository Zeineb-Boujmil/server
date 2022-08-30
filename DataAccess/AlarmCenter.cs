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
    
    public partial class AlarmCenter : Interfaces.IIdentifiable<Guid>, Interfaces.ITrackable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AlarmCenter()
        {
            this.InsurerAlarmCenters = new HashSet<InsurerAlarmCenter>();
            this.MandateAlarmCenters = new HashSet<MandateAlarmCenter>();
            this.LeasingCompanyAlarmCenters = new HashSet<LeasingCompanyAlarmCenter>();
        }
    
        public System.Guid Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public string ExternalCode { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public string CultureCodePreferredLanguage { get; set; }
        public Nullable<bool> Inactive { get; set; }
    
        public virtual OrganizationUnit OrganizationUnit { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InsurerAlarmCenter> InsurerAlarmCenters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MandateAlarmCenter> MandateAlarmCenters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LeasingCompanyAlarmCenter> LeasingCompanyAlarmCenters { get; set; }
    }
}