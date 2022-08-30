using System;

namespace Api.Messages
{
    public class DefaultTemplateViewModel
    {
        public Guid Id { get; set; }
        public string EmailBody { get; set; }
        public Guid Department { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentLongName { get; set; }
        public string DepartmentLongName2 { get; set; }

        public string DepartmentCode { get; set; }

        public Guid BusinessUnit { get; set; }

        public string BusinessUnitName { get; set; }
        public string BusinessUnitLongName { get; set; }
        public string BusinessUnitLongName2 { get; set; }

        public string BusinessUnitCode { get; set; }


        public Guid LegalEntity { get; set; }
        public string LegalEntityName { get; set; }

        public string LegalEntityLongName { get; set; }
        public string LegalEntityLongName2 { get; set; }

        public string LegalEntityCode { get; set; }


        public DateTime? EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public Guid DefaultTemplate { get; set; }
        public string DefaultTemplateName { get; set; }





    }
}