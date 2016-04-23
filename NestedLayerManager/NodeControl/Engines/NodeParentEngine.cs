using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Autodesk.Max;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.Maps;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;

namespace NestedLayerManager.NodeControl.Engines
{
    public class NodeParentEngine
    {
        private NlmTreeListView ListView;

        public NodeParentEngine(NlmTreeListView listView)
        {
            ListView = listView;
        }

        public void AddChild(BaseTreeNode child, BaseTreeNode parent, Boolean refreshObject)
        {
            parent.Children.Add(child);
            child.Parent = parent;
            if (refreshObject)
                ListView.RefreshObject(parent);
        }

        public void MoveTreeNode(BaseTreeNode child, BaseTreeNode newParent)
        {
            MoveTreeNodes(new List<BaseTreeNode>() { child }, newParent);
        }

        // TODO: Change List to IEnumerable
        public void MoveTreeNodes(IEnumerable<BaseTreeNode> children, BaseTreeNode newParent)
        {
            // Removing root nodes is expensive.
            // In order to speed things up, build an array of root nodes to delete.
            List<BaseTreeNode> rootRemoveNodes = new List<BaseTreeNode>();
            List<BaseTreeNode> rootAddNodes = new List<BaseTreeNode>();
    
            foreach (BaseTreeNode child in children)
            {
                if (!(child.Parent == null && newParent == null))
                {
                    // If the parent is null it's a root node.
                    // If there is a parent, remove it from children.
                    if (child.Parent == null)
                    {
                        rootRemoveNodes.Add(child);
                    }
                    else
                    {
                        child.Parent.Children.Remove(child);
                    }
                    // Set the child to the new parent.
                    child.Parent = newParent;
                    // If the new parent is null, it should become a root node.
                    // If newParent is not null, add it to the children.
                    if (newParent == null)
                    {
                        rootAddNodes.Add(child);
                    }
                    else
                    {
                        newParent.Children.Add(child);
                    }
                }
            }
            // Finally, add all new root nodes to the listview and refresh nodes.
            if (rootRemoveNodes.Count > 0)
            {
                ListView.RemoveObjects(rootRemoveNodes);
            }
            if (rootAddNodes.Count > 0)
            {
                ListView.AddObjects(rootAddNodes);
            }
        }
    }
}
