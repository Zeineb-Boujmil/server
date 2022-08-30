using System.Linq;
using System.Web.Http;
using System.Web.OData;
using DataAccess;

namespace Api.Controllers
{
    public class TaxTypeController: ODataController
    {
        protected readonly MasterDataContext Context = new MasterDataContext();

        [HttpGet]
        [EnableQuery(PageSize = 25)]
        public virtual IQueryable<TaxType> Get()
        {
            return Context.Set<TaxType>();
        }

        [HttpGet]
        [EnableQuery]
        public SingleResult<TaxType> Get([FromODataUri] short key)
        {
            return SingleResult.Create(Context.Set<TaxType>().Where(e => e.Id == key));
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