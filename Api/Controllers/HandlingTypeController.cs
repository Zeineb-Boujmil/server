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
    public class HandlingTypeController : ODataController
    {
        // TODO: Refactor BaseController so we can use it with different identifier types.

        private readonly MasterDataContext _context = new MasterDataContext();

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit)]
        [EnableQuery(PageSize = 25)]
        public virtual IQueryable<HandlingType> Get()
        {
            return _context.Set<HandlingType>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit)]
        [EnableQuery]
        public SingleResult<HandlingType> Get([FromODataUri] short key)
        {
            return SingleResult.Create(_context.Set<HandlingType>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.OrganizationUnit)]
        public virtual async Task<IHttpActionResult> Post(HandlingType entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = _context.HandlingTypes.OrderByDescending(e => e.Id).FirstOrDefault()?.Id ?? 0;
            entity.Id = (short) (id + 1);

            _context.Set<HandlingType>().Add(entity);
            await _context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPatch]
        //[Auth(AuthActionTypes.Update, AuthRoles.OrganizationUnit)]
        public virtual async Task<IHttpActionResult> Patch([FromODataUri] short key, Delta<HandlingType> delta)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _context.Set<HandlingType>().FindAsync(key);

            if (entity == null)
            {
                return NotFound();
            }

            delta.Patch(entity);

            await _context.SaveChangesAsync();

            return Updated(entity);
        }

        [HttpPut]
       // [Auth(AuthActionTypes.Update, AuthRoles.OrganizationUnit)]
        public virtual async Task<IHttpActionResult> Put([FromODataUri] short key, HandlingType entity)
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