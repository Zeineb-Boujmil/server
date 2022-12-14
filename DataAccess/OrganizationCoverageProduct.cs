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
    
    public partial class OrganizationCoverageProduct : Interfaces.IIdentifiable<Guid>, Interfaces.ITrackable
    {
        public System.Guid Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public string Name { get; set; }
        public string SpecificInformation { get; set; }
        public bool Inactive { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string PolisCheckTelephoneNr { get; set; }
        public string PolisCheckAlternateTelephone { get; set; }
        public string PolisCheckPortalWebsite { get; set; }
        public Nullable<bool> HasCustomCommunication { get; set; }
        public string DeviantBillingParty { get; set; }
        public System.Guid OrganizationUnit_Id { get; set; }
        public System.Guid CoverageProduct_Id { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    
        public virtual CoverageProduct CoverageProduct { get; set; }
        public virtual OrganizationUnit OrganizationUnit { get; set; }
        public virtual OrganizationCoverageProductsExtension OrganizationCoverageProductsExtension { get; set; }
    }
}
