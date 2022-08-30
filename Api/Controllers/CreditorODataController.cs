using Api.Attributes;
using Api.Constants;
using Api.Controllers.Abstract;
using Api.Messages;
using Api.NumberingService;
using Api.ViewModels;
using CED.Framework.Logging;
using CED.Framework.Messaging.ServiceBusTopic;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace Api.Controllers
{
    [ODataRoutePrefix("Creditor")]
    public class CreditorODataController : BaseController<Creditor>
    {
        private readonly NumberingServiceClient _numberingServiceClient = new NumberingServiceClient();
        private static readonly string LoggerName = ConfigurationManager.AppSettings["LoggerName"];
        private readonly Logger _logger;

        public CreditorODataController()
        {
            _logger = Logger.GetLogger(LoggerName);
        }

        [HttpGet]
        [ODataRoute]
       // [Auth(AuthActionTypes.Read, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<Creditor> Get()
        {
            return Context.Set<Creditor>();
        }

        [HttpGet]
        [ODataRoute("({key})")]
        //[Auth(AuthActionTypes.Read, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        [EnableQuery(MaxExpansionDepth = 10)]
        public override SingleResult<Creditor> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<Creditor>().Where(e => e.Id == key));
        }

        [HttpGet]
        [ODataRoute("Services.FillCreditorAvailableSuppliersDropDown(legalEntityId={legalEntityId},vatNumber={vatNumber},filter={filter})")]
        public async Task<IEnumerable<DropDownViewModel>> FillCreditorAvailableSuppliersDropDown(Guid? legalEntityId, string vatNumber, string filter)
        {
            var query = Context.Suppliers
                    .Include(x => x.OrganizationUnit).AsQueryable();

            query = query.Where(x =>
                !x.OrganizationUnit.CreditorOrganizationRelations.Any(c => c.Creditor.LegalEntityId == legalEntityId));
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(o => o.OrganizationUnit.Code.Contains(filter) || o.OrganizationUnit.LongName.Contains(filter) || o.OrganizationUnit.LongName2.Contains(filter));
            }


            var result = await query.OrderBy(x => x.OrganizationUnit.LongName).Take(25).ToListAsync();

            return result
                .Select(obj => new DropDownViewModel()
                {
                    Value = obj.Id,
                    Text = obj.OrganizationUnit.Code + " - " + obj.OrganizationUnit.LongName + (obj.OrganizationUnit.LongName2?.Length > 0 ? " - " + obj.OrganizationUnit.LongName2 : "")
                });
        }

        [HttpPost]
        [ODataRoute]
        //[Auth(AuthActionTypes.Create, AuthRoles.Finance)]
        public override async Task<IHttpActionResult> Post(Creditor entity)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            entity.Id = Guid.NewGuid();

            if (entity.LegalEntityId == Guid.Empty)
                return BadRequest("Legal entity is required");

            var existingCreditors = Context.Creditors.Where(x =>
                    x.DefaultSupplierId == entity.DefaultSupplierId && x.LegalEntityId == entity.LegalEntityId)
                .AsNoTracking().ToList();

            if (existingCreditors.Any())
                return BadRequest(
                    $"Creditor with default supplier id ${entity.DefaultSupplierId} and legal entity id ${entity.LegalEntityId} already exists");

            if (entity.Number == null)
            {
                var organizationUnitCode = Context.OrganizationUnits.FirstOrDefault(x => x.Id == entity.LegalEntityId)?.Code;

                if (organizationUnitCode == null)
                {
                    return BadRequest("Legal entity doesn't exist");
                }
                entity.Number = await GenerateCreditorNumber(organizationUnitCode);
            }

            if (entity.Address != null)
            {
                entity.Address.Id = Guid.NewGuid();
                Context.Addresses.Add(entity.Address);
            }

            if (entity.PostOfficeBox != null)
            {
                entity.PostOfficeBox.Id = Guid.NewGuid();
                Context.PostOfficeBoxes.Add(entity.PostOfficeBox);
            }

            if (entity.CreditorOrganizationRelations != null)
                foreach (var creditorOrganizationRelation in entity.CreditorOrganizationRelations)
                {
                    creditorOrganizationRelation.CreditorId = entity.Id;

                    var organizationPaymentMethod = creditorOrganizationRelation.OrganizationUnit
                        .OrganizationPaymentMethods.FirstOrDefault();

                    if (organizationPaymentMethod != null)
                    {
                        if (organizationPaymentMethod.Id == Guid.Empty)
                        {
                            organizationPaymentMethod.Id = Guid.NewGuid();
                            organizationPaymentMethod.EffectiveDate = DateTime.Now;
                            Context.OrganizationPaymentMethods.Add(organizationPaymentMethod);
                        }
                        else
                            Context.Entry(organizationPaymentMethod).State = EntityState.Modified;
                    }

                    var organizationPaymentCondition = creditorOrganizationRelation.OrganizationUnit
                        .OrganizationPaymentConditions.FirstOrDefault();

                    if (organizationPaymentCondition != null)
                    {
                        if (organizationPaymentCondition.Id == Guid.Empty)
                        {
                            organizationPaymentCondition.Id = Guid.NewGuid();
                            organizationPaymentCondition.EffectiveDate = DateTime.Now;
                            Context.OrganizationPaymentConditions.Add(organizationPaymentCondition);
                        }
                        else
                            Context.Entry(organizationPaymentCondition).State = EntityState.Modified;
                    }

                    creditorOrganizationRelation.OrganizationUnit = null;
                    Context.CreditorOrganizationRelations.Add(creditorOrganizationRelation);
                    SendCreditorOrganizationRelationEntryMessage(entity.Id, creditorOrganizationRelation.OrganizationUnitId);
                }

            if (entity.CreditorAccounts != null)
                foreach (var creditorAccount in entity.CreditorAccounts)
                {
                    creditorAccount.CreditorId = entity.Id;
                    creditorAccount.BankAccount = null;
                    Context.CreditorAccounts.Add(creditorAccount);
                }

            Context.Creditors.Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        [ODataRoute("({key})")]
        //[Auth(AuthActionTypes.Update, AuthRoles.Finance)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, Creditor entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var creditor = Context.Creditors.AsNoTracking().FirstOrDefault(x => x.Id == key);

            if (creditor == null)
                return NotFound();

            if (entity.Address != null)
            {
                if (entity.Address.Id == Guid.Empty)
                {
                    entity.Address.Id = Guid.NewGuid();
                    Context.Addresses.Add(entity.Address);
                }
                else
                {
                    Context.Entry(entity.Address).State = EntityState.Modified;
                }
            }

            if (entity.PostOfficeBox != null)
            {
                if (entity.PostOfficeBox.Id == Guid.Empty)
                {
                    entity.PostOfficeBox.Id = Guid.NewGuid();
                    Context.PostOfficeBoxes.Add(entity.PostOfficeBox);
                }
                else
                {
                    Context.Entry(entity.PostOfficeBox).State = EntityState.Modified;
                }
            }

            // CreditorOrganizationRelations
            var oldCOR = Context.CreditorOrganizationRelations.Where(x => x.CreditorId == entity.Id).AsNoTracking().ToList();
            var newCOR = entity.CreditorOrganizationRelations;

            foreach (var dor in oldCOR.Where(old => newCOR.All(n => n.Id != old.Id)))
            {
                Context.CreditorOrganizationRelations.Attach(dor);
                Context.CreditorOrganizationRelations.Remove(dor);
            }

            CRUD(entity.CreditorOrganizationRelations,
                Context.CreditorOrganizationRelations.Where(x => x.CreditorId == entity.Id).AsNoTracking().ToList(),
                cor =>
                {
                    cor.CreditorId = entity.Id;
                    var organizationPaymentMethod = cor.OrganizationUnit.OrganizationPaymentMethods.FirstOrDefault();
                    if (organizationPaymentMethod != null && organizationPaymentMethod.Id == Guid.Empty)
                    {
                        organizationPaymentMethod.Id = Guid.NewGuid();
                        organizationPaymentMethod.EffectiveDate = DateTime.Now;
                        Context.OrganizationPaymentMethods.Add(organizationPaymentMethod);
                    }

                    var organizationPaymentCondition = cor.OrganizationUnit.OrganizationPaymentConditions.FirstOrDefault();
                    if (organizationPaymentCondition != null && organizationPaymentCondition.Id == Guid.Empty)
                    {
                        organizationPaymentCondition.Id = Guid.NewGuid();
                        organizationPaymentCondition.EffectiveDate = DateTime.Now;
                        Context.OrganizationPaymentConditions.Add(organizationPaymentCondition);
                    }

                    cor.OrganizationUnit = null;
                    Context.CreditorOrganizationRelations.Add(cor);
                    SendCreditorOrganizationRelationEntryMessage(entity.Id, cor.OrganizationUnitId);
                },
                cor =>
                {
                    var organizationPaymentMethod = cor.OrganizationUnit.OrganizationPaymentMethods.FirstOrDefault();
                    if (organizationPaymentMethod != null)
                    {
                        if (organizationPaymentMethod.Id == Guid.Empty)
                        {
                            organizationPaymentMethod.Id = Guid.NewGuid();
                            organizationPaymentMethod.EffectiveDate = DateTime.Now;
                            Context.OrganizationPaymentMethods.Add(organizationPaymentMethod);
                        }
                        else
                        {
                            Context.Entry(organizationPaymentMethod).State = EntityState.Modified;
                            Context.Entry(cor.OrganizationUnit).State = EntityState.Unchanged;
                            Context.Entry(cor).State = EntityState.Unchanged;
                        }
                    }

                    var organizationPaymentCondition = cor.OrganizationUnit.OrganizationPaymentConditions.FirstOrDefault();
                    if (organizationPaymentCondition != null)
                    {
                        if (organizationPaymentCondition.Id == Guid.Empty)
                        {
                            organizationPaymentCondition.Id = Guid.NewGuid();
                            organizationPaymentCondition.EffectiveDate = DateTime.Now;
                            Context.OrganizationPaymentConditions.Add(organizationPaymentCondition);
                        }
                        else
                        {
                            Context.Entry(organizationPaymentCondition).State = EntityState.Modified;
                            Context.Entry(cor.OrganizationUnit).State = EntityState.Unchanged;
                            Context.Entry(cor).State = EntityState.Unchanged;
                        }
                    }
                },
                cor => { });

            CRUD(entity.CreditorAccounts,
                Context.CreditorAccounts.Where(x => x.CreditorId == entity.Id).AsNoTracking().ToList(),
                ca =>
                {
                    ca.CreditorId = entity.Id;
                    ca.BankAccount = null;
                    Context.CreditorAccounts.Add(ca);
                },
                ca =>
                {
                    if (ca.BankAccount != null)
                        Context.Entry(ca.BankAccount).State = EntityState.Unchanged;
                    Context.Entry(ca).State = EntityState.Modified;
                },
                ca => { });

            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();

            return Updated(entity);
        }

        private async Task<string> GenerateCreditorNumber(string legalEntityCode)
        {
            try
            {
                var getNextNumberResponse = await _numberingServiceClient.GetNextNumberAsync(new GetNextNumberRequest
                {
                    DocumentDate = DateTime.Now,
                    DocumentName = $"{legalEntityCode}.Creditor"
                });

                if (getNextNumberResponse.ExceptionOccurred)
                {
                    throw new Exception(getNextNumberResponse.ExceptionMessage);
                }

                return getNextNumberResponse.NextNumber;
            }
            catch (TimeoutException e)
            {
                throw new Exception("Numbering service timeout", e);
            }
            catch (Exception e)
            {
                throw new Exception($"Numbering service error for legal entity with code ${legalEntityCode}", e);
            }
        }

        private void SendCreditorOrganizationRelationEntryMessage(Guid creditorId, Guid organizationUnitId)
        {
            var serviceBusTopicOptions = new ServiceBusTopicOptions("Creditor");
            var serviceConfigured = new CreditorOrganizationRelationEntry
            {
                CreditorId = creditorId,
                OrganizationUnitId = organizationUnitId
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