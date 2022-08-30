using Api.Controllers.Abstract;
using DataAccess;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace Api.Controllers
{
    [ODataRoutePrefix("Template")]
    public class TemplatesController : BaseController<Template>
    {
        [HttpGet]
        [ODataRoute]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5, MaxNodeCount = 200)]
        public override IQueryable<Template> Get()
        {
            return Context.Set<Template>();
        }
    }
}
