using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NestedLayerManager.SubControls;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;

namespace NestedLayerManager.NodeControl.Engines
{
    public class NodeCollapseExpandEngine
    {
        NlmTreeListView ListView;
        NodeQueryEngine NodeQuery;

        public NodeCollapseExpandEngine(NlmTreeListView listView, NodeQueryEngine nodeQuery)
        {
            ListView = listView;
            NodeQuery = nodeQuery;

            ListView.CanExpandGetter = delegate(Object treeNode)
            {
                return (treeNode as BaseTreeNode).Children.Count > 0;
            };

            ListView.ChildrenGetter = delegate(Object treeNode)
            {
                return (treeNode as BaseTreeNode).Children;
            };
        }

        public void ExpandAllParents(BaseTreeNode treeNode)
        {
            while (treeNode.Parent != null)
            {
                ListView.Expand(treeNode.Parent);
                treeNode = treeNode.Parent;
            }
        }

        public void ExpandWasExpanded(List<BaseTreeNode> treeNodes)
        {
            foreach (BaseTreeNode treeNode in treeNodes)
            {
                if (treeNode.WasExpanded)
                {
                    ListView.Expand(treeNode);
                    if (treeNode.Children.Count > 0)
                    {
                        ExpandWasExpanded(treeNode.Children);
                    }
                }
            }
        }

        public void CollapseExpandAll()
        {
            if (NodeQuery.AllNodes.Any(x => ListView.IsExpanded(x)))
            {
                ListView.CollapseAll();
            }
            else
            {
                foreach (FolderTreeNode folderTreeNode in NodeQuery.FolderNodes)
                {
                    ListView.Expand(folderTreeNode);
                }
            }
        }
    }
}
