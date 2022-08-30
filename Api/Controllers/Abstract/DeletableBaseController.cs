using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using DataAccess.Interfaces;

namespace Api.Controllers.Abstract
{
    public class DeletableBaseController<T> : BaseController<T>
        where T : class, IIdentifiable<Guid>, new()
    {
        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromODataUri]Guid key)
        {
            var entity = new T()
            {
                Id = key
            };

            Context.Set<T>().Attach(entity);
            Context.Set<T>().Remove(entity);
            await Context.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

    }
}