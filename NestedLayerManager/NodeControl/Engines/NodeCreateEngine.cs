using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;
using System.Windows.Forms;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.Maps;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.IO.Data;

namespace NestedLayerManager.NodeControl.Engines
{
    public class NodeCreateEngine
    {
        private NlmTreeListView ListView;
        private HandleMap HandleMap;

        public NodeCreateEngine(NlmTreeListView listView, HandleMap handleMap)
        {
            ListView = listView;
            HandleMap = handleMap;
        }

        #region Initilise Scene Tree

        public void BuildSceneTree()
        {
            // Create list of tree nodes to append to control.
            // Create folder node hashtable and use ID as key.
            // The hashtable is used to reparent nodes using stored parent ID's.
            List<BaseTreeNode> treeNodeList = new List<BaseTreeNode>();
            Hashtable folderNodeIdMap = new Hashtable();
            List<FolderData> folderNodeDataList = MaxIO.LoadFolderRootNodeData();

            // These methods add the nodes to the tree node list.
            if (folderNodeDataList != null)
            {
                // We only add folder nodes if the data list obtained is not null.
                BuildFolderNodes(treeNodeList, folderNodeDataList, folderNodeIdMap);
            }
            BuildLayerAndObjectNodes(treeNodeList, MaxLayers.Layers, folderNodeIdMap);
            
            // Finally, add the list to the listview.
            ListView.Roots = treeNodeList;
            ListView.NodeControl.CollapseExpand.ExpandWasExpanded(treeNodeList);

            // And now sort :)
            ListView.Sort(ListView.NlmColumns.NameColumn, SortOrder.Ascending);
        }

        public void BuildFolderNodes(List<BaseTreeNode> treeNodeList, List<FolderData> folderNodeDataList, Hashtable folderNodeIdMap)
        {
            // We only add the folder nodes that are not already existing in the treelist.
            // We do this by checking if the hashtable already contains the key.
            // If not present, we build the tree nodes and append to hashtable with the ID as key.
            List<FolderData> folderNodesToAdd = new List<FolderData>();
            foreach (FolderData folderNodeData in folderNodeDataList)
            {
                if (!folderNodeIdMap.Contains(folderNodeData.ID))
                {
                    FolderTreeNode folderTreeNode = new FolderTreeNode(folderNodeData);
                    folderNodeIdMap.Add(folderNodeData.ID, folderTreeNode);
                    folderNodesToAdd.Add(folderNodeData);
                }
            }
            // Loop through folder node data and build folder tree nodes.
            foreach (FolderData folderNodeData in folderNodesToAdd)
            {
                FolderTreeNode node = folderNodeIdMap[folderNodeData.ID] as FolderTreeNode;
                // If folderNodeData does not have a parent ID,
                // it should be appended as a root node.
                if (folderNodeData.ParentID == Guid.Empty)
                {
                    treeNodeList.Add(node);
                }
                // If not, it should be parented to an existing treeNode,
                // as long as it exists.
                else
                {
                    FolderTreeNode parent = folderNodeIdMap[folderNodeData.ParentID] as FolderTreeNode;
                    if (parent == null)
                    {
                        treeNodeList.Add(node);
                    }
                    else
                    {
                        node.Parent = parent;
                        parent.Children.Add(node);
                    }
                }
            }
        }

        public void BuildLayerAndObjectNodes(List<BaseTreeNode> treeNodeList, IEnumerable<IILayer> layers, Hashtable folderNodeIdMap)
        {
            foreach (IILayer layer in layers)
            {
                UIntPtr handle = MaxAnimatable.GetHandleByAnim(layer);
                LayerData layerNodeData = MaxIO.LoadLayerData(layer);

                // If layer has node data, create treeNode based on that.
                // If not, create a new treeNode and append to root.
                LayerTreeNode layerTreeNode;
                if (layerNodeData == null)
                {
                    layerTreeNode = new LayerTreeNode(handle, HandleMap);
                    BuildChildObjects(layerTreeNode, layer);
                    treeNodeList.Add(layerTreeNode);
                }
                else
                {
                    List<Guid> layerTreeNodeParentIDs = layerNodeData.ParentIDs;
                    foreach (Guid layerTreeNodeParentID in layerTreeNodeParentIDs)
                    {
                        // If the handle already exists it is an instance.
                        // Populate the instance properties on all layers with that handle.
                        if (HandleMap.ContainsHandle(handle))
                        {
                            layerTreeNode = new LayerTreeNode(layerNodeData, handle, HandleMap);
                        }
                        else
                        {
                            layerTreeNode = new LayerTreeNode(layerNodeData, handle, HandleMap);
                        }

                        // If folderNodeData does not have a parent ID,
                        // it should be appended as a root node.
                        if (layerTreeNodeParentID == Guid.Empty)
                        {
                            treeNodeList.Add(layerTreeNode);
                        }
                        // If not, it should be parented to an existing treeNode,
                        // as long as it exists.
                        else
                        {
                            FolderTreeNode parent = folderNodeIdMap[layerTreeNodeParentID] as FolderTreeNode;
                            if (parent == null)
                            {
                                treeNodeList.Add(layerTreeNode);
                            }
                            else
                            {
                                layerTreeNode.Parent = parent;
                                parent.Children.Add(layerTreeNode);
                            }
                        }

                        // Add objects to layer.
                        BuildChildObjects(layerTreeNode, layer);
                    }
                }
            }
        }

        private void BuildChildObjects(LayerTreeNode layerTreeNode, IILayer maxLayer)
        {
            IEnumerable<IINode> layerNodes = MaxLayers.GetChildNodes(maxLayer);
            foreach (IINode maxNode in layerNodes)
            {
                UIntPtr iNodeHandle = MaxAnimatable.GetHandleByAnim(maxNode);
                ObjectTreeNode objectTreeNode = new ObjectTreeNode(MaxNodes.GetObjectClass(maxNode), iNodeHandle, HandleMap);
                ListView.NodeControl.Parent.AddChild(objectTreeNode, layerTreeNode, false);
            }
        }

        public void AddMissingChildObjects(IEnumerable<LayerTreeNode> layerNodes)
        {
            foreach (LayerTreeNode layerNode in layerNodes)
            {
                IILayer layer = MaxAnimatable.GetAnimByHandle(layerNode.Handle) as IILayer;
                IEnumerable<IINode> maxNodes = MaxLayers.GetChildNodes(layer);
                foreach (IINode maxNode in maxNodes)
                {
                    UIntPtr iNodeHandle = MaxAnimatable.GetHandleByAnim(maxNode);
                    // TODO: This needs to work with instanced layers.
                    if (!HandleMap.ContainsHandle(iNodeHandle))
                    {
                        ObjectTreeNode objectTreeNode = new ObjectTreeNode(MaxNodes.GetObjectClass(maxNode), iNodeHandle, HandleMap);
                        objectTreeNode.Parent = layerNode;
                        layerNode.Children.Add(objectTreeNode);
                    }
                }
            }
        }

        #endregion
    }
}
