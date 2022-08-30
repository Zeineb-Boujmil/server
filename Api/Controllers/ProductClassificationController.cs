using System;
using Api.Attributes;
using Api.Constants;
using DataAccess;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Api.Controllers.Abstract;

namespace Api.Controllers
{
    public class ProductClassificationController : BaseController<ProductClassification>
    {
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25)]
        public override IQueryable<ProductClassification> Get()
        {
            return Context.Set<ProductClassification>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery]
        public override SingleResult<ProductClassification> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.ProductClassifications.Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(ProductClassification entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Context.ProductClassifications.Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, ProductClassification entity)
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
