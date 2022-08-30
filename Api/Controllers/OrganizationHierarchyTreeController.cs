using Api.ViewModels;
using DataAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{
    public class OrganizationHierarchyTreeController : ApiController
    {
        private readonly MasterDataContext _context = new MasterDataContext();

        [HttpGet]
        public async Task<IHttpActionResult> Get(Guid organizationId)
        {
            var topParentId = await GetTopParentId(organizationId);

            var parent = await _context.OrganizationUnits
                .Where(ou => ou.Id == topParentId)
                .Select(oh => new OrganizationHierarchyTreeNode()
                {
                    Id = oh.Id,
                    Name = oh.LongName
                })
                .SingleAsync();

            await BuildTree(parent);

            return Ok(parent);
        }

        private async Task<Guid> GetTopParentId(Guid organizationId)
        {
            var parentId = await _context.OrganizationHierarchies
                .AsNoTracking()
                .Where(oh => oh.Id == organizationId)
                .Select(oh => oh.ParentId)
                .SingleOrDefaultAsync();

            if (parentId != null && parentId != Guid.Empty)
            {
                return await GetTopParentId(parentId);
            }
            else
            {
                return organizationId;
            }
        }

        private async Task BuildTree(OrganizationHierarchyTreeNode parent)
        {
            var children = await _context.OrganizationHierarchies
                .AsNoTracking()
                .Include(oh => oh.Child)
                .Where(oh => oh.ParentId == parent.Id)
                .Select(oh => new OrganizationHierarchyTreeNode()
                {
                    Id = oh.Child.Id,
                    Name = oh.Child.LongName,
                    Name2 = oh.Child.LongName2
                })
                .ToListAsync();

            if (children.Count > 0)
            {
                parent.Children = children;

                foreach (var child in children)
                {
                    await BuildTree(child);
                }
            }
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