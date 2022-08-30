using System;
using System.Collections.Generic;

namespace Api.Messages
{
    public class ClientProductsEntry
    {
        public Guid ClientId { get; set; }

        public List<Guid> ProductIds { get; set; }
    }
}