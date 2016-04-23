using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;
using NestedLayerManager.Nodes.Base;

namespace NestedLayerManager.Interfaces
{
    public interface IAspectEngine
    {
        Object GetTreeNodeValue(BaseTreeNode treeNode);
        void PutTreeNodeValue(BaseTreeNode treeNode, Object newValue);

        Object GetMaxValue(BaseTreeNode treeNode);
        void PutMaxValue(BaseTreeNode treeNode, Object newValue);

        Object GetAspect(Object rowObject);
        void PutAspect(Object rowObject, Object newValue);

        Boolean? getFolderCheckState(BaseTreeNode treeNode);
        Boolean IsSubItemUnchecked(Object rowObject);
        void ToggleCheckState(Object rowObject);
        void ToggleCheckStates(Object rowObject);
    }
}
