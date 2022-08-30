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
    public class TaxCodeController : ODataController
    {
       // TODO: Refactor BaseController so we can use it with different identifier types.

        protected readonly MasterDataContext Context = new MasterDataContext();

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25)]
        public virtual IQueryable<TaxCode> Get()
        {
            return Context.Set<TaxCode>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery]
        public SingleResult<TaxCode> Get([FromODataUri] string key)
        {
            return SingleResult.Create(Context.Set<TaxCode>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public virtual async Task<IHttpActionResult> Post(TaxCode entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Context.Set<TaxCode>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPatch]
        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public virtual async Task<IHttpActionResult> Patch([FromODataUri] string key, Delta<TaxCode> delta)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await Context.Set<TaxCode>().FindAsync(key);

            if (entity == null)
            {
                return NotFound();
            }

            delta.Patch(entity);

            await Context.SaveChangesAsync();

            return Updated(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator, AuthRoles.Finance)]
        public virtual async Task<IHttpActionResult> Put([FromODataUri] string key, TaxCode entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Context.Entry(entity).State = EntityState.Modified;

            await Context.SaveChangesAsync();

            return Updated(entity);
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