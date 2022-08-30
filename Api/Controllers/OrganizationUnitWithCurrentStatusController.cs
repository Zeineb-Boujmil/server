using DataAccess;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using Api.Attributes;
using Api.Constants;

namespace Api.Controllers
{
    public class OrganizationUnitWithCurrentStatusController : ODataController
    {
        private readonly MasterDataContext _context = new MasterDataContext();
        
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public IQueryable<OrganizationUnitWithCurrentStatus> Get()
        {
            return _context.Set<OrganizationUnitWithCurrentStatus>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}