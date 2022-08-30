using Api.Attributes;
using Api.Constants;
using DataAccess;
using System.Linq;
using System.Web.Http;

namespace Api.Controllers
{
    public class DebtorAttributeController : ApiController
    {
        private readonly MasterDataContext _context;

        public DebtorAttributeController()
        {
            _context = new MasterDataContext();
        }

        [Route("api/debtorattributenames")]
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        public IHttpActionResult GetDebtorAttributeNames()
        {
            var names = _context.DebtorAttributes.Select(d => d.AttributeName).Distinct().ToList();
            return Ok(names);
        }
    }
}