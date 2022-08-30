using Api.Attributes;
using Api.Constants;
using DataAccess;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{
    public class CurrencyController : ODataController
    {
        private readonly MasterDataContext _context = new MasterDataContext();

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25)]
        public virtual IQueryable<Currency> Get()
        {
            return _context.Currencies;
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery]
        public SingleResult<Currency> Get([FromODataUri] string key)
        {
            return SingleResult.Create(_context.Currencies.Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public virtual async Task<IHttpActionResult> Post(Currency entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Currencies.Add(entity);
            await _context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
       // [Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public virtual async Task<IHttpActionResult> Put([FromODataUri] string key, Currency entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Updated(entity);
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