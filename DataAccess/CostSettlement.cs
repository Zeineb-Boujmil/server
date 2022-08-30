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
    
    public partial class CostSettlement : Interfaces.IIdentifiable<Guid>, Interfaces.ITrackable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CostSettlement()
        {
            this.CostSettlementLines = new HashSet<CostSettlementLine>();
        }
    
        public System.Guid Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public string SettlementName { get; set; }
        public Nullable<System.Guid> LegalEntityId { get; set; }
        public Nullable<System.Guid> SupplierId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public string ContractNumber { get; set; }
        public string DiscountCode { get; set; }
        public string EntrySystem { get; set; }
    
        public virtual LegalEntity LegalEntity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CostSettlementLine> CostSettlementLines { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}