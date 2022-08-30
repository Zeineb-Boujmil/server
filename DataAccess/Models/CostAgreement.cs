namespace DataAccess
{
    public partial class CostAgreement
    {
        public bool IsEqualTo(CostAgreement other)
        {
            if (other == null || AgreementName != other.AgreementName ||
                CostAgreementTypeId != other.CostAgreementTypeId ||
                CurrencyCode != other.CurrencyCode ||
                EffectiveDate != other.EffectiveDate ||
                TerminationDate != other.TerminationDate ||
                IncludingTax != other.IncludingTax ||
                FixedCostAgreement?.FixedCostAmount != other.FixedCostAgreement?.FixedCostAmount
               ) return false;

            return true;
        }

    }
}