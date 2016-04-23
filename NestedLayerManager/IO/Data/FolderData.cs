using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using NestedLayerManager.SubControls;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.Nodes;

namespace NestedLayerManager.IO.Data
{
    [Serializable]
    public class FolderData
    {
        public Guid ID;
        public Guid ParentID;

        public String Name;
        public Boolean? Visible;
        public Boolean? Current;
        public Boolean? Freeze;
        public Boolean? Render;
        public Color? Color;
        public Boolean? Box;
        public Boolean Expanded;

        public FolderData(FolderTreeNode treeNode, NlmTreeListView owner)
        {
            // Node Properties
            ID = treeNode.ID;
            if (treeNode.Parent != null) ParentID = treeNode.Parent.ID;
            Expanded = owner.IsExpanded(treeNode);

            // Column Properties
            Name = treeNode.Name;
            Visible = treeNode.Visible;
            Freeze = treeNode.Freeze;
            Render = treeNode.Render;
            Color = treeNode.Color;
            Box = treeNode.Box;
        }
    }
}
