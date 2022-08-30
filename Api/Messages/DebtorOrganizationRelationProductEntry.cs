using System;
using System.Collections.Generic;

namespace Api.Messages
{
    public class DebtorOrganizationRelationProductEntry
    {
        public Guid DebtorId { get; set; }

        public List<ClientProductsEntry> ClientProducts { get; set; }
    }
}