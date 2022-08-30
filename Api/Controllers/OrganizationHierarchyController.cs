using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Api.ViewModels;
using DataAccess;

namespace Api.Controllers
{
    public class OrganizationHierarchyController : ODataController
    {
        private readonly MasterDataContext _context = new MasterDataContext();

        [HttpGet]
        public async Task<IHttpActionResult> FillDropDown([FromODataUri] Guid? organizationId, [FromODataUri] string filter, [FromODataUri] Guid? hierarchyTypeId)
        {
            var query = _context.OrganizationUnitWithCurrentStatus.AsNoTracking().AsQueryable();

            // When creating a new organization, we can be sure that it
            // does not have any children of its own. So we don't have to 
            // filter out any potential circular references.
            if (organizationId != null)
            {
                // Never show itself
                query = query.Where(ou => ou.Id != organizationId);

                // !!! Warning !!!
                // This method of filtering potentially has dirty reads.
                var children = await GetChildrenRecursivelyAsync((Guid)organizationId);
                if (children.Count > 0)
                    foreach (var childId in children)
                        query = query.Where(ou => ou.Id != childId);
            }

            // Retrieve active 
            query = query.Where(ou => ou.Inactive == false);
            
            // Only retrieve approved organizations
            query = query.Where(ou => ou.ValidationStatusId == "433C02C7-35BE-4493-B236-B0D9C421DD17");

            #region Filter out based on hierarchy type.

            /* CED hierachy types have a specific order they must follow. */

            var cedFiscalEntityId = new Guid("59F8E18B-2BF3-41FF-A8F4-0796CF094519");
            var cedLegalAndFiscalId = new Guid("CE04A1BC-DCC8-405A-837A-0B2A0145F9A7");
            var cedLegalEntityId = new Guid("E1CFA385-1C31-4EEA-9B7C-6C314CF84513");
            var cedBusinessUnitId = new Guid("3D9F5A8E-EEDD-422F-9902-E2C85038DAC0");
            var cedDepartmentId = new Guid("FA65DD11-78FC-4E3B-9FF5-31093640AE78");

            if (hierarchyTypeId == cedFiscalEntityId)
                return BadRequest("CED Fiscal entity, cannot have an parent");

            if (hierarchyTypeId == cedLegalAndFiscalId)
                return BadRequest("CED Legal & Fiscal, cannot have an parent");

            if (hierarchyTypeId == cedLegalEntityId)
            {
                query = query.Where(ou => ou.HierarchyTypeId == cedFiscalEntityId);
            }
            else if (hierarchyTypeId == cedBusinessUnitId)
            {
                query = query.Where(ou => ou.HierarchyTypeId == cedLegalAndFiscalId || ou.HierarchyTypeId == cedLegalEntityId);
            }
            else if (hierarchyTypeId == cedDepartmentId)
            {
                query = query.Where(ou => ou.HierarchyTypeId == cedBusinessUnitId);
            }
            else
            {
                query = query.Where(ou => ou.HierarchyTypeId != cedFiscalEntityId
                                          && ou.HierarchyTypeId != cedLegalAndFiscalId
                                          && ou.HierarchyTypeId != cedLegalEntityId
                                          && ou.HierarchyTypeId != cedBusinessUnitId
                                          && ou.HierarchyTypeId != cedDepartmentId);
            }

            #endregion

            // Apply any search filter
            if (!string.IsNullOrEmpty(filter)) query = query.Where(ou => ou.Code == filter || ou.LongName.Contains(filter));

            var result = await query
                .Select(ou => new { ou.Id, ou.Code, ou.LongName })
                .OrderBy(x => x.LongName)
                .Take(25)
                .ToListAsync();

            return Ok(result.Select(ou => new DropDownViewModel { Value = ou.Id, Text = $"{ou.Code} - {ou.LongName}" }));
        }

        private async Task<List<Guid>> GetChildrenRecursivelyAsync(Guid parentId)
        {
            var allChildren = new List<Guid>();

            var queryResult = await _context.OrganizationUnits
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}