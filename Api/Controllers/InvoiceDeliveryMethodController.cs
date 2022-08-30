using System.Linq;
using System.Web.Http;
using System.Web.OData;
using Api.Attributes;
using Api.Constants;
using DataAccess;

namespace Api.Controllers
{
    public class InvoiceDeliveryMethodController : ODataController
    {
        private readonly MasterDataContext _context = new MasterDataContext();

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.PurchaseInvoiceAccess, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25)]
        public virtual IQueryable<InvoiceDeliveryMethod> Get()
        {
            return _context.InvoiceDeliveryMethods;
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.PurchaseInvoiceAccess, AuthRoles.Finance)]
        [EnableQuery]
        public SingleResult<InvoiceDeliveryMethod> Get([FromODataUri] string key)
        {
            return SingleResult.Create(_context.InvoiceDeliveryMethods.Where(e => e.Id == key));
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