using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using DataAccess;
using DataAccess.Interfaces;
using System.Collections.Generic;

namespace Api.Controllers.Abstract
{
    public abstract class BaseController<T> : ODataController
        where T : class, IIdentifiable<Guid>
    {
        protected readonly MasterDataContext Context = new MasterDataContext();

        [HttpGet]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public virtual IQueryable<T> Get()
        {
            return Context.Set<T>();
        }

        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 5)]
        public virtual SingleResult<T> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<T>().Where(e => e.Id == key));
        }

        [HttpPost]
        public virtual async Task<IHttpActionResult> Post(T entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Context.Set<T>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPatch]
        public virtual async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<T> delta)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await Context.Set<T>().FindAsync(key);

            if (entity == null)
            {
                return NotFound();
            }

            delta.Patch(entity);

            await Context.SaveChangesAsync();

            return Updated(entity);
        }

        [HttpPut]
        public virtual async Task<IHttpActionResult> Put([FromODataUri] Guid key, T entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Context.Entry(entity).State = EntityState.Modified;

            await Context.SaveChangesAsync();

            return Updated(entity);
        }
        protected static void CRUD<T>(IEnumerable<T> newEntities, IEnumerable<T> oldEntities, Action<T> create, Action<T> update, Action<T> delete)
            where T : IIdentifiable<Guid>
        {
            newEntities = newEntities ?? new List<T>();
            oldEntities = oldEntities ?? new List<T>();

            newEntities
                .Where(e => e.Id == Guid.Empty)
                .ToList()
                .ForEach(create);
            newEntities
                .Where(n => oldEntities.Any(o => o.Id == n.Id))
                .ToList()
                .ForEach(update);
            oldEntities
                .Where(n => newEntities.All(o => o.Id != n.Id))
                .ToList()
                .ForEach(delete);
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