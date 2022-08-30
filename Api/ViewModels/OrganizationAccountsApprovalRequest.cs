using System;
using System.Collections.Generic;

namespace Api.ViewModels
{
	public class OrganizationAccountsApprovalRequest
	{
		public bool IsApproved { get; set; }

		public List<Guid> OrganizationAccountIds { get; set; }
	}
}