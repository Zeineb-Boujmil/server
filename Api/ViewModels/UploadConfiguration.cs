using System;
using System.Collections.Generic;

namespace Api.ViewModels
{
    public class UploadConfiguration
    {
        public Guid? TenantId { get; set; }
        public Guid OrganizationCodeTypeId { get; set; }
        public bool IsAgent { get; set; }
        public bool IsClient { get; set; }
        public bool IsMandate { get; set; }
        public bool IsRepairer { get; set; }
        public bool IsPartner { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsAlarmCenter { get; set; }
        public bool IsInternationalAssistanceGroup { get; set; }
        public bool IsInsurer { get; set; }
        public List<string> ServiceIds { get; set; }
        public List<string> OrganizationApplicationIds { get; set; }
    }
}