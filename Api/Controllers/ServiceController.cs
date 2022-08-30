using Api.Attributes;
using Api.Constants;
using Api.Controllers.Abstract;
using DataAccess;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Api.Messages;
using CED.Framework.Logging;
using CED.Framework.Messaging.ServiceBusTopic;

namespace Api.Controllers
{
    public class ServiceController : BaseController<Service>
    {

        private static readonly string LoggerName = ConfigurationManager.AppSettings["LoggerName"];
        private readonly Logger _logger;

        public ServiceController()
        {
            _logger = Logger.GetLogger(LoggerName);
        }

        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<Service> Get()
        {
            return Context.Set<Service>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<Service> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<Service>().Where(e => e.Id == key));
        }

        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(Service entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            entity.Id = Guid.NewGuid();

            #region Applications

            foreach (var application in entity.ServiceApplications)
            {
                application.ServiceId = entity.Id;
                Context.ServiceApplications.Add(application);

                if (application.Application != null)
                {
                    Context.Entry(application.Application).State = EntityState.Detached;
                }
            }

            #endregion

            await UpdateProductClassification(entity);
            Context.Services.Add(entity);
            await Context.SaveChangesAsync();
            SendServiceConfiguredMessage(entity.Id);
            return Created(entity);
        }

        [Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, Service entity)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            #region Applications

            var oldApplications = await Context.ServiceApplications.AsNoTracking().Where(e => e.ServiceId == key).ToListAsync();
            var newApplications = entity.ServiceApplications;

            foreach (var application in newApplications.Where(n => n.Id == Guid.Empty).ToList())
            {
                application.ServiceId = key;
                Context.ServiceApplications.Add(application);

                if (application.Application != null)
                {
                    Context.Entry(application.Application).State = EntityState.Detached;
                }
            }

            foreach (var application in oldApplications.Where(o => newApplications.All(n => n.Id != o.Id)).ToList())
            {
                Context.ServiceApplications.Attach(application);
                Context.ServiceApplications.Remove(application);
            }

            #endregion

            await UpdateProductClassification(entity);
            Context.Entry(entity).State = EntityState.Modified;

            await Context.SaveChangesAsync();
            SendServiceConfiguredMessage(key);
            return Updated(entity);
        }

        private async Task UpdateProductClassification(Service entity)
        {
            if (entity.ProductId.HasValue)
            {
                var serviceProductClassification = await Context.ProductClassifications.FirstOrDefaultAsync(p => p.Code == "SERVICE");
                if (serviceProductClassification != null)
                {
                    var product = await Context.Products.FirstOrDefaultAsync(p => p.Id == entity.ProductId);
                    if (product != null && product.ProductClassification_Id != serviceProductClassification.Id)
                    {
                        product.ProductClassification_Id = serviceProductClassification.Id;
                    }
                }
            }
        }

        private void SendServiceConfiguredMessage(Guid serviceId)
        {
            var serviceBusTopicOptions = new ServiceBusTopicOptions("Service");
            var serviceConfigured = new ServiceConfigured
            {
                ServiceId = serviceId
            };

            try
            {
                var topicMessenger = new ServiceBusTopicMessenger();
                topicMessenger.SendMessage(serviceConfigured, serviceBusTopicOptions);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message, exception);
            }
        }
    }
}