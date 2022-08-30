using DataAccess;
using System.Linq;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{
    public class RoadAuthorityTypeController : ODataController
    {
        protected readonly MasterDataContext Context = new MasterDataContext();

        [HttpGet]
        [EnableQuery(PageSize = 25)]
        public virtual IQueryable<RoadAuthorityType> Get()
        {
            return Context.Set<RoadAuthorityType>();
        }

        [HttpGet]
        [EnableQuery]
        public SingleResult<RoadAuthorityType> Get([FromODataUri] string key)
        {
            return SingleResult.Create(Context.Set<RoadAuthorityType>().Where(e => e.Id == key));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
