using System;
using System.Collections.Generic;
using System.Linq;
using BrightIdeasSoftware;
using System.Windows.Forms;
using NestedLayerManager.Nodes;
using NestedLayerManager.Interfaces;

namespace NestedLayerManager.Filters
{
    // Node class filter, which is added to NlmNodeFilter when required.
    public class NodeClassFilter : BaseTreeNodeFilter
    {
        public ObjectTreeNode.ObjectClass ObjectClass;

        internal NodeClassFilter(ObjectTreeNode.ObjectClass objectClass, TreeNodeFilter treeNodeFilter)
            : base()
        {
            ObjectClass = objectClass;
            TreeNodeFilter = treeNodeFilter;
        }

        public override Boolean Filter(Object treeNode)
        {
            ObjectTreeNode objectTreeNode = treeNode as ObjectTreeNode;
            if (objectTreeNode != null)
            {
                if (objectTreeNode.Class == ObjectClass) return false;
            }
            return true;
        }
    }
}
