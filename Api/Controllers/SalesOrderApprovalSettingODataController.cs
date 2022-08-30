using Api.Attributes;
using Api.Constants;
using DataAccess;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace Api.Controllers
{
    public class SalesOrderApprovalSettingODataController : ODataController
    {
        private readonly MasterDataContext _context;

        public SalesOrderApprovalSettingODataController()
        {
            _context = new MasterDataContext();
        }

        [HttpGet]
        [ODataRoute("salesorderapprovalsettings")]
       // [Auth(AuthActionTypes.Read, AuthRoles.OrganizationUnit, AuthRoles.Finance)]                
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public IQueryable<SalesOrderApprovalSettingsView> GetSalesOrderApprovalSettings()
        {
            return _context.Set<SalesOrderApprovalSettingsView>();
        }
    }
}