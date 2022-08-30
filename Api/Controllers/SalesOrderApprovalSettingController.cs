using Api.Attributes;
using Api.Constants;
using Api.Messages;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Api.Controllers
{
    public class SalesOrderApprovalSettingController : ApiController
    {
        private readonly MasterDataContext _context;

        public SalesOrderApprovalSettingController()
        {
            _context = new MasterDataContext();
        }

        [Route("api/salesorderapprovalsettings")]
        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Finance, AuthRoles.OrganizationUnit)]        
        public IHttpActionResult CreateOrUpdateSalesOrderApprovalSettings([FromBody] SalesOrderApprovalSettingEntry salesOrderApprovalSettingEntry)
        {
            if (salesOrderApprovalSettingEntry == null)
            {
                return BadRequest();
            }

            try
            {
                var debtorId = salesOrderApprovalSettingEntry.DebtorId;
                var legalEntityId = salesOrderApprovalSettingEntry.LegalEntityId;
                var clientEntrySystems = salesOrderApprovalSettingEntry.ClientEntrySystems ?? new List<ClientEntrySystemsEntry>();                
                var canApproveAutomatically = salesOrderApprovalSettingEntry.AutoAuthorizeChargedExpensesOrders || salesOrderApprovalSettingEntry.AutoAuthorizeFeeOrders;
                foreach (var entry in clientEntrySystems)
                {
                    var entrySystems = entry.EntrySystems ?? new List<string>();
                    var salesOrderApprovalSettings = _context.SalesOrderApprovalSettings.Where(s => s.ClientId == entry.ClientId && s.LegalEntityId == legalEntityId).ToList();
                    foreach (var entrySystem in entrySystems)
                    {
                        var salesOrderApprovalSetting = salesOrderApprovalSettings.FirstOrDefault(s => s.EntrySystem == entrySystem);
                        if(salesOrderApprovalSetting == null && canApproveAutomatically)
                        {
                            var newSalesOrderApprovalSetting = new SalesOrderApprovalSetting
                            {
                                ApprovedBySystem = true,
                                ClientId = entry.ClientId,
                                EffectiveDate = DateTime.UtcNow,
                                EntrySystem = entrySystem,
                                Id = Guid.NewGuid(),
                                LegalEntityId = legalEntityId                                
                            };
                            _context.SalesOrderApprovalSettings.Add(newSalesOrderApprovalSetting);
                        }
                        else if(salesOrderApprovalSetting != null)
                        {
                            salesOrderApprovalSetting.ApprovedBySystem = canApproveAutomatically;
                        }
                    }

                    //set ApprovedBySystem to false for the settings for which the entrysystem is not present in the request
                    var unapprovedSalesOrderApprovalSettings = salesOrderApprovalSettings.Where(s => !entrySystems.Contains(s.EntrySystem)).ToList();
                    unapprovedSalesOrderApprovalSettings.ForEach(s => s.ApprovedBySystem = false);
                }

                //set ApprovedBySystem to false for the settings for which the entrysystem and client are not present in the request
                //When you switch off the automated sales order it should deselect for all clients and for all applications the automated process
                if (debtorId != null && clientEntrySystems.Count == 0 && !canApproveAutomatically)
                {
                    var unapprovedSalesOrderApprovalSettings = _context.Set<SalesOrderApprovalSettingsView>()
                        .Where(s => s.DebtorId == debtorId && s.LegalEntityId == legalEntityId)
                        .Select(s=>s.Id)
                        .ToList();

                    var salesOrderApprovalSettings = _context.SalesOrderApprovalSettings
                        .Where(s => unapprovedSalesOrderApprovalSettings.Contains(s.Id))
                        .ToList();

                    salesOrderApprovalSettings.ForEach(s => s.ApprovedBySystem = false);
                }

                _context.SaveChanges();
                return Ok();
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}