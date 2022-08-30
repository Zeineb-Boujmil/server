using Newtonsoft.Json;
using VatValidatorJob.VatNumberCheckService;

namespace VatValidatorJob
{
    public class WcfServiceCedVatNumberCheckClient
    {
        public virtual VatNumberCheckResponse CheckVatNumber(CheckVatNumberRequest checkVatNumberRequest)
        {
            return new WcfService_Ced_VatNumberCheckClient().CheckVatNumber(checkVatNumberRequest);
        }
    }

    public class WcfServiceCedVatNumberCheckClientMock : WcfServiceCedVatNumberCheckClient
    {
        public override VatNumberCheckResponse CheckVatNumber(CheckVatNumberRequest checkVatNumberRequest)
        {
            return JsonConvert.DeserializeObject<VatNumberCheckResponse>(@"{""Success"":true,""Message"":null,""Result"":{""CountryCode"":""NL"",""VatNumber"":""1231123"",""RequestDate"":""2018-04-09T00:00:00+02:00"",""IsValid"":false,""CompanyName"":""---"",""FullAddress"":""---""}}");
        }
    }
}
