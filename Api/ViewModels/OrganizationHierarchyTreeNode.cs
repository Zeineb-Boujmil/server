using System;
using System.Collections.Generic;

namespace Api.ViewModels
{
    class OrganizationHierarchyTreeNode
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public IEnumerable<OrganizationHierarchyTreeNode> Children { get; set; }
    }
}