using System;
using System.Collections.Generic;

namespace Api.Messages
{
    public class SalesOrderApprovalSettingEntry
    {
        public Guid LegalEntityId { get; set; }

        public Guid? DebtorId { get; set; }

        public bool AutoAuthorizeFeeOrders { get; set; }

        public bool AutoAuthorizeChargedExpensesOrders { get; set; }

        public List<ClientEntrySystemsEntry> ClientEntrySystems { get; set; }
    }
}