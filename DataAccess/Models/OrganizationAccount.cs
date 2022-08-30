namespace DataAccess
{
    public partial class OrganizationAccount
    {
        public bool IsEqualTo(OrganizationAccount other)
        {
            if (other == null || AccountCode != other.AccountCode ||
                AccountName != other.AccountName ||
                OrganizationUnitId != other.OrganizationUnitId ||
                IsApproved != other.IsApproved
            ) return false;

            return true;
        }
    }
}
