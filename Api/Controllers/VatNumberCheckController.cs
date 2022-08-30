using System;
using System.Threading.Tasks;
using System.Web.Http;
using Api.VatNumberCheckService;

namespace Api.Controllers
{
    [RoutePrefix("api/vatNumberCheck")]
    public class VatNumberCheckController : ApiController
    {
        private readonly WcfService_Ced_VatNumberCheckClient _vatNumberCheckServiceClient;

        public VatNumberCheckController()
        {
            _vatNumberCheckServiceClient = new WcfService_Ced_VatNumberCheckClient();
        }

        [HttpGet]
        [Route("CheckVatNumber/{countryCode}/{vatNumber}")]
        public async Task<IHttpActionResult> CheckVatNumber(string countryCode, string vatNumber)
        {
            try
            {
                var response = await _vatNumberCheckServiceClient.CheckVatNumberAsync(new CheckVatNumberRequest
                {
                    Iso2CountryCode = countryCode,
                    VatNumber = vatNumber
                });
                return Ok(response);
            }
            catch (TimeoutException e)
            {
                throw new Exception("VAT number check service timeout", e);
            }
            catch (Exception e)
            {
                throw new Exception("VAT number check service error", e);
            }
        }
    }
}