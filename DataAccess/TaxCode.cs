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
    
    public partial class TaxCode : Interfaces.IIdentifiable<String>, Interfaces.ITrackable
    {
        public string Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public string Description { get; set; }
        public short TaxTypeId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    
        public virtual TaxType TaxType { get; set; }
    }
}
