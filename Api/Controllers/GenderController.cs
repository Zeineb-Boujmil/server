using System.Linq;
using System.Web.OData;
using DataAccess;

namespace Api.Controllers
{
    public class GenderController : ODataController
    {
        private readonly MasterDataContext _context = new MasterDataContext();

        public IQueryable<Gender> Get()
        {
            return _context.Genders;
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