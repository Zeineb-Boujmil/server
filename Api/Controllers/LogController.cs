using DataAccess;
using System.Linq;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{
    public class LogController : ODataController
    {
        private readonly LogContext _context = new LogContext();

        [HttpGet]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public IQueryable<Log> Get()
        {
            return _context.Logs;
        }

        [HttpGet]
        [EnableQuery]
        public SingleResult<Log> Get([FromODataUri] int key)
        {
            return SingleResult.Create(_context.Logs.Where(e => e.Id == key));
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
