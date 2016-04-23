using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.Maps;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.Nodes;

namespace NestedLayerManager.NodeControl.Engines
{
    public class NodeDeleteEngine
    {

        #region Properties

        private NlmTreeListView ListView;
        private NodeQueryEngine NodeQuery;
        private HandleMap HandleMap;

        #endregion

        #region Constructor

        public NodeDeleteEngine(NlmTreeListView listView, NodeQueryEngine nodeQuery, HandleMap handleMap)
        {
            ListView = listView;
            NodeQuery = nodeQuery;
            HandleMap = handleMap;
        }

        #endregion

        #region Deletion

        /// <summary>
        /// Clear all nodes from the scene tree.
        /// This is used for reset events.
        /// </summary>
        public void ClearSceneTree()
        {
            ListView.ClearObjects();
            ListView.ClearCachedInfo();
            HandleMap.ClearAll();
        }

        /// <summary>
        /// Deletes the selected treenodes, including associated max nodes.
        /// All children of selection are also deleted.
        /// Max nodes will not be deleted if not all instanced nodes are selected.
        /// If every layer 0 instance is selected, nothing will happen.
        /// </summary>
        public void DeleteSelection()
        {

#if DEBUG
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
#endif

            ListView.NodeControl.MaxEvents.NodeEvents.Unregister();
            ListView.NodeControl.MaxEvents.LayerEvents.LayerDeleted.UnregisterNotification();

            // Don't do anything if every single layer 0 is selected.
            IILayer layer0 = MaxLayers.GetLayer(0);
            UIntPtr layer0handle = MaxAnimatable.GetHandleByAnim(layer0);
            List<BaseTreeNode> layerTreeNodes = HandleMap.GetTreeNodesByHandle(layer0handle);

            if (layerTreeNodes.All(x => NodeQuery.SelectionAndAllChildNodes.Contains(x)))
                return;

            // Collect the selection IEnumerables to use.
            HashSet<BaseTreeNode> selectionAndAllChildren = new HashSet<BaseTreeNode>
                (NodeQuery.SelectionAndAllChildNodes);

            // Calculate layer and object handles.
            IEnumerable<UIntPtr> objectHandles = selectionAndAllChildren.Where
                (x => x is ObjectTreeNode).Cast<ObjectTreeNode>().Select(x => x.Handle);
            IEnumerable<UIntPtr> layerHandles = selectionAndAllChildren.Where
                (x => x is LayerTreeNode).Cast<LayerTreeNode>().Select(x => x.Handle);

            // Delete handles from max.
            foreach (UIntPtr handle in objectHandles)
            {
                List<BaseTreeNode> instances = HandleMap.GetTreeNodesByHandle(handle);
                if (instances.Count() > 1)
                    if (!instances.All(x => selectionAndAllChildren.Contains(x)))
                        continue;
                MaxNodes.DeleteNode(handle);
            }
            foreach (UIntPtr handle in layerHandles)
            {
                List<BaseTreeNode> instances = HandleMap.GetTreeNodesByHandle(handle);
                if (instances.Count() > 1)
                    if (!instances.All(x => selectionAndAllChildren.Contains(x)))
                        continue;
                MaxLayers.DeleteLayer(handle);
            }

            // And now to delete the tree nodes now there are no max nodes.
            DeleteTreeNodes(selectionAndAllChildren);

            // The default behaviour of the listview is to maintain selection based on item index.
            // This is not very desirable, as the selection should be nothing.
            ListView.SelectedObjects = new Object[]{};

            ListView.NodeControl.MaxEvents.LayerEvents.LayerDeleted.RegisterNotification();
            ListView.NodeControl.MaxEvents.NodeEvents.Register();

#if DEBUG
            stopwatch.Stop();
            MaxListener.PrintToListener("DeleteSelection completed in: " + stopwatch.ElapsedMilliseconds);
#endif
        }

        /// <summary>
        /// Fires DeleteTreeNodes for provided node. 
        /// Please see DeleteTreeNodes for more info.
        /// </summary>
        /// <param name="treeNode"></param>
        public void DeleteTreeNode(BaseTreeNode treeNode)
        {
            DeleteTreeNodes( new List<BaseTreeNode> { treeNode } );
        }

        /// <summary>
        /// Deletes the selected treenodes, but does not delete associated max nodes.
        /// All children need to be provided, otherwise the child nodes will not be removed from the HandleMap.
        /// </summary>
        /// <param name="treeNodes"></param>
        public void DeleteTreeNodes(IEnumerable<BaseTreeNode> treeNodes)
        {

#if DEBUG
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
#endif

            List<BaseTreeNode> rootNodes = new List<BaseTreeNode>();
            HashSet<BaseTreeNode> refreshNodes = new HashSet<BaseTreeNode>();

            // Delete nodes in NLM
            foreach (BaseTreeNode treeNode in treeNodes)
            {
                // Instead of removing each branch node independently, append to array.
                // We can then remove them all at the same time, which is way quicker.
                if (treeNode.Parent == null)
                {
                    rootNodes.Add(treeNode);
                }
                else
                {
                    treeNode.Parent.Children.Remove(treeNode);
                    // Instead of refreshing each node independently, append to array.
                    // We can then refresh all objects at the same time which is way quicker.
                    if (!refreshNodes.Contains(treeNode.Parent) && !treeNodes.Any(x => treeNode.IsAncestor(x)))
                        refreshNodes.Add(treeNode.Parent);
                    treeNode.Parent = null;
                }

                // Remove anim handle / treenode link from map.
                // Note:
                // Using if (x is y) is creating a cast, which is doubling the casting.
                // Instead, casting once and checking for null is used.
                // Continue is used to avoid unnecessary casting.
                ObjectTreeNode objectTreeNode = treeNode as ObjectTreeNode;
                if (objectTreeNode != null)
                {
                    HandleMap.RemoveTreeNodeFromHandle(objectTreeNode, objectTreeNode.Handle);
                    continue;
                }
                LayerTreeNode layerTreeNode = treeNode as LayerTreeNode;
                if (layerTreeNode != null)
                {
                    HandleMap.RemoveTreeNodeFromHandle(layerTreeNode, layerTreeNode.Handle);
                    continue;
                }
  
            }

            // Work through the appended arrays and remove / refresh.
            ListView.RemoveObjects(rootNodes);
            ListView.RefreshObjects(refreshNodes.ToList());

#if DEBUG
            stopwatch.Stop();
            MaxListener.PrintToListener("DeleteTreeNodes completed in: " + stopwatch.ElapsedMilliseconds);
#endif

        }

        #endregion
    }
}
