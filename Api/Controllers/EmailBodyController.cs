using DataAccess;
using System.Linq;
using System.Web.Http;

namespace Api.Controllers
{
    [Route("api/EmailBody")]
    public class EmailBodyController : ApiController
    {
        private readonly MasterDataContext _context;

        public EmailBodyController()
        {
            _context = new MasterDataContext();
            _context.Configuration.ProxyCreationEnabled = false;
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var response = _context.Settings.Where(e => e.Context == "EmailBody");
            return Ok(response);
        }
    }
}
