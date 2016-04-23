using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using NestedLayerManager.SubControls;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.Nodes;
using NestedLayerManager.Maps;

namespace NestedLayerManager.IO.Data
{
    //This is an example of how the saved data can be expanded. It is not yet implemented anywhere.
    //[Serializable]
    //private class LayerDataExpansion2014 : LayerData
    //{
    //    public Boolean SomeValue;

    //    public LayerDataExpansion2014(LayerTreeNode treeNode, NlmTreeListView owner)
    //        : base(treeNode, owner)
    //    { }
    //}

    [Serializable]
    public class LayerData
    {
        public Guid ID;
        public List<Guid> ParentIDs;

        public Boolean? Visible;
        public Boolean? Current;
        public Boolean? Freeze;
        public Boolean? Render;
        public Color? Color;
        public Boolean? Box;
        public Boolean Expanded;

        public LayerData(LayerTreeNode treeNode, NlmTreeListView listView, HandleMap handleMap)
        {
            ParentIDs = new List<Guid>();

            // Node Properties
            ID = treeNode.ID;
            if (treeNode.Parent == null)
                ParentIDs.Add(new Guid());
            else
                ParentIDs.Add(treeNode.Parent.ID);

            foreach (LayerTreeNode instanceParent in handleMap.GetTreeNodesByHandle(treeNode.Handle))
            {
                if (instanceParent != treeNode)
                {
                    if (instanceParent.Parent == null)
                        ParentIDs.Add(new Guid());
                    else
                        ParentIDs.Add(instanceParent.Parent.ID);
                }
            }

            // Column Properties
            Visible = treeNode.Visible;
            Freeze = treeNode.Freeze;
            Render = treeNode.Render;
            Box = treeNode.Box;
            Expanded = listView.IsExpanded(treeNode);
        }
    }
}
