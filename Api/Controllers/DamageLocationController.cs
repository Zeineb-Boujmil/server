using Api.Attributes;
using Api.Constants;
using Api.Controllers.Abstract;
using DataAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{
    public class DamageLocationController : BaseController<DamageLocation>
    {
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<DamageLocation> Get()
        {
            return Context.Set<DamageLocation>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<DamageLocation> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<DamageLocation>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(DamageLocation entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var insuranceObjectDamageLocation in entity.InsuranceObjectDamageLocations)
            {
                insuranceObjectDamageLocation.DamageLocationId = entity.Id;
                insuranceObjectDamageLocation.InsuranceObject = null;
                Context.Entry(insuranceObjectDamageLocation).State = EntityState.Added;
            }

            Context.Set<DamageLocation>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, DamageLocation entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CRUD(entity.InsuranceObjectDamageLocations,
                Context.InsuranceObjectDamageLocations.AsNoTracking().Where(e => e.DamageLocationId == entity.Id).ToList(),
                insuranceObjectDamageLocation =>
                {
                    insuranceObjectDamageLocation.DamageLocationId = entity.Id;
                    Context.Entry(insuranceObjectDamageLocation).State = EntityState.Added;
                    Context.Entry(insuranceObjectDamageLocation.InsuranceObject).State = EntityState.Detached;
                },
                insuranceObjectDamageLocation => { Context.Entry(insuranceObjectDamageLocation).State = EntityState.Modified; },
                insuranceObjectDamageLocation => { Context.Entry(insuranceObjectDamageLocation).State = EntityState.Deleted; });

            Context.Entry(entity).State = EntityState.Modified;

            await Context.SaveChangesAsync();

            return Updated(entity);
        }

        [HttpDelete]
        //[Auth(AuthActionTypes.Delete, AuthRoles.Administrator)]
        public async Task<IHttpActionResult> Delete([FromODataUri]Guid key)
        {
            var entity = new DamageLocation()
            {
                Id = key
            };

            Context.Set<DamageLocation>().Attach(entity);
            Context.Set<DamageLocation>().Remove(entity);
            await Context.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}