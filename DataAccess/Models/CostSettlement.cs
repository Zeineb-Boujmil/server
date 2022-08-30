using System.Linq;

namespace DataAccess
{
    public partial class CostSettlement
    {
        public bool IsEqualTo(CostSettlement other)
        {
            if (Id != other.Id ||
                SettlementName != other.SettlementName ||
                LegalEntityId != other.LegalEntityId ||
                SupplierId != other.SupplierId)
                return false;

            if (CostSettlementLines.Count != other.CostSettlementLines.Count)
                return false;

            if (!CostSettlementLines
                    .ToList()
                    .All(first => other
                                    .CostSettlementLines
                                    .Any(second =>
                    {

                        return first.LineNumber == second.LineNumber &&
                               first.ServiceId == second.ServiceId &&
                               first.CostAgreement?.Id == second.CostAgreement?.Id;
                    })
                                                   ))
                return false;

            return true;
        }
    }
}