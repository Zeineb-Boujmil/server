using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Api.Attributes;
using Api.Constants;
using DataAccess;

namespace Api.Controllers
{
	public class OrganizationCoverageProductsExtensionController : ApiController
    {
	    private readonly MasterDataContext _context;

	    public OrganizationCoverageProductsExtensionController()
	    {
			_context = new MasterDataContext();
	    }

		[HttpPost]
	    [Route("api/OrganizationCoverageProductsExtensions")]
	    //[Auth(AuthActionTypes.Create, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
	    public async Task<IHttpActionResult> CreateOrUpdateOrganizationCoverageProductsExtension(List<OrganizationCoverageProductsExtension> entities)
	    {
		    if (!ModelState.IsValid)
		    {
			    return BadRequest(ModelState);
		    }

		    foreach (var entity in entities)
		    {
				var currentEntity = _context.OrganizationCoverageProductsExtensions.SingleOrDefault(e => e.Id == entity.Id);

			    if (currentEntity == null)
				    _context.OrganizationCoverageProductsExtensions.Add(entity);
			    else
			    {
				    _context.Entry(currentEntity).CurrentValues.SetValues(entity);
			    }
			}
			
		    await _context.SaveChangesAsync();

		    return Ok();
	    }
	}
}
