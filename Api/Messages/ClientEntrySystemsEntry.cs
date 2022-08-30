using System;
using System.Collections.Generic;

namespace Api.Messages
{
    public class ClientEntrySystemsEntry
    {
        public Guid ClientId { get; set; }

        public List<string> EntrySystems { get; set; }
    }
}