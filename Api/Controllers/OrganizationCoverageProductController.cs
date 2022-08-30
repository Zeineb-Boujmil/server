using System.Linq;
using System.Web.Http;
using System.Web.OData;
using Api.Attributes;
using Api.Constants;
using Api.Controllers.Abstract;
using DataAccess;

namespace Api.Controllers
{
    public class OrganizationCoverageProductController : BaseController<OrganizationCoverageProduct>
    {
	    [HttpGet]
		//[Auth(AuthActionTypes.Read, AuthRoles.OrganizationUnit)]
		[EnableQuery(PageSize = 500, MaxExpansionDepth = 5)]
	    public override IQueryable<OrganizationCoverageProduct> Get()
	    {
		    return Context.Set<OrganizationCoverageProduct>();
	    }
	}
}
