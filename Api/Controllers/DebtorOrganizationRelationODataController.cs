using Api.Attributes;
using Api.Constants;
using DataAccess;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace Api.Controllers
{
    public class DebtorOrganizationRelationODataController : ODataController
    {
        private readonly MasterDataContext _context;

        public DebtorOrganizationRelationODataController()
        {
            _context = new MasterDataContext();
        }

        [HttpGet]
        [ODataRoute("debtororganizationrelations")]
        //[Auth(AuthActionTypes.Read, AuthRoles.OrganizationUnit, AuthRoles.Finance)]                
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public IQueryable<DebtorOrganizationRelationsView> GetDebtorOrganizationRelations()
        {
            return _context.Set<DebtorOrganizationRelationsView>();
        }
    }
}