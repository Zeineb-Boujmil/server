using Api.Attributes;
using Api.Constants;
using Api.Controllers.Abstract;
using Api.NumberingService;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Api.ViewModels;

namespace Api.Controllers
{
    [ODataRoutePrefix("Debtor")]
    public class DebtorODataController : BaseController<Debtor>
    {
        private readonly NumberingServiceClient _numberingServiceClient = new NumberingServiceClient();

        [HttpGet]
        [ODataRoute]
        //[Auth(AuthActionTypes.Read, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<Debtor> Get()
        {
            return Context.Set<Debtor>();
        }

        [HttpGet]
        [ODataRoute("({key})")]
        //[Auth(AuthActionTypes.Read, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        [EnableQuery(MaxExpansionDepth = 10)]
        public override SingleResult<Debtor> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<Debtor>().Where(e => e.Id == key));
        }

        [HttpGet]
        [ODataRoute("Services.FillDebtorAvailableClientsDropDown(legalEntityId={legalEntityId}, vatNumber={vatNumber},filter={filter})")]
        public async Task<IEnumerable<DropDownViewModel>> FillDebtorAvailableClientsDropDown(Guid? legalEntityId, string vatNumber, string filter)
        {
            var vatNumbersList = new List<string> { null };

            if (!string.IsNullOrWhiteSpace(vatNumber))
                vatNumbersList.Add(vatNumber);            

            var query = vatNumbersList.Count == 2
                ? Context.Clients
                    .Include(x => x.OrganizationUnit)
                    .Where(x => x.Inactive != true && x.OrganizationUnit.Inactive != true && vatNumbersList.Contains(x.OrganizationUnit.VatNumber)).AsQueryable()
                : Context.Clients
                    .Include(x => x.OrganizationUnit)
                    .Where(x => x.Inactive != true && x.OrganizationUnit.Inactive != true)
                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(o => o.OrganizationUnit.Code.Contains(filter) || o.OrganizationUnit.LongName.Contains(filter) || o.OrganizationUnit.LongName2.Contains(filter));
            }

            var result = await query.OrderBy(x => x.OrganizationUnit.LongName).ToListAsync();

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
        public override async Task<IHttpActionResult> Post(Debtor entity)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            entity.Id = Guid.NewGuid();

            if (entity.LegalEntityId == Guid.Empty)
                return BadRequest("Legal entity is required");

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

            if (entity.DebtorOrganizationRelations != null)
                foreach (var debtorOrganizationRelation in entity.DebtorOrganizationRelations)
                {
                    debtorOrganizationRelation.DebtorId = entity.Id;

                    var organizationPaymentMethod = debtorOrganizationRelation.OrganizationUnit
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

                    var organizationPaymentCondition = debtorOrganizationRelation.OrganizationUnit
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

                    debtorOrganizationRelation.OrganizationUnit = null;
                    Context.DebtorOrganizationRelations.Add(debtorOrganizationRelation);
                }

            if (entity.DebtorAccounts != null)
                foreach (var debtorAccount in entity.DebtorAccounts)
                {
                    debtorAccount.DebtorId = entity.Id;
                    debtorAccount.BankAccount = null;
                    Context.DebtorAccounts.Add(debtorAccount);
                }

            if (entity.DebtorAttributes != null)
            {
                foreach (var debtorAttribute in entity.DebtorAttributes)
                {
                    debtorAttribute.DebtorId = entity.Id;
                    Context.DebtorAttributes.Add(debtorAttribute);
                }
            }

            Context.Debtors.Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        [ODataRoute("({key})")]
        [AllowAnonymous]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, Debtor entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var debtor = Context.Debtors.AsNoTracking().FirstOrDefault(x => x.Id == key);

            if (debtor == null)
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

            // DebtorOrganizationRelations
            if(entity.Inactive)
            {
                entity.DebtorOrganizationRelations.Clear();
            }
            var oldDOR = Context.DebtorOrganizationRelations.Where(x => x.DebtorId == entity.Id).AsNoTracking().ToList();
            var newDOR = entity.DebtorOrganizationRelations;

            foreach (var dor in oldDOR.Where(old => newDOR.All(n => n.OrganizationUnitId != old.OrganizationUnitId)))
            {
                Context.DebtorOrganizationRelations.Attach(dor);
                Context.DebtorOrganizationRelations.Remove(dor);
            }

            var remainingOldDOR = Context.DebtorOrganizationRelations.Where(x => x.DebtorId == entity.Id).AsNoTracking().ToList();
            var remainingNewDOR = newDOR.Where(n => remainingOldDOR.All(r => r.OrganizationUnitId != n.OrganizationUnitId)).ToList();

            var organizationPaymentConditions = remainingOldDOR.SelectMany(d => d.OrganizationUnit.OrganizationPaymentConditions).ToList();
            organizationPaymentConditions.AddRange(remainingNewDOR.SelectMany(d => d.OrganizationUnit.OrganizationPaymentConditions));
            organizationPaymentConditions.ForEach(o =>
            {
                o.OrganizationUnit = null;
            });
            var newOrganizationPaymentConditions = organizationPaymentConditions.Where(o => o.Id == Guid.Empty);
            if (newOrganizationPaymentConditions.Any())
            {
                var uniqueNewOrganizationPaymentConditions = newOrganizationPaymentConditions.GroupBy(o => o.OrganizationUnitId)
                                                                                .First().ToList();
                uniqueNewOrganizationPaymentConditions.ForEach(o =>
                {
                    o.Id = Guid.NewGuid();
                    o.EffectiveDate = DateTime.Now;
                    Context.OrganizationPaymentConditions.Add(o);
                });
            }
            remainingOldDOR.ForEach(dor =>
            {
                dor.OrganizationUnit = null;
            });

            newDOR.ToList().ForEach(dor =>
            {
                dor.OrganizationUnit = null;
            });

            CRUD(remainingNewDOR,
                remainingOldDOR,
                dor =>
                {
                    dor.DebtorId = entity.Id;
                    dor.OrganizationUnit = null;
                    Context.DebtorOrganizationRelations.Add(dor);
                },
                dor => { },
                dor => { });

            CRUD(entity.DebtorAccounts,
                Context.DebtorAccounts.Where(x => x.DebtorId == entity.Id).AsNoTracking().ToList(),
                da =>
                {
                    da.DebtorId = entity.Id;
                    da.BankAccount = null;
                    Context.DebtorAccounts.Add(da);
                },
                da =>
                {
                    if (da.BankAccount != null)
                        Context.Entry(da.BankAccount).State = EntityState.Unchanged;
                    Context.Entry(da).State = EntityState.Modified;
                },
                da => { });

            CRUD(entity.DebtorAttributes,
                Context.DebtorAttributes.Where(x => x.DebtorId == entity.Id).AsNoTracking().ToList(),
                da =>
                {
                    da.DebtorId = entity.Id;
                    Context.DebtorAttributes.Add(da);
                },
                da =>
                {
                    Context.Entry(da).State = EntityState.Modified;
                },
                da =>
                {
                    Context.Entry(da).State = EntityState.Deleted;
                });

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
                    DocumentName = $"{legalEntityCode}.Debtor"
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
    }
}
