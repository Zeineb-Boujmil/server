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
    
    public partial class Creditor : Interfaces.IIdentifiable<Guid>, Interfaces.ITrackable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Creditor()
        {
            this.CreditorOrganizationRelations = new HashSet<CreditorOrganizationRelation>();
            this.CreditorAccounts = new HashSet<CreditorAccount>();
        }
    
        public System.Guid Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public bool IsCompany { get; set; }
        public Nullable<short> GenderId { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string HomePage { get; set; }
        public Nullable<System.Guid> AddressId { get; set; }
        public Nullable<System.Guid> PostOfficeBoxId { get; set; }
        public System.Guid LegalEntityId { get; set; }
        public string LedgerAccount { get; set; }
        public string EntrySystem { get; set; }
        public string EntryNumber { get; set; }
        public string EntryBatch { get; set; }
        public string VatNumber { get; set; }
        public string ChamberOfCommerceNumber { get; set; }
        public Nullable<System.Guid> BankAccountId { get; set; }
        public Nullable<System.Guid> BlockedAccountId { get; set; }
        public string ExactPaymentMethod { get; set; }
        public string ExactAccountCategory { get; set; }
        public bool Inactive { get; set; }
        public Nullable<System.Guid> TenantId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public string PurchaseTaxCodeId { get; set; }
        public Nullable<System.DateTime> ExactExportDate { get; set; }
        public Nullable<System.Guid> DefaultSupplierId { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPhone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string SearchText { get; set; }
        public bool NoVatNumber { get; set; }
    
        public virtual LegalEntity LegalEntity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditorOrganizationRelation> CreditorOrganizationRelations { get; set; }
        public virtual Address Address { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual BankAccount BlockedBankAccount { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual PostOfficeBox PostOfficeBox { get; set; }
        public virtual Tenant Tenant { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditorAccount> CreditorAccounts { get; set; }
        public virtual Supplier DefaultSupplier { get; set; }
    }
}
