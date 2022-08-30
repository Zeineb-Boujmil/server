using Api.Attributes;
using Api.Constants;
using Api.Controllers.Abstract;
using Api.NumberingService;
using Api.ViewModels;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Api.Extensions;

namespace Api.Controllers
{
    [ODataRoutePrefix("OrganizationUnit")]
    public class OrganizationUnitODataController : BaseController<OrganizationUnit>
    {
        private readonly NumberingServiceClient _numberingServiceClient = new NumberingServiceClient();

        [HttpGet]
        [ODataRoute]
        //[Auth(AuthActionTypes.Read, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5, MaxNodeCount = 200)]
        public override IQueryable<OrganizationUnit> Get()
        {
            return Context.Set<OrganizationUnit>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [ODataRoute("/organizationUnitWithInvalidVat")]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public IQueryable<OrganizationUnit> InvalidVat()
        {
            return Context
                .OrganizationUnits
                .Where(o => o.OrganizationVatValidations.FirstOrDefault() != null && !o.OrganizationVatValidations.OrderByDescending(x => x.CreatedDate).FirstOrDefault().IsValid).AsQueryable();
        }

        [HttpPost]
        [ODataRoute("Services.SaveAll")]
        //[Auth(AuthActionTypes.Create, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        public async Task<IHttpActionResult> SaveAll(ODataActionParameters parameters)
        {

            if (!parameters.TryGetValue("organizationUnits", out var value))
                return NotFound();

            var organizationUnits = (IEnumerable<OrganizationUnit>)value;

            if (organizationUnits == null)
                return BadRequest();

            foreach (var entity in organizationUnits)
            {
                if (entity.Id == Guid.Empty)
                    await CreateOne(entity);
                else
                    await UpdateOne(entity.Id, entity);
            }

            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [ODataRoute("({key})")]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(MaxExpansionDepth = 8)]
        public override SingleResult<OrganizationUnit> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<OrganizationUnit>().Where(e => e.Id == key));
        }

        [HttpPost]
        [ODataRoute]
        //[Auth(AuthActionTypes.Create, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        public override async Task<IHttpActionResult> Post(OrganizationUnit entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await CreateOne(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        [ODataRoute("({key})")]
        //[Auth(AuthActionTypes.Update, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, OrganizationUnit entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key == Guid.Empty || key != entity.Id || !Context.OrganizationUnits.Any(e => e.Id == key && e.Code == entity.Code))
            {
                return BadRequest("Invalid key or code");
            }

            await UpdateOne(key, entity);
            await Context.SaveChangesAsync();
            return Updated(entity);
        }

        private async Task<OrganizationUnit> CreateOne(OrganizationUnit entity)
        {

            entity.Id = Guid.NewGuid();
            if (entity.Code.IsNullOrEmpty())
            {
                entity.Code = await GenerateOrganizationUnitCode();
            }

            // Always set status to approved.
            entity.ValidationStatusHistories = new List<OrganizationUnitValidationStatusHistory>() {
                new OrganizationUnitValidationStatusHistory()
                {
                    Id = Guid.NewGuid(),
                    OrganizationUnitId = entity.Id,
                    ValidationStatusId = new Guid("433C02C7-35BE-4493-B236-B0D9C421DD17"),
                }
            };

            if (entity.OrganizationAddresses != null && entity.OrganizationAddresses.Count > 0)
            {
                var organizationAddresses = entity.OrganizationAddresses.ToList();
                foreach (var organizationAddress in organizationAddresses)
                {
                    if (organizationAddress.Address != null)
                    {
                        organizationAddress.Address.Id = new Guid();
                    }
                    if (organizationAddress.PostOfficeBox != null)
                    {
                        organizationAddress.PostOfficeBox.Id = new Guid();
                    }
                    if (organizationAddress.AddressTypeId == Guid.Empty)
                    {
                        organizationAddress.AddressType = Context.AddressTypes.FirstOrDefault(at => at.Code == "PA");
                    }
                    Context.OrganizationAddresses.Add(organizationAddress);
                }

                var firstAddress = organizationAddresses.FirstOrDefault(oa => oa.Address != null)?.Address;
                if (firstAddress != null)
                {
                    entity.Address = firstAddress;
                    entity.AddressId = firstAddress.Id;
                }

                var firstPostOfficeBox = organizationAddresses.FirstOrDefault(oa => oa.PostOfficeBox != null)?.PostOfficeBox;
                if (firstPostOfficeBox != null)
                {
                    entity.PostOfficeBox = firstPostOfficeBox;
                    entity.PostOfficeBoxId = firstPostOfficeBox.Id;
                }
            }

            Context.OrganizationUnits.Add(entity);

            if (entity.Agent != null)
            {
                entity.Agent.Id = entity.Id;
                Context.Agents.Add(entity.Agent);

                if (entity.Agent.ClientAgents != null)
                {
                    foreach (var clientAgent in entity.Agent.ClientAgents)
                    {
                        clientAgent.ClientId = entity.Agent.Id;
                        Context.ClientAgents.Add(clientAgent);
                    }
                }
            }

            if (entity.Client != null)
            {
                entity.Client.Id = entity.Id;

                if (entity.Client.Mandate != null)
                {
                    entity.Client.Mandate.Id = entity.Id;

                    if (entity.Client.Mandate.ClientMandates != null)
                        foreach (var clientMandate in entity.Client.Mandate.ClientMandates)
                        {
                            clientMandate.MandateId = entity.Id;

                            if (clientMandate.Client != null && clientMandate.Client.ClientInvoiceRecipients.Any())
                            {
                                var clientInvoiceRecipents = clientMandate.Client.ClientInvoiceRecipients.ToList();
                                foreach (var clientClientInvoiceRecipient in clientInvoiceRecipents)
                                {
                                    clientClientInvoiceRecipient.ClientId = clientMandate.ClientId;
                                    clientClientInvoiceRecipient.OrganizationUnitId = entity.Id;
                                    Context.ClientInvoiceRecipients.Add(clientClientInvoiceRecipient);
                                }
                            }

                            if (clientMandate.Client != null)
                                Context.Entry(clientMandate.Client).State = EntityState.Detached;
                            Context.ClientMandates.Add(clientMandate);
                        }

                    Context.Mandates.Add(entity.Client.Mandate);
                }

                if (entity.Client.ClientPreferredSuppliers != null && entity.Client.ClientPreferredSuppliers.Count > 0)
                {
                    foreach (var client in entity.Client.ClientPreferredSuppliers.ToList())
                    {
                        client.Client_Id = entity.Id;
                        Context.ClientPreferredSuppliers.Add(client);
                    }
                }

                Context.Clients.Add(entity.Client);
            }

            if (entity.Repairer != null)
            {
                if (entity.Repairer.RepairerSkills != null && entity.Repairer.RepairerSkills.Count > 0)
                {
                    foreach (var repairerSkill in entity.Repairer.RepairerSkills.ToList())
                    {
                        repairerSkill.RepairerId = entity.Id;
                        Context.RepairerSkills.Add(repairerSkill);
                        if (repairerSkill.Skill != null)
                            Context.Entry(repairerSkill.Skill).State = EntityState.Detached;
                    }
                }

                entity.Repairer.Id = entity.Id;
                Context.Repairers.Add(entity.Repairer);
            }

            if (entity.Partner != null)
            {
                entity.Partner.Id = entity.Id;
                Context.Partners.Add(entity.Partner);
            }

            if (entity.AlarmCenter != null)
            {
                entity.AlarmCenter.Id = entity.Id;
                Context.AlarmCenters.Add(entity.AlarmCenter);
            }

            if (entity.InternationalAssistanceGroup != null)
            {
                entity.InternationalAssistanceGroup.Id = entity.Id;
                Context.InternationalAssistanceGroups.Add(entity.InternationalAssistanceGroup);
            }

            if (entity.Supplier != null)
            {
                if (entity.Supplier.SupplierServices != null && entity.Supplier.SupplierServices.Count > 0)
                {
                    foreach (var suppllierService in entity.Supplier.SupplierServices.ToList())
                    {
                        suppllierService.SupplierId = entity.Id;
                        Context.SupplierServices.Add(suppllierService);
                    }
                }

                if (entity.Supplier.SupplierBrands != null && entity.Supplier.SupplierBrands.Count > 0)
                {
                    foreach (var supplierBrand in entity.Supplier.SupplierBrands.ToList())
                    {
                        supplierBrand.SupplierId = entity.Id;
                        Context.SupplierBrands.Add(supplierBrand);
                        if (supplierBrand.Brand != null)
                            Context.Entry(supplierBrand.Brand).State = EntityState.Detached;
                    }
                }

                if (entity.Supplier.ClientPreferredSuppliers != null && entity.Supplier.ClientPreferredSuppliers.Count > 0)
                {
                    foreach (var client in entity.Supplier.ClientPreferredSuppliers.ToList())
                    {
                        client.Supplier_Id = entity.Id;
                        Context.ClientPreferredSuppliers.Add(client);
                    }
                }

                entity.Supplier.Id = entity.Id;
                Context.Suppliers.Add((entity.Supplier));
            }

            if (entity.OrganizationHierarchy != null)
            {
                entity.OrganizationHierarchy.Id = entity.Id;
                entity.OrganizationHierarchy.StartDate = DateTime.UtcNow;
                Context.OrganizationHierarchies.Add(entity.OrganizationHierarchy);
            }

            if (entity.OrganizationCodes != null && entity.OrganizationCodes.Count > 0)
            {
                foreach (var code in entity.OrganizationCodes)
                {
                    code.OrganizationUnitId = entity.Id;
                    Context.OrganizationCodes.Add(code);
                }
            }

            if (entity.OrganizationLabels != null && entity.OrganizationLabels.Count > 0)
            {
                foreach (var label in entity.OrganizationLabels)
                {
                    label.OrganizationUnitId = entity.Id;
                    Context.OrganizationLabels.Add(label);
                }
            }

            if (entity.OrganizationNotes != null && entity.OrganizationNotes.Count > 0)
            {
                foreach (var note in entity.OrganizationNotes)
                {
                    note.OrganizationUnitId = entity.Id;
                    Context.OrganizationNotes.Add(note);
                }
            }

            if (entity.ContactPersons != null && entity.ContactPersons.Count > 0)
            {
                foreach (var person in entity.ContactPersons)
                {
                    person.OrganizationUnitId = entity.Id;
                    Context.ContactPersons.Add(person);
                }
            }

            if (entity.BusinessHours != null && entity.BusinessHours.Count > 0)
            {
                foreach (var businessHour in entity.BusinessHours)
                {
                    businessHour.OrganizationUnitId = entity.Id;
                    Context.BusinessHours.Add(businessHour);
                }
            }

            if (entity.Insurer != null)
            {
                entity.Insurer.Id = entity.Id;
                Context.Insurers.Add(entity.Insurer);

                if (entity.Insurer.InsurerAlarmCenters != null && entity.Insurer.InsurerAlarmCenters.Count > 0)
                {
                    foreach (var insurerAlarmCenter in (entity.Insurer.InsurerAlarmCenters))
                    {
                        insurerAlarmCenter.InsurerId = entity.Id;
                        Context.InsurerAlarmCenters.Add(insurerAlarmCenter);
                    }
                }
            }

            if (entity.ContractParty != null)
            {
                entity.ContractParty.Id = entity.Id;
                Context.ContractParties.Add(entity.ContractParty);
            }

            if (entity.ConvenantParty != null)
            {
                entity.ConvenantParty.Id = entity.Id;
                Context.ConvenantParties.Add(entity.ConvenantParty);
            }

            if (entity.RoadAuthority != null)
            {
                entity.RoadAuthority.Id = entity.Id;
                Context.RoadAuthorities.Add(entity.RoadAuthority);
            }

            if (entity.LeasingCompany != null)
            {
                entity.LeasingCompany.Id = entity.Id;
                Context.LeasingCompanies.Add(entity.LeasingCompany);

                if (entity.LeasingCompany.LeasingCompanyAlarmCenters != null && entity.LeasingCompany.LeasingCompanyAlarmCenters.Count > 0)
                {
                    foreach (var leasingCompanyAlarmCenter in (entity.LeasingCompany.LeasingCompanyAlarmCenters))
                    {
                        leasingCompanyAlarmCenter.LeasingCompanyId = entity.Id;
                        Context.LeasingCompanyAlarmCenters.Add(leasingCompanyAlarmCenter);
                    }
                }
            }

            CRUD(entity.OrganizationApplications,
                await Context.OrganizationApplications.AsNoTracking().Where(oa => oa.OrganizationId == entity.Id).ToListAsync(),
                oa =>
                {
                    oa.OrganizationId = entity.Id;
                    Context.OrganizationApplications.Add(oa);
                    Context.Entry(oa.Application).State = EntityState.Detached;
                },
                oa => { },
                oa => Context.Entry(oa).State = EntityState.Deleted);

            return entity;
        }

        private async Task<OrganizationUnit> UpdateOne(Guid key, OrganizationUnit entity)
        {
            CRUD(entity.OrganizationAddresses,
                await Context.OrganizationAddresses.AsNoTracking().Where(oa => oa.OrganizationUnitId == entity.Id).ToListAsync(),
                oa =>
                {
                    oa.OrganizationUnitId = entity.Id;
                    if (oa.Address != null)
                    {
                        oa.Address.Id = new Guid();
                    }
                    if (oa.PostOfficeBox != null)
                    {
                        oa.PostOfficeBox.Id = new Guid();
                    }
                    if (oa.AddressTypeId == Guid.Empty)
                    {
                        oa.AddressType = Context.AddressTypes.FirstOrDefault(at => at.Code == "PA");
                    }
                    Context.OrganizationAddresses.Add(oa);
                },
                oa =>
                {
                    if (oa.Address != null)
                    {
                        Context.Entry(oa.Address).State = EntityState.Modified;
                    }
                    if (oa.PostOfficeBox != null)
                    {
                        Context.Entry(oa.PostOfficeBox).State = EntityState.Modified;
                    }
                    Context.Entry(oa).State = EntityState.Modified;
                },
                oa =>
                {
                    Context.Entry(oa).State = EntityState.Deleted;
                    if (oa.Address != null)
                    {
                        Context.Entry(oa.Address).State = EntityState.Deleted;
                    }
                    if (oa.PostOfficeBox != null)
                    {
                        Context.Entry(oa.PostOfficeBox).State = EntityState.Deleted;
                    }
                });

            var firstAddress = entity.OrganizationAddresses?.FirstOrDefault(oa => oa.Address != null)?.Address;
            if (firstAddress != null)
            {
                entity.Address = firstAddress;
                entity.AddressId = firstAddress.Id;
            }
            else
            {
                entity.Address = null;
                entity.AddressId = null;
            }

            var firstPostOfficeBox = entity.OrganizationAddresses?.FirstOrDefault(oa => oa.PostOfficeBox != null)?.PostOfficeBox;
            if (firstPostOfficeBox != null)
            {
                entity.PostOfficeBox = firstPostOfficeBox;
                entity.PostOfficeBoxId = firstPostOfficeBox.Id;
            }
            else
            {
                entity.PostOfficeBox = null;
                entity.PostOfficeBoxId = null;
            }

            if (entity.Agent != null)
            {
                if (entity.Agent.Id == Guid.Empty)
                {
                    entity.Agent.Id = key;
                    Context.Agents.Add(entity.Agent);

                    if (entity.Agent.ClientAgents != null)
                    {
                        foreach (var clientAgent in entity.Agent.ClientAgents)
                        {
                            clientAgent.AgentId = entity.Agent.Id;
                            Context.ClientAgents.Add(clientAgent);
                        }
                    }
                }
                else
                {
                    Context.Entry(entity.Agent).State = EntityState.Modified;

                    CRUD(entity.Agent.ClientAgents,
                        await Context.ClientAgents.AsNoTracking().Where(ca => ca.AgentId == entity.Agent.Id).ToListAsync(),
                        ca =>
                        {
                            ca.AgentId = entity.Agent.Id;
                            Context.ClientAgents.Add(ca);
                        },
                        ca =>
                        {
                            Context.Entry(ca).State = EntityState.Modified;
                        },
                        ca => {/* Ignore */});
                }
            }

            if (entity.Client != null)
            {
                #region mandate

                if (entity.Client.Mandate != null)
                {
                    if (entity.Client.Mandate.Id == Guid.Empty)
                    {
                        entity.Client.Mandate.Id = entity.Id;

                        if (entity.Client.Mandate.ClientMandates != null)
                            foreach (var clientMandate in entity.Client.Mandate.ClientMandates)
                            {
                                clientMandate.MandateId = entity.Id;

                                if (clientMandate.Client?.ClientInvoiceRecipients != null)
                                {
                                    foreach (var clientClientInvoiceRecipient in clientMandate.Client.ClientInvoiceRecipients)
                                    {
                                        clientClientInvoiceRecipient.ClientId = clientMandate.ClientId;
                                        clientClientInvoiceRecipient.OrganizationUnitId = entity.Id;
                                        Context.ClientInvoiceRecipients.Add(clientClientInvoiceRecipient);
                                    }
                                }

                                clientMandate.Client = null;
                                Context.ClientMandates.Add(clientMandate);
                            }

                        Context.Mandates.Add(entity.Client.Mandate);
                    }
                    else
                    {
                        #region Client mandates

                        var oldClientMandates = Context.ClientMandates.AsNoTracking().Where(e => e.MandateId == entity.Id).ToList();
                        var newClientMandets = entity.Client.Mandate.ClientMandates;

                        if (newClientMandets != null && newClientMandets.Count > 0)
                        {
                            // Create
                            foreach (var clientMandate in newClientMandets.Where(n => n.Id == Guid.Empty))
                            {
                                clientMandate.MandateId = entity.Id;
                                Context.ClientMandates.Add(clientMandate);

                                if (clientMandate.Client == null || !clientMandate.Client.ClientInvoiceRecipients.Any())
                                    continue;

                                foreach (var clientClientInvoiceRecipient in clientMandate.Client.ClientInvoiceRecipients)
                                {
                                    clientClientInvoiceRecipient.ClientId = clientMandate.ClientId;
                                    clientClientInvoiceRecipient.OrganizationUnitId = entity.Id;
                                    Context.ClientInvoiceRecipients.Add(clientClientInvoiceRecipient);
                                }
                            }

                            // Update
                            foreach (var clientMandate in newClientMandets.Where(n => oldClientMandates.Any(o => o.Id == n.Id)))
                            {
                                if (clientMandate.Client != null)
                                {
                                    CRUD(clientMandate.Client.ClientInvoiceRecipients,
                                        await Context.ClientInvoiceRecipients.AsNoTracking().Where(cir => cir.ClientId == clientMandate.ClientId && cir.OrganizationUnitId == key).ToListAsync(),
                                        cir =>
                                        {
                                            cir.OrganizationUnitId = key;
                                            cir.ClientId = clientMandate.ClientId;
                                            Context.ClientInvoiceRecipients.Add(cir);
                                        },
                                        cir =>
                                        {
                                            cir.OrganizationUnit = null;
                                            Context.Entry(cir).State = EntityState.Modified;
                                        },
                                        cir =>
                                        {
                                            Context.Entry(cir).State = EntityState.Deleted;
                                        });
                                }

                                clientMandate.Client = null;
                                Context.Entry(clientMandate).State = EntityState.Modified;
                            }

                            // Delete
                            foreach (var clientMandate in oldClientMandates.Where(o => newClientMandets.All(n => n.Id != o.Id)))
                            {
                                Context.ClientMandates.Attach(clientMandate);
                                Context.ClientMandates.Remove(clientMandate);

                                var clientInvoiceRecipients = Context.ClientInvoiceRecipients.Where(cir => cir.ClientId == clientMandate.ClientId && cir.OrganizationUnitId == key).ToList();

                                foreach (var clientInvoiceRecipient in clientInvoiceRecipients)
                                {
                                    Context.Entry(clientInvoiceRecipient).State = EntityState.Deleted;
                                }
                            }
                        }
                        else if (oldClientMandates.Count > 0)
                        {
                            foreach (var clientMandate in oldClientMandates)
                            {
                                Context.ClientMandates.Attach(clientMandate);
                                Context.ClientMandates.Remove(clientMandate);

                                var clientInvoiceRecipients = Context.ClientInvoiceRecipients.Where(cir => cir.ClientId == clientMandate.ClientId && cir.OrganizationUnitId == key).ToList();

                                foreach (var clientInvoiceRecipient in clientInvoiceRecipients)
                                {
                                    Context.Entry(clientInvoiceRecipient).State = EntityState.Deleted;
                                }
                            }
                        }

                        #endregion

                        Context.Entry(entity.Client.Mandate).State = EntityState.Modified;
                    }
                }

                #endregion

                if (entity.Client.Id == Guid.Empty)
                {
                    entity.Client.Id = key;
                    if (entity.Client.OrganizationUnit != null)
                        Context.Entry(entity.Client.OrganizationUnit).State = EntityState.Detached;

                    if (entity.Client.ClientPreferredSuppliers != null && entity.Client.ClientPreferredSuppliers.Count > 0)
                    {
                        foreach (var client in entity.Client.ClientPreferredSuppliers.ToList())
                        {
                            client.Client_Id = entity.Id;
                            Context.ClientPreferredSuppliers.Add(client);
                        }
                    }

                    Context.Clients.Add(entity.Client);
                }
                else
                {
                    #region Clients preferred suppliers

                    var oldClients = Context.ClientPreferredSuppliers.AsNoTracking().Where(e => e.Client_Id == key).ToList();
                    var newClients = entity.Client.ClientPreferredSuppliers;

                    if (newClients != null && newClients.Count > 0)
                    {
                        foreach (var client in newClients)
                        {
                            if (client.Id == Guid.Empty) //create
                            {
                                client.Client_Id = entity.Id;
                                Context.ClientPreferredSuppliers.Add(client);
                            }
                            else //update
                            {
                                var existingClient = oldClients.FirstOrDefault(c => c.Id == client.Id);
                                if (existingClient != null)
                                {
                                    Context.Entry(client).State = EntityState.Modified;
                                }
                            }
                        }

                        //delete
                        foreach (var client in oldClients.Where(o => newClients.All(n => n.Id != o.Id)))
                        {
                            Context.ClientPreferredSuppliers.Attach(client);
                            Context.ClientPreferredSuppliers.Remove(client);
                        }
                    }
                    else if (oldClients.Count > 0) //delete all
                    {
                        foreach (var client in oldClients)
                        {
                            Context.ClientPreferredSuppliers.Attach(client);
                            Context.ClientPreferredSuppliers.Remove(client);
                        }
                    }

                    #endregion

                    #region Debtor organization relations

                    if (entity.Inactive)
                    {
                        var oldDOR = Context.DebtorOrganizationRelations.Where(x => x.OrganizationUnitId == entity.Id).AsNoTracking().ToList();
                        foreach (var dor in oldDOR)
                        {
                            Context.DebtorOrganizationRelations.Attach(dor);
                            Context.DebtorOrganizationRelations.Remove(dor);
                        }
                    }

                    #endregion

                    Context.Entry(entity.Client).State = EntityState.Modified;
                }
            }

            if (entity.Repairer != null)
            {
                if (entity.Repairer.Id == Guid.Empty)
                {
                    if (entity.Repairer.RepairerSkills != null && entity.Repairer.RepairerSkills.Count > 0)
                    {
                        foreach (var repairerSkill in entity.Repairer.RepairerSkills)
                        {
                            repairerSkill.RepairerId = key;
                            Context.RepairerSkills.Add(repairerSkill);
                            if (repairerSkill.Skill != null)
                                Context.Entry(repairerSkill.Skill).State = EntityState.Detached;
                        }
                    }

                    entity.Repairer.Id = key;
                    Context.Repairers.Add(entity.Repairer);
                }
                else
                {
                    #region Repairer skills

                    var oldRepairerSkills = Context.RepairerSkills.AsNoTracking().Where(e => e.RepairerId == key).ToList();
                    var newRepairerSkills = entity.Repairer.RepairerSkills;

                    if (newRepairerSkills != null && newRepairerSkills.Count > 0)
                    {
                        // Create
                        foreach (var repairerSkill in newRepairerSkills.Where(n => n.Id == Guid.Empty))
                        {
                            repairerSkill.RepairerId = entity.Id;
                            Context.RepairerSkills.Add(repairerSkill);
                            if (repairerSkill.Skill != null)
                                Context.Entry(repairerSkill.Skill).State = EntityState.Detached;
                        }

                        // Delete
                        foreach (var repairerSkill in oldRepairerSkills.Where(o => newRepairerSkills.All(n => n.Id != o.Id)))
                        {
                            Context.RepairerSkills.Attach(repairerSkill);
                            Context.RepairerSkills.Remove(repairerSkill);
                        }
                    }
                    else if (oldRepairerSkills.Count > 0)
                    {
                        foreach (var repairerSkill in oldRepairerSkills)
                        {
                            Context.RepairerSkills.Attach(repairerSkill);
                            Context.RepairerSkills.Remove(repairerSkill);
                        }
                    }

                    #endregion

                    Context.Entry(entity.Repairer).State = EntityState.Modified;
                }
            }

            if (entity.Partner != null)
            {
                if (entity.Partner.Id == Guid.Empty)
                {
                    entity.Partner.Id = key;
                    Context.Partners.Add(entity.Partner);
                }
                else
                {
                    Context.Entry(entity.Partner).State = EntityState.Modified;
                }
            }

            if (entity.Insurer != null)
            {
                if (entity.Insurer.Id == Guid.Empty)
                {
                    entity.Insurer.Id = key;

                    if (entity.Insurer.InsurerAlarmCenters != null && entity.Insurer.InsurerAlarmCenters.Count > 0)
                    {
                        foreach (var insurerAlarmCenter in (entity.Insurer.InsurerAlarmCenters))
                        {
                            insurerAlarmCenter.InsurerId = entity.Id;
                            Context.InsurerAlarmCenters.Add(insurerAlarmCenter);
                        }
                    }

                    Context.Insurers.Add(entity.Insurer);
                }
                else
                {
                    CRUD(entity.Insurer.InsurerAlarmCenters,
                        await Context.InsurerAlarmCenters.AsNoTracking().Where(iac => iac.InsurerId == key).ToListAsync(),
                        iac =>
                        {
                            iac.InsurerId = key;
                            Context.InsurerAlarmCenters.Add(iac);
                        },
                        iac => { },
                        iac =>
                        {
                            Context.Entry(iac).State = EntityState.Deleted;
                        });

                    Context.Entry(entity.Insurer).State = EntityState.Modified;
                }
            }

            if (entity.AlarmCenter != null)
            {
                if (entity.AlarmCenter.Id == Guid.Empty)
                {
                    entity.AlarmCenter.Id = key;
                    Context.AlarmCenters.Add(entity.AlarmCenter);
                }
                else
                {
                    Context.Entry(entity.AlarmCenter).State = EntityState.Modified;
                }
            }

            if (entity.InternationalAssistanceGroup != null)
            {
                if (entity.InternationalAssistanceGroup.Id == Guid.Empty)
                {
                    entity.InternationalAssistanceGroup.Id = key;
                    Context.InternationalAssistanceGroups.Add(entity.InternationalAssistanceGroup);
                }
                else
                {
                    Context.Entry(entity.InternationalAssistanceGroup).State = EntityState.Modified;
                }
            }

            if (entity.Supplier != null)
            {
                if (entity.Supplier.Id == Guid.Empty)
                {
                    if (entity.Supplier.SupplierServices != null && entity.Supplier.SupplierServices.Count > 0)
                    {
                        foreach (var suppllierService in entity.Supplier.SupplierServices.ToList())
                        {
                            suppllierService.SupplierId = entity.Id;
                            Context.SupplierServices.Add(suppllierService);
                        }
                    }

                    if (entity.Supplier.SupplierBrands != null && entity.Supplier.SupplierBrands.Count > 0)
                    {
                        foreach (var supplierBrand in entity.Supplier.SupplierBrands.ToList())
                        {
                            supplierBrand.SupplierId = entity.Id;
                            Context.SupplierBrands.Add(supplierBrand);
                            if (supplierBrand.Brand != null)
                                Context.Entry(supplierBrand.Brand).State = EntityState.Detached;
                        }
                    }

                    if (entity.Supplier.ClientPreferredSuppliers != null && entity.Supplier.ClientPreferredSuppliers.Count > 0)
                    {
                        foreach (var client in entity.Supplier.ClientPreferredSuppliers.ToList())
                        {
                            client.Supplier_Id = entity.Id;
                            Context.ClientPreferredSuppliers.Add(client);
                        }
                    }

                    entity.Supplier.Id = key;
                    Context.Suppliers.Add(entity.Supplier);
                }
                else
                {
                    #region Supplier services

                    var oldSupplierService = Context.SupplierServices.AsNoTracking().Where(e => e.SupplierId == key).ToList();
                    var newSupplierService = entity.Supplier.SupplierServices;

                    if (newSupplierService != null && newSupplierService.Count > 0)
                    {
                        // Create
                        foreach (var supplierService in newSupplierService.Where(n => n.Id == Guid.Empty))
                        {
                            supplierService.SupplierId = entity.Id;
                            Context.SupplierServices.Add(supplierService);
                        }

                        // Update
                        foreach (var supplierService in newSupplierService.Where(n => oldSupplierService.Any(o => o.Id == n.Id)))
                        {
                            Context.Entry(supplierService).State = EntityState.Modified;
                        }

                        // Delete
                        foreach (var supplierService in oldSupplierService.Where(o => newSupplierService.All(n => n.Id != o.Id)))
                        {
                            Context.SupplierServices.Attach(supplierService);
                            Context.SupplierServices.Remove(supplierService);
                        }
                    }
                    else if (oldSupplierService.Count > 0)
                    {
                        foreach (var supplierService in oldSupplierService)
                        {
                            Context.SupplierServices.Attach(supplierService);
                            Context.SupplierServices.Remove(supplierService);
                        }
                    }

                    #endregion

                    #region Supplier brands

                    var oldSupplierBrands = Context.SupplierBrands.AsNoTracking().Where(e => e.SupplierId == key).ToList();
                    var newSupplierBrands = entity.Supplier.SupplierBrands;

                    if (newSupplierBrands != null && newSupplierBrands.Count > 0)
                    {
                        // Create
                        foreach (var supplierBrand in newSupplierBrands.Where(n => n.Id == Guid.Empty))
                        {
                            supplierBrand.SupplierId = entity.Id;
                            Context.SupplierBrands.Add(supplierBrand);
                            if (supplierBrand.Brand != null)
                                Context.Entry(supplierBrand.Brand).State = EntityState.Detached;
                        }

                        // Delete
                        foreach (var supplierBrand in oldSupplierBrands.Where(o => newSupplierBrands.All(n => n.Id != o.Id)))
                        {
                            Context.SupplierBrands.Attach(supplierBrand);
                            Context.SupplierBrands.Remove(supplierBrand);
                        }
                    }
                    else if (oldSupplierBrands.Count > 0)
                    {
                        foreach (var supplierBrand in oldSupplierBrands)
                        {
                            Context.SupplierBrands.Attach(supplierBrand);
                            Context.SupplierBrands.Remove(supplierBrand);
                        }
                    }

                    #endregion

                    #region Clients preferred suppliers

                    var oldClients = Context.ClientPreferredSuppliers.AsNoTracking().Where(e => e.Supplier_Id == key).ToList();
                    var newClients = entity.Supplier.ClientPreferredSuppliers;

                    if (newClients != null && newClients.Count > 0)
                    {
                        foreach (var client in newClients)
                        {
                            if (client.Id == Guid.Empty) //create
                            {
                                client.Supplier_Id = entity.Id;
                                Context.ClientPreferredSuppliers.Add(client);
                            }
                            else //update
                            {
                                var existingClient = oldClients.FirstOrDefault(c => c.Id == client.Id);
                                if (existingClient != null)
                                {
                                    Context.Entry(client).State = EntityState.Modified;
                                }
                            }
                        }

                        //delete
                        foreach (var client in oldClients.Where(o => newClients.All(n => n.Id != o.Id)))
                        {
                            Context.ClientPreferredSuppliers.Attach(client);
                            Context.ClientPreferredSuppliers.Remove(client);
                        }
                    }
                    else if (oldClients.Count > 0) //delete all
                    {
                        foreach (var client in oldClients)
                        {
                            Context.ClientPreferredSuppliers.Attach(client);
                            Context.ClientPreferredSuppliers.Remove(client);
                        }
                    }

                    #endregion

                    Context.Entry(entity.Supplier).State = EntityState.Modified;
                }
            }

            if (entity.ContractParty != null)
            {
                if (entity.ContractParty.Id == Guid.Empty)
                {
                    entity.ContractParty.Id = key;
                    Context.ContractParties.Add(entity.ContractParty);
                }
                else
                {
                    Context.Entry(entity.ContractParty).State = EntityState.Modified;
                }
            }

            if (entity.ConvenantParty != null)
            {
                if (entity.ConvenantParty.Id == Guid.Empty)
                {
                    entity.ConvenantParty.Id = key;
                    Context.ConvenantParties.Add(entity.ConvenantParty);
                }
                else
                {
                    Context.Entry(entity.ConvenantParty).State = EntityState.Modified;
                }
            }

            if (entity.OrganizationHierarchy != null)
            {
                if (entity.OrganizationHierarchy.Id == Guid.Empty)
                {
                    entity.OrganizationHierarchy.Id = key;
                    entity.OrganizationHierarchy.StartDate = DateTime.UtcNow;
                    Context.OrganizationHierarchies.Add(entity.OrganizationHierarchy);
                }
                else
                {
                    Context.Entry(entity.OrganizationHierarchy).State = EntityState.Modified;
                }
            }
            else if (await Context.OrganizationHierarchies.AnyAsync(oh => oh.Id == key))
            {
                Context.Entry(new OrganizationHierarchy() { Id = key }).State = EntityState.Deleted;
            }

            if (entity.RoadAuthority != null)
            {
                if (entity.RoadAuthority.Id == Guid.Empty)
                {
                    entity.RoadAuthority.Id = key;
                    Context.RoadAuthorities.Add(entity.RoadAuthority);
                }
                else
                {
                    Context.Entry(entity.RoadAuthority).State = EntityState.Modified;
                }
            }

            if (entity.LeasingCompany != null)
            {
                if (entity.LeasingCompany.Id == Guid.Empty)
                {
                    entity.LeasingCompany.Id = key;

                    if (entity.LeasingCompany.LeasingCompanyAlarmCenters != null && entity.LeasingCompany.LeasingCompanyAlarmCenters.Count > 0)
                    {
                        foreach (var leasingCompanyAlarmCenter in (entity.LeasingCompany.LeasingCompanyAlarmCenters))
                        {
                            leasingCompanyAlarmCenter.LeasingCompanyId = entity.Id;
                            Context.LeasingCompanyAlarmCenters.Add(leasingCompanyAlarmCenter);
                        }
                    }

                    Context.LeasingCompanies.Add(entity.LeasingCompany);
                }
                else
                {
                    CRUD(entity.LeasingCompany.LeasingCompanyAlarmCenters,
                        await Context.LeasingCompanyAlarmCenters.AsNoTracking().Where(lac => lac.LeasingCompanyId == key).ToListAsync(),
                        lac =>
                        {
                            lac.LeasingCompanyId = key;
                            Context.LeasingCompanyAlarmCenters.Add(lac);
                        },
                        lac => { },
                        lac =>
                        {
                            Context.Entry(lac).State = EntityState.Deleted;
                        });

                    Context.Entry(entity.LeasingCompany).State = EntityState.Modified;
                }
            }

            #region Organization codes

            var oldCodes = Context.OrganizationCodes.AsNoTracking().Where(e => e.OrganizationUnitId == entity.Id).ToList();
            var newCodes = entity.OrganizationCodes;

            // Add
            foreach (var code in newCodes.Where(e => e.Id == Guid.Empty))
            {
                code.OrganizationUnitId = entity.Id;
                Context.OrganizationCodes.Add(code);
            }

            // Update
            foreach (var code in newCodes.Where(n => oldCodes.Any(old => old.Id == n.Id)))
            {
                Context.Entry(code).State = EntityState.Modified;
            }

            // Delete
            foreach (var code in oldCodes.Where(old => newCodes.All(n => n.Id != old.Id)))
            {
                Context.OrganizationCodes.Attach(code);
                Context.OrganizationCodes.Remove(code);
            }

            #endregion

            #region Organization labels

            var oldLabels = Context.OrganizationLabels.AsNoTracking().Where(e => e.OrganizationUnitId == entity.Id).ToList();
            var newLabels = entity.OrganizationLabels;

            // Add
            foreach (var label in newLabels.Where(e => e.Id == Guid.Empty))
            {
                label.OrganizationUnitId = entity.Id;
                Context.OrganizationLabels.Add(label);
            }

            // Update
            foreach (var label in newLabels.Where(n => oldLabels.Any(old => old.Id == n.Id)))
            {
                Context.Entry(label).State = EntityState.Modified;
            }

            // Delete
            foreach (var label in oldLabels.Where(old => newLabels.All(n => n.Id != old.Id)))
            {
                Context.OrganizationLabels.Attach(label);
                Context.OrganizationLabels.Remove(label);
            }

            #endregion
            #region Organization notes
            CRUD(entity.OrganizationNotes,
                Context.OrganizationNotes.AsNoTracking().Where(e => e.OrganizationUnitId == entity.Id).ToList(),
                note =>
                {
                    note.OrganizationUnitId = entity.Id;
                    Context.OrganizationNotes.Add(note);
                },
                note =>
                {
                    Context.Entry(note).State = EntityState.Modified;
                },
                note =>
                {
                    Context.Entry(note).State = EntityState.Deleted;
                }
            );
            #endregion

            #region Organization contacts

            var oldContacts = Context.OrganizationContacts.AsNoTracking().Where(e => e.OrganizationUnitId == entity.Id).ToList();
            var newContacts = entity.OrganizationContacts;

            // Add
            foreach (var contact in newContacts.Where(e => e.Id == Guid.Empty))
            {
                contact.OrganizationUnitId = entity.Id;
                Context.OrganizationContacts.Add(contact);
            }

            // Update
            foreach (var Contact in newContacts.Where(n => oldContacts.Any(old => old.Id == n.Id)))
            {
                Context.Entry(Contact).State = EntityState.Modified;
            }

            // Delete
            foreach (var Contact in oldContacts.Where(old => newContacts.All(n => n.Id != old.Id)))
            {
                Context.OrganizationContacts.Attach(Contact);
                Context.OrganizationContacts.Remove(Contact);
            }

            var contactTypes = Context.ContactTypes.ToList();

            var primaryPhone = newContacts.FirstOrDefault(c => c.IsPrimary && c.ContactTypeId == contactTypes.FirstOrDefault(c2 => c2.IsLandline())?.Id);
            if (primaryPhone != null)
                entity.Phone = primaryPhone.ContactValue;

            var primaryEmail = newContacts.FirstOrDefault(c => c.IsPrimary && c.ContactTypeId == contactTypes.FirstOrDefault(c2 => c2.IsEmail())?.Id);
            if (primaryEmail != null)
                entity.Email = primaryEmail.ContactValue;

            #endregion

            #region Contact persons

            if (entity.ContactPersons != null && entity.ContactPersons.Count > 0)
            {
                var oldPerson = Context.ContactPersons.AsNoTracking().Where(e => e.OrganizationUnitId == entity.Id).ToList();
                var newPerson = entity.ContactPersons;

                // Add
                foreach (var person in newPerson.Where(e => e.Id == Guid.Empty))
                {
                    person.OrganizationUnitId = entity.Id;
                    Context.ContactPersons.Add(person);
                }

                // Update
                foreach (var person in newPerson.Where(n => oldPerson.Any(old => old.Id == n.Id)))
                {
                    Context.Entry(person).State = EntityState.Modified;
                }
            }

            #endregion

            #region Statuses

            if (entity.ValidationStatusHistories != null && entity.ValidationStatusHistories.Count > 0)
            {
                // Only add new statusses to retain the history
                foreach (var statusHistory in entity.ValidationStatusHistories.Where(e => e.Id == Guid.Empty))
                {
                    statusHistory.OrganizationUnitId = entity.Id;
                    Context.OrganizationUnitValidationStatusHistories.Add(statusHistory);
                }
            }

            #endregion

            CRUD(entity.BusinessHours,
                Context.BusinessHours.AsNoTracking().Where(e => e.OrganizationUnitId == entity.Id).ToList(),
                businessHour =>
                {
                    businessHour.OrganizationUnitId = entity.Id;
                    Context.BusinessHours.Add(businessHour);
                },
                businessHour =>
                {
                    Context.Entry(businessHour).State = EntityState.Modified;
                },
                businessHour => {/* Ignore */ }
            );

            CRUD(entity.OrganizationApplications,
                await Context.OrganizationApplications.AsNoTracking().Where(oa => oa.OrganizationId == entity.Id).ToListAsync(),
                oa =>
                {
                    oa.OrganizationId = entity.Id;
                    Context.OrganizationApplications.Add(oa);
                    Context.Entry(oa.Application).State = EntityState.Detached;
                },
                oa => { },
                oa => Context.Entry(oa).State = EntityState.Deleted);

            Context.Entry(entity).State = EntityState.Modified;

            return entity;
        }

        /// <summary>
        /// Fills the dropdown where deparments have to fall under a specific legal entity.
        /// </summary>
        /// <param name="legalEntityId"> The legal entity under which the department has to fall </param>
        /// <param name="filter"> Text has either the code or long name as to contain </param>
        /// <returns></returns>
        [HttpGet]
        [ODataRoute("Services.FillDepartmentDropDown(legalEntityId={legalEntityId}, filter={filter})")]
        public async Task<IHttpActionResult> FillDepartmentDropDown([FromODataUri] Guid legalEntityId, [FromODataUri] string filter)
        {
            async Task<List<Guid>> GetChildrenRecursivelyAsync(Guid parentId)
            {
                var allChildren = new List<Guid>();

                var queryResult = await Context.OrganizationUnits
                    .Where(ou => ou.OrganizationHierarchy.ParentId == parentId)
                    .Select(ou => ou.Id)
                    .ToListAsync();

                if (queryResult.Count > 0)
                {
                    allChildren.AddRange(queryResult);

                    foreach (var id in queryResult)
                    {
                        var c = await GetChildrenRecursivelyAsync(id);

                        if (c.Count > 0) allChildren.AddRange(c);
                    }
                }

                return allChildren;
            }

            var childIds = await GetChildrenRecursivelyAsync(legalEntityId);

            if (childIds.Count <= 0)
                return Ok(new List<DropDownViewModel>());

            var organizations = new List<OrganizationUnit>();
            foreach (var id in childIds)
            {
                // By checking HierarchyTypeId, we know if its an department (Guid is seeded in the database).
                var query = Context.OrganizationUnits.Where(ou => ou.Id == id && ou.HierarchyTypeId == new Guid("FA65DD11-78FC-4E3B-9FF5-31093640AE78"));

                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(ou => ou.Code.Contains(filter) || ou.LongName.Contains(filter));
                }

                var organization = await query.SingleOrDefaultAsync();

                if (organization != null)
                {
                    organizations.Add(organization);
                }
            }

            var result = organizations
                .OrderBy(x => x.LongName)
                .Select(ou => new DropDownViewModel()
                {
                    Text = ou.Code + " - " + ou.LongName,
                    Value = ou.Id
                });

            return Ok(result);
        }

        private async Task<string> GenerateOrganizationUnitCode()
        {
            var getNextNumberResponse = await _numberingServiceClient.GetNextNumberAsync(new GetNextNumberRequest
            {
                DocumentDate = DateTime.Now,
                DocumentName = "CED.OU"
            });

            if (getNextNumberResponse.ExceptionOccurred)
            {
                throw new Exception(getNextNumberResponse.ExceptionMessage);
            }

            return getNextNumberResponse.NextNumber;
        }
    }
}