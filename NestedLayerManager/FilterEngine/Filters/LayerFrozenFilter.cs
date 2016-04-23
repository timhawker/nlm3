using System;
using System.Collections.Generic;
using System.Linq;
using BrightIdeasSoftware;
using System.Windows.Forms;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.Interfaces;

namespace NestedLayerManager.Filters
{
    // Layer frozen filter, which is added to NlmNodeFilter when required.
    public class LayerFrozenFilter : BaseTreeNodeFilter
    {
        public LayerFrozenFilter(TreeNodeFilter treeNodeFilter)
        {
            TreeNodeFilter = treeNodeFilter;
        }

        public override Boolean Filter(Object treeNode)
        {
            LayerTreeNode layerTreeNode = treeNode as LayerTreeNode;
            if (layerTreeNode != null)
            {
                if (layerTreeNode.Freeze == false) return false;
            }
            return true;
        }
    }
}
