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
    
    public partial class RepairerSkill : Interfaces.IIdentifiable<Guid>, Interfaces.ITrackable
    {
        public System.Guid Id { get; set; }
        public byte[] TimeStamp { get; set; }
        public System.Guid RepairerId { get; set; }
        public System.Guid SkillId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    
        public virtual Repairer Repairer { get; set; }
        public virtual Skill Skill { get; set; }
    }
}