using DataAccess;
using System;

namespace Api.Messages
{
    public class DefaultTemplateEntry
    {
        public Guid? Id { get; set; }
        public string EmailBody { get; set; }
        public Department Department { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public Guid DefaultTemplate { get; set; }
    }
}