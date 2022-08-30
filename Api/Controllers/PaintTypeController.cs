using Api.Attributes;
using Api.Constants;
using Api.Controllers.Abstract;
using DataAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{
    public class PaintTypeController : BaseController<PaintType>
    {
        [HttpGet]
      //  [Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<PaintType> Get()
        {
            return Context.Set<PaintType>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<PaintType> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<PaintType>().Where(e => e.Id == key));
        }

        [HttpPost]
       // [Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(PaintType entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Context.Set<PaintType>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, PaintType entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Context.Entry(entity).State = EntityState.Modified;

            await Context.SaveChangesAsync();

            return Updated(entity);
        }
    }
}