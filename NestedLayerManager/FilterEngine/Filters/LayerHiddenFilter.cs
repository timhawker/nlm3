using System;
using System.Collections.Generic;
using System.Linq;
using BrightIdeasSoftware;
using System.Windows.Forms;
using NestedLayerManager.Nodes;
using NestedLayerManager.Interfaces;

namespace NestedLayerManager.Filters
{
    // Layer hidden filter, which is added to NlmNodeFilter when required.
    public class LayerHiddenFilter : BaseTreeNodeFilter
    {
        public LayerHiddenFilter(TreeNodeFilter treeNodeFilter)
        {
            TreeNodeFilter = treeNodeFilter;
        }

        public override Boolean Filter(Object treeNode)
        {
            LayerTreeNode layerTreeNode = treeNode as LayerTreeNode;
            if (layerTreeNode != null)
            {
                if (layerTreeNode.Visible == false) return false;
            }
            return true;
        }
    }
}
