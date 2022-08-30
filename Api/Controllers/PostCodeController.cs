using System;
using System.Threading.Tasks;
using System.Web.Http;
using Api.PostcodeCheckService;

namespace Api.Controllers
{
    [RoutePrefix("api/PostCode")]
    public class PostCodeController : ApiController
    {
        private readonly PostcodeCheckServiceClient _postCodeCheckServiceClient;

        public PostCodeController()
        {
            _postCodeCheckServiceClient = new PostcodeCheckServiceClient();
        }

        [HttpGet]
        [Route("GetAddressByPostCode/{postCode}/{houseNumber}")]
        public async Task<IHttpActionResult> GetAddressByPostCode(string postCode, int houseNumber)
        {
            try
            {
                var response = await _postCodeCheckServiceClient.GetAddressByPostcodeAsync(new GetAddressByPostcodeRequest
                {
                    Housenumber = houseNumber,
                    Postcode = postCode
                });
                return Ok(response);
            }
            catch (TimeoutException e)
            {
                throw new Exception("Post code service timeout", e);
            }
            catch (Exception e)
            {
                throw new Exception("Post code service error", e);
            }
        }

        [HttpGet]
        [Route("GetPostCodeByAddress/{city}/{street}/{houseNumber}")]
        public async Task<IHttpActionResult> GetPostCodeByAddress(string city, string street, int houseNumber)
        {
            try
            {
                var response = await _postCodeCheckServiceClient.GetPostcodeByAddressAsync(new GetPostcodeByAddressRequest
                {
                    City = city,
                    Street = street,
                    Housenumber = houseNumber
                });
                return Ok(response);
            }
            catch (TimeoutException e)
            {
                throw new Exception("Post code service timeout", e);
            }
            catch (Exception e)
            {
                throw new Exception("Post code service error", e);
            }
        }
    }
}
