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
    
    public partial class LegalEntity : Interfaces.IIdentifiable<Guid>, Interfaces.ITrackable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LegalEntity()
        {
            this.BusinessUnits = new HashSet<BusinessUnit>();
            this.CostSettlements = new HashSet<CostSettlement>();
            this.Offices = new HashSet<Office>();
            this.CostCenters = new HashSet<CostCenter>();
            this.CostUnits = new HashSet<CostUnit>();
            this.Journals = new HashSet<Journal>();
            this.OrganizationTaxCodes = new HashSet<OrganizationTaxCode>();
            this.EmployeeAuthorizations = new HashSet<EmployeeAuthorization>();
            this.Creditors = new HashSet<Creditor>();
            this.HandlingCountries = new HashSet<HandlingCountry>();
            this.PaymentConditions = new HashSet<PaymentCondition>();
            this.SubContracts = new HashSet<SubContract>();
            this.Debtors = new HashSet<Debtor>();
            this.OrganizationPaymentMethods = new HashSet<OrganizationPaymentMethod>();
            this.SalesOrderApprovalSettings = new HashSet<SalesOrderApprovalSetting>();
            this.ClientTemplates = new HashSet<ClientTemplate>();
        }
    
        public System.Guid Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public System.Guid LegalEntityTypeId { get; set; }
        public Nullable<System.Guid> FiscalEntityId { get; set; }
        public string ExactAdministration { get; set; }
        public string CurrencyCode { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public string DocumentNamePrefix { get; set; }
        public string SalesJournalId { get; set; }
        public string PurchaseJournalId { get; set; }
        public string ConsolidationDebtorNumber { get; set; }
        public string ConsolidationCreditorNumber { get; set; }
        public string CreditSalesJournalId { get; set; }
        public string CreditPurchaseJournalId { get; set; }
        public string PurchaseMemorialJournalId { get; set; }
        public string PrepaymentSuspenseAccount { get; set; }
        public string EqualizationSuspenseAccount { get; set; }
        public Nullable<System.Guid> DefaultBankAccountId { get; set; }
        public Nullable<System.Guid> GeneralSupplierId { get; set; }
        public Nullable<System.Guid> DefaultPaymentConditionId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BusinessUnit> BusinessUnits { get; set; }
        public virtual FiscalEntity FiscalEntity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CostSettlement> CostSettlements { get; set; }
        public virtual LegalEntityType LegalEntityType { get; set; }
        public virtual OrganizationUnit OrganizationUnit { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Office> Offices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CostCenter> CostCenters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CostUnit> CostUnits { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Journal> Journals { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual Journal CreditPurchaseJournal { get; set; }
        public virtual Journal CreditSalesJournal { get; set; }
        public virtual Journal PurchaseJournal { get; set; }
        public virtual Journal SalesJournal { get; set; }
        public virtual Supplier GeneralSupplier { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrganizationTaxCode> OrganizationTaxCodes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeAuthorization> EmployeeAuthorizations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Creditor> Creditors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HandlingCountry> HandlingCountries { get; set; }
        public virtual PaymentCondition PaymentCondition { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentCondition> PaymentConditions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubContract> SubContracts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Debtor> Debtors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrganizationPaymentMethod> OrganizationPaymentMethods { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesOrderApprovalSetting> SalesOrderApprovalSettings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClientTemplate> ClientTemplates { get; set; }
    }
}
