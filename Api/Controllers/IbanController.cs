using System;
using System.Threading.Tasks;
using System.Web.Http;
using Api.IbanService;

namespace Api.Controllers
{
    [RoutePrefix("api/Iban")]
    public class IbanController : ApiController
    {
        private readonly IbanServiceClient _ibanServiceClient;

        public IbanController()
        {
            _ibanServiceClient = new IbanServiceClient();
        }

        [HttpGet]
        [Route("ValidateIban/{iban}")]
        public async Task<IHttpActionResult> GetAddressByPostCode(string iban)
        {
            try
            {
                var response = await _ibanServiceClient.ValidateIbanAsync(new ValidateIbanRequest
                {
                    Iban = iban
                });
                return Ok(response);
            }
            catch (TimeoutException e)
            {
                throw new Exception("IBAN service timeout", e);
            }
            catch (Exception e)
            {
                throw new Exception("IBAN service error", e);
            }
        }
    }
}