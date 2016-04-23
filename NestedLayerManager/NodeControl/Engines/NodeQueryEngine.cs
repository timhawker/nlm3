using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.MaxInteractivity.MaxAnims;

namespace NestedLayerManager.NodeControl.Engines
{
    public class NodeQueryEngine
    {
        public NlmTreeListView ListView;

        public NodeQueryEngine(NlmTreeListView listView)
        {
            ListView = listView;
        }

        #region Selection Query

        /// <summary>
        /// Gets all selected BaseTreeNode objects in listview.
        /// </summary>
        public IEnumerable<BaseTreeNode> SelectedNodes
        {
            get
            {
                return ListView.SelectedObjects.Cast<BaseTreeNode>();
            }
        }

        /// <summary>
        /// Gets all top level selected BaseTreeNode objects. Only the highest selected Ancestors are returned.
        /// </summary>
        public IEnumerable<BaseTreeNode> SelectedAncestors
        {
            get
            {
                IEnumerable<BaseTreeNode> selection = SelectedNodes;
                foreach (BaseTreeNode baseTreeNode in selection)
                {
                    if (!IsAncestorSelected(baseTreeNode))
                    {
                        yield return baseTreeNode;
                    }
                }
            }
        }

        /// <summary>
        /// Returns True if an ancestor is Selected. Returns False if an ancestor is not selected.
        /// </summary>
        public Boolean IsAncestorSelected(BaseTreeNode treeNode)
        {
            BaseTreeNode parent = treeNode.Parent;
            while (parent != null)
            {
                if (ListView.IsSelected(parent))
                {
                    return true;
                }
                parent = parent.Parent;
            }
            return false;
        }

        /// <summary>
        /// Gets selected BaseTreeNode objects, including all children of selected BaseTreeNode objects.
        /// </summary>
        public IEnumerable<BaseTreeNode> SelectionAndAllChildNodes
        {
            get
            {
                return SelectedAncestors.SelectMany(GetAllChildren);
            }
        }

        /// <summary>
        /// Gets all ObjectTreeNode Handles of all selected ObjectTreeNode objects, including all children of selection.
        /// </summary>
        public IEnumerable<UIntPtr> SelectionAndAllChildObjectHandles
        {
            get
            {
                return SelectedAncestors.SelectMany(GetAllChildObjectHandles);
            }
        }

        #endregion

        #region Node Query

        /// <summary>
        /// Gets all root BaseTreeNode objects in listview.
        /// </summary>
        public IEnumerable<BaseTreeNode> RootNodes
        {
            get
            {
                return ListView.Roots.Cast<BaseTreeNode>();
            }
        }

        /// <summary>
        /// Gets all BaseTreeNode objects in listview.
        /// </summary>
        public IEnumerable<BaseTreeNode> AllNodes
        {
            get
            {
                return RootNodes.SelectMany(GetAllChildren);
            }
        }

        /// <summary>
        /// Gets all BaseTreeNode objects in listview.
        /// </summary>
        public IEnumerable<LayerTreeNode> LayerNodes
        {
            get
            {
                return RootNodes.SelectMany(GetAllChildLayerNodes);
            }
        }

        /// <summary>
        /// Gets all BaseTreeNode objects in listview.
        /// </summary>
        public IEnumerable<FolderTreeNode> FolderNodes
        {
            get
            {
                return RootNodes.SelectMany(GetAllChildFolderNodes);
            }
        }

        #endregion

        #region Recursive Linq Methods

        /// <summary>
        /// Returns all children of BaseTreeNode, including node itself. 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerable<BaseTreeNode> GetAllChildren(BaseTreeNode node)
        {
            yield return node;

            foreach (BaseTreeNode child in node.Children)
                foreach (BaseTreeNode childChild in GetAllChildren(child))
                    yield return childChild;
        }

        /// <summary>
        /// Returns all children of provided BaseTreeNode that are LayerTreeNode objects, including node itself.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerable<LayerTreeNode> GetAllChildLayerNodes(BaseTreeNode node)
        {
            if (node is LayerTreeNode)
                yield return node as LayerTreeNode;

            foreach (BaseTreeNode child in node.Children)
                foreach (BaseTreeNode childChild in GetAllChildren(child))
                    if (childChild is LayerTreeNode)
                        yield return childChild as LayerTreeNode;
        }

        /// <summary>
        /// Returns all children of provided BaseTreeNode that are FolderTreeNode objects, including node itself.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerable<FolderTreeNode> GetAllChildFolderNodes(BaseTreeNode node)
        {
            if (node is FolderTreeNode)
                yield return node as FolderTreeNode;

            foreach (BaseTreeNode child in node.Children)
                foreach (BaseTreeNode childChild in GetAllChildren(child))
                    if (childChild is FolderTreeNode)
                        yield return childChild as FolderTreeNode;
        }

        /// <summary>
        /// Returns all child Anim Handles of provided BaseTreeNode that are an ObjectTreeNode, including node itself.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerable<UIntPtr> GetAllChildObjectHandles(BaseTreeNode node)
        {
            if (node is ObjectTreeNode)
                yield return (node as ObjectTreeNode).Handle;

            foreach (BaseTreeNode child in node.Children)
                foreach (BaseTreeNode childChild in GetAllChildren(child))
                    if (childChild is ObjectTreeNode)
                        yield return (childChild as ObjectTreeNode).Handle;
        }

        #endregion
    }
}
