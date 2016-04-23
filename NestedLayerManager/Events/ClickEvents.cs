using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using MaxCustomControls;
using Autodesk.Max;
using System.Collections;
using BrightIdeasSoftware;
using NestedLayerManager.Events;
using NestedLayerManager.Filters;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.IO;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.Events.CustomArgs;

namespace NestedLayerManager.Events
{
    public static class ClickEvents
    {
        #region onCreateLayer

        public static void onCreateLayer(Object sender, ClickEventArgs e)
        {
            onCreateLayer(sender, e, false);
        }

        public static void onCreateLayerAddSelection(Object sender, ClickEventArgs e)
        {
            onCreateLayer(sender, e, true);
        }

        private static void onCreateLayer(Object sender, ClickEventArgs e, Boolean addSelection)
        {
            NlmTreeListView listView = e.ListView;

            listView.NodeControl.MaxEvents.LayerEvents.LayerCreated.UnregisterNotification();

            IILayer layer = MaxLayers.CreateLayer(true, addSelection);
            UIntPtr handle = MaxAnimatable.GetHandleByAnim(layer);

            LayerTreeNode layerTreeNode = new LayerTreeNode(handle, listView.NodeControl.HandleMap);

            // Get parent node.
            BaseTreeNode parentTreeNode = null;
            if (listView.SelectedObjects.Count > 0)
            {
                BaseTreeNode treeNode = listView.SelectedObjects[0] as BaseTreeNode;
                if (treeNode is FolderTreeNode)
                {
                    parentTreeNode = treeNode;
                }
                if (treeNode is LayerTreeNode)
                {
                    parentTreeNode = treeNode.Parent;
                }
            }

            // Add folder to listview, ensure is visible by expanding parents.
            listView.AddObject(layerTreeNode, parentTreeNode);
            if (parentTreeNode != null)
                listView.Expand(parentTreeNode);

            // Scroll to new item.
            Int32 parentIndex = listView.IndexOf(layerTreeNode);
            if (parentIndex != -1)
                listView.EnsureVisible(parentIndex);

            // Sort, select
            listView.Sort(listView.NlmColumns.NameColumn, SortOrder.Ascending);
            listView.SelectedObjects = new List<Object> { layerTreeNode };

            // Focus on the listview to ensure text entry goes to the edit box, and begin edit.
            listView.Focus();
            listView.EditModel(layerTreeNode);

            // Register notification.
            listView.NodeControl.MaxEvents.LayerEvents.LayerCreated.RegisterNotification();
        }

        #endregion

        #region onInstanceSelectedLayers

        public static void onInstanceSelectedLayers(Object sender, ClickEventArgs e)
        {
            NlmTreeListView listView = e.ListView;
            
            // Multiple items are being added, so let's use BeginUpdate.
            try
            {
                listView.BeginUpdate();

                foreach (BaseTreeNode treeNode in listView.SelectedObjects)
                {
                    LayerTreeNode layerTreeNode = treeNode as LayerTreeNode;
                    if (layerTreeNode != null)
                    {
                        LayerTreeNode newLayerTreeNode = new LayerTreeNode(layerTreeNode.Handle, listView.NodeControl.HandleMap);

                        foreach (ObjectTreeNode objectTreeNode in treeNode.Children)
                        {
                            ObjectTreeNode newObjectTreeNode = new ObjectTreeNode(objectTreeNode.Class, objectTreeNode.Handle, listView.NodeControl.HandleMap);
                            newObjectTreeNode.Parent = newLayerTreeNode;
                            newLayerTreeNode.Children.Add(newObjectTreeNode);
                        }

                        //foreach (LayerTreeNode instanceNode in layerTreeNode.Instances)
                        //{
                        //    instanceNode.Instances.Add(newLayerTreeNode);
                        //    newLayerTreeNode.Instances.Add(instanceNode);
                        //}
                        //layerTreeNode.Instances.Add(newLayerTreeNode);
                        //newLayerTreeNode.Instances.Add(layerTreeNode);

                        listView.AddObject(newLayerTreeNode, treeNode.Parent);
                    }
                }

                listView.Sort(listView.NlmColumns.NameColumn, SortOrder.Ascending);
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                listView.EndUpdate();
            }
        }

        #endregion

        public static void onMergeSelectedLayers(Object sender, ClickEventArgs e)
        { 
        }
        
        #region onCreateFolder

        public static void onCreateFolder(Object sender, ClickEventArgs e)
        {
            NlmTreeListView listView = e.ListView;

            // Calculate unique name in current level.
            IEnumerable<FolderTreeNode> folderNodes = listView.NodeControl.Query.FolderNodes;

            String folderName = "Folder";
            Int32 index = 1;
            while (folderNodes.Any(x => x.Name == folderName + index.ToString("000")))
            {
                index += 1;
            }
            folderName += index.ToString("000");
            
            // Create folder node.
            FolderTreeNode folderTreeNode = new FolderTreeNode(folderName);

            // Get parent node.
            BaseTreeNode parentTreeNode = null;
            if (listView.SelectedObjects.Count > 0)
            {
                BaseTreeNode treeNode = listView.SelectedObjects[0] as BaseTreeNode;
                if (treeNode is FolderTreeNode)
                {
                    parentTreeNode = treeNode;
                }
                if (treeNode is LayerTreeNode)
                {
                    parentTreeNode = treeNode.Parent;
                }
            }

            // Add folder to listview, ensure is visible by expanding parents.
            listView.AddObject(folderTreeNode, parentTreeNode);
            if (parentTreeNode != null) 
                listView.Expand(parentTreeNode);

            // Scroll to new item.
            Int32 parentIndex = listView.IndexOf(folderTreeNode);
            if (parentIndex != -1)
                listView.EnsureVisible(parentIndex);

            // Sort, select, and begin edit.
            listView.Sort(listView.NlmColumns.NameColumn, SortOrder.Ascending);
            listView.SelectedObjects = new List<Object> { folderTreeNode };
            listView.EditModel(folderTreeNode);
        }

        #endregion

        #region onCreateFolder

        public static void onCreateFolderAddSelection(Object sender, ClickEventArgs e)
        {
            NlmTreeListView listView = e.ListView;
            IEnumerable<BaseTreeNode> selection = listView.SelectedObjects.Cast<BaseTreeNode>();

            onCreateFolder(sender, e);

            FolderTreeNode folderTreeNode = listView.SelectedObject as FolderTreeNode;
            if (folderTreeNode != null)
            {
                IEnumerable<BaseTreeNode> layerFolderSelection = selection
                    .Where(x => x is LayerTreeNode || x is FolderTreeNode)
                    .Where(x => !folderTreeNode.IsAncestor(x));
                IEnumerable<BaseTreeNode> refreshNodes = layerFolderSelection
                    .Where(x => x.Parent != null)
                    .Select(x => x.Parent);

                listView.NodeControl.Parent.MoveTreeNodes(layerFolderSelection, folderTreeNode);
                listView.RefreshObjects(refreshNodes.ToList());
                listView.RefreshObject(folderTreeNode);
            }
        }

        #endregion

        public static void onMergeSelectedFolders(Object sender, ClickEventArgs e)
        {
        }

        public static void onDuplicateSelectionCopyCopy(Object sender, ClickEventArgs e)
        {
        }

        public static void onDuplicateSelectionCopyInstance(Object sender, ClickEventArgs e)
        {
        }

        public static void onDuplicateSelectionInstance(Object sender, ClickEventArgs e)
        {
        }

        #region onSmartFolderMode

        public static void onSmartFolderMode(Object sender, ClickEventArgs e)
        {
            NlmCheckButton button = sender as NlmCheckButton;
            e.ListView.SmartFolderMode = button.Checked;
            e.ListView.Refresh();
        }

        #endregion

        #region onIsolateSelection

        //TODO: 
        // Perhaps move this to a new class / node control. It's pretty huge.
        // Use MaxLayers static class to manipulate max objects instead of in code here.

        /// <summary>
        /// Isolate selected TreeNodes in Nested Layer Manager.
        /// Any TreeNode that is not isolated is disabled, and the respective node hidden in Max.
        /// </summary>
        /// <param name="sender">An NLMCheckButton that contains a checked property.</param>
        /// <param name="e">NLM Button Event Args, that contain a pointer to the listview.</param>
        public static void onIsolateSelection(Object sender, ClickEventArgs e)
        {
            // If the sender is not an NLMCheckButton, the request has come from somewhere else (right click or shortcut).
            // For this reason, change the check state of the button UI which will raise this event again.
            if (!(sender is NlmCheckButton))
            {
                e.ListView.ButtonPanelLeft.IsolateSelectedButton.Checked = !e.ListView.ButtonPanelLeft.IsolateSelectedButton.Checked;
                return;
            }

            NlmTreeListView listView = e.ListView;
            NlmCheckButton button = sender as NlmCheckButton;

            // Pause UI from refreshing.
            try
            {
                listView.BeginUpdate();

                // Isolate selected if button checked.
                if (button.Checked)
                {
                    // This method has some serious processing going on, so hashsets are used to maximise performance.
                    // Calculate all enabled nodes and disabled nodes.
                    HashSet<BaseTreeNode> enabledNodes = new HashSet<BaseTreeNode>(listView.NodeControl.Query.SelectionAndAllChildNodes);
                    HashSet<BaseTreeNode> disabledNodes = new HashSet<BaseTreeNode>(listView.NodeControl.Query.AllNodes.Where(x => !enabledNodes.Contains(x)));

                    // Disable disabled nodes.
                    listView.DisableObjects(disabledNodes);

                    // Now we need to work out what maxNodes and layers to temporarily hide.
                    // Firstly, get the highest selected objects.
                    IEnumerable<ObjectTreeNode> selectedNodes = listView.NodeControl.Query.SelectedAncestors
                        .Where(x => (x is ObjectTreeNode)).Cast<ObjectTreeNode>();
                    HashSet<IILayer> onLayers = new HashSet<IILayer>();

                    // If objects are selected, we need to ensure that the layer is not hidden.
                    // If any objects are selected, append the layer to the onLayers hashset.
                    foreach (ObjectTreeNode objectTreeNode in selectedNodes)
                    {
                        IINode maxNode = MaxAnimatable.GetAnimByHandle(objectTreeNode.Handle) as IINode;

                        if (maxNode == null)
                            continue;

                        IILayer layer = MaxLayers.GetLayer(maxNode);
                        if (!onLayers.Contains(layer))
                        {
                            onLayers.Add(layer);
                        }
                    }

                    // Ensure that the layer is on so that objects are visible.
                    // The layer is only turned on if an object is selected, and no ancestor is selected.
                    // For any object that is not selected, the object needs to be hidden.
                    foreach (IILayer layer in onLayers)
                    {
                        if (layer.IsHidden)
                        {
#if Max2013 || Max2014
                            layer.IsHidden = false;
#endif
#if Max2015
                            layer.Hide(false, false);
#endif
                        }

                        foreach (IINode maxNode in MaxLayers.GetChildNodes(layer))
                        {
                            UIntPtr maxNodeHandle = MaxAnimatable.GetHandleByAnim(maxNode);
                            if (!selectedNodes.Any(x => (x).Handle == maxNodeHandle) && !maxNode.IsObjectHidden)
                            {
                                listView.DisabledHandles.Add(maxNodeHandle);
                                maxNode.Hide(true);
                            }
                        }
                    }

                    // Loop through all layers that should be turned off.
                    // If the layer should be hidden (because it is not in onLayers or in enabledLayers), hide it.
                    IEnumerable<LayerTreeNode> enabledLayers = enabledNodes.Where(x => (x is LayerTreeNode)).Cast<LayerTreeNode>();
                    IEnumerable<IILayer> offLayers = MaxLayers.Layers.Where(x => !onLayers.Contains(x));
                    foreach (IILayer layer in offLayers)
                    {
                        UIntPtr layerHandle = MaxAnimatable.GetHandleByAnim(layer);
                        if (!layer.IsHidden && !enabledLayers.Any(x => (x).Handle == layerHandle))
                        {
#if Max2013 || Max2014
                            layer.IsHidden = true;
#endif
#if Max2015
                            layer.Hide(true, false);
#endif
                            listView.DisabledHandles.Add(layerHandle);
                        }
                    }

                }
                // Remove isolation and restore state if unchecked.
                else
                {
                    // Enable all objects.
                    listView.EnableObjects(listView.DisabledObjects);

                    // Turn on all handles that were turned off.
                    foreach (UIntPtr handle in listView.DisabledHandles)
                    {
                        IAnimatable maxAnim = MaxAnimatable.GetAnimByHandle(handle);
                        IILayer maxLayer = maxAnim as IILayer;
                        if (maxLayer != null)
                        {
#if Max2013 || Max2014
                            maxLayer.IsHidden = false;
#endif
#if Max2015
                            maxLayer.Hide(false, false);
#endif
                            continue;
                        }
                        IINode maxNode = maxAnim as IINode;
                        if (maxNode != null)
                        {
                            maxNode.Hide(false);
                            continue;
                        }
                    }

                    // Clear list of handles. 
                    listView.DisabledHandles.Clear();
                }
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                listView.EndUpdate();
            }
            MaxUI.RedrawViewportsNow();
        }

        #endregion

        #region onDeleteSelection

        public static void onDeleteSelection(Object sender, ClickEventArgs e)
        {
            e.ListView.NodeControl.Destroy.DeleteSelection();
            MaxUI.RedrawViewportsNow();
        }

        #endregion

        public static void onDeleteObjectsInSelection(Object sender, ClickEventArgs e)
        {
        }

        public static void onDeleteEmptyLayers(Object sender, ClickEventArgs e)
        {
        }

        public static void onDeleteEmptyFolders(Object sender, ClickEventArgs e)
        {
        }

        #region onAddSelectedObjectsToLayer

        public static void onAddSelectedObjectsToLayer(Object sender, ClickEventArgs e)
        {
            // TODO: 
            // This is quite slow compared to the NodeEventCallback LayerChanged. Look at that for tips.
            // Do we really need BeginUpdate and EndUpdate? Calculate which objects to refresh.
            // Also fix crappy bug where adding children to an expanded layer does not redraw properly.

            NlmTreeListView listView = e.ListView;
            try
            {
                listView.BeginUpdate();

                IList selection = listView.SelectedObjects;
                if (selection.Count == 1)
                {
                    LayerTreeNode layerTreeNode = selection[0] as LayerTreeNode;
                    if (layerTreeNode != null)
                    {
                        List<BaseTreeNode> moveTreeNodes = new List<BaseTreeNode>();

                        IAnimatable layerAnim = MaxAnimatable.GetAnimByHandle(layerTreeNode.Handle);
                        foreach (IINode maxNode in MaxNodes.SelectedNodes)
                        {
                            UIntPtr maxNodeHandle = MaxAnimatable.GetHandleByAnim(maxNode as IAnimatable);
                            List<BaseTreeNode> treeNodes = listView.NodeControl.HandleMap.GetTreeNodesByHandle(maxNodeHandle);
                            foreach (BaseTreeNode treeNode in treeNodes)
                            {
                                moveTreeNodes.Add(treeNode);
                            }
                            MaxNodes.MoveNodeToLayer(maxNode, layerAnim as IILayer);
                        }
                        listView.NodeControl.Parent.MoveTreeNodes(moveTreeNodes, layerTreeNode);
                    }
                }

                listView.Sort(listView.NlmColumns.NameColumn, SortOrder.Ascending);
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                e.ListView.EndUpdate();
            }
        }

        #endregion

        #region onSelectObjectsFromHighlight

        public static void onSelectObjectsFromHighlight(Object sender, ClickEventArgs e)
        {
            NlmTreeListView listView = e.ListView;

            MaxNodes.ClearNodeSelection();

            IEnumerable<UIntPtr> handles = listView.NodeControl.Query.SelectionAndAllChildObjectHandles;

            MaxNodes.SelectNodes(handles);        
        }

        #endregion

        #region onSelectLayersFromSelectedObjects

        public static void onSelectLayersFromSelectedObjects(Object sender, ClickEventArgs e)
        {
            NlmTreeListView listView = e.ListView;
            List<BaseTreeNode> newSelection = new List<BaseTreeNode>();

            foreach (IINode maxNode in MaxNodes.SelectedNodes)
            {
                UIntPtr maxNodeHandle = MaxAnimatable.GetHandleByAnim(maxNode as IAnimatable);
                List<BaseTreeNode> objectTreeNodes = listView.NodeControl.HandleMap.GetTreeNodesByHandle(maxNodeHandle);

                foreach (BaseTreeNode objectTreeNode in objectTreeNodes)
                {
                    BaseTreeNode layerTreeNode = objectTreeNode.Parent;
                    if (!newSelection.Contains(layerTreeNode))
                    {
                        newSelection.Add(layerTreeNode);
                    }
                    listView.NodeControl.CollapseExpand.ExpandAllParents(layerTreeNode);
                }
            }
            listView.SelectedObjects = newSelection;
        }

        #endregion

        #region onHideUnhideAll

        public static void onHideUnhideAll(Object sender, ClickEventArgs e)
        {
            try
            {
                e.ListView.BeginUpdate();
                MaxLayers.HideUnhideAll();
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                e.ListView.EndUpdate();
            }
        }

        #endregion

        #region onFreezeUnfreezeAll

        public static void onFreezeUnfreezeAll(Object sender, ClickEventArgs e)
        {
            try
            {
                e.ListView.BeginUpdate();
                MaxLayers.FreezeUnfreezeAll();
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                e.ListView.EndUpdate();
            }
        }

        #endregion

        #region onCollapseExpandAll

        public static void onCollapseExpandAll(Object sender, ClickEventArgs e)
        {
            try
            {
                e.ListView.BeginUpdate();
                e.ListView.NodeControl.CollapseExpand.CollapseExpandAll();
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                e.ListView.EndUpdate();
            }
        }

        #endregion


        #region onInformation

        public static void onInformation(Object sender, ClickEventArgs e)
        {
            // Show the info window.
            NlmAboutWindow infoWindow = new NlmAboutWindow(e.ListView.Parent);
            infoWindow.ShowDialog(e.ListView);
            
        }

        #endregion

        #region onSettings

        public static void onSettings(Object sender, ClickEventArgs e)
        {
            // Show the settings window.
            NlmSettingsWindow settingsWindow = new NlmSettingsWindow(e.ListView.Parent);
            settingsWindow.ShowDialog(e.ListView);
        }

        #endregion


        #region onFilterSelectAll

        public static void onFilterSelectAll(Object sender, ClickEventArgs e)
        {
            try
            {
                e.ListView.BeginUpdate();
                foreach (Object o in (sender as NlmButton).Parent.Controls)
                {
                    if (o is NlmCheckButton) ((NlmCheckButton)o).Checked = true;
                }
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                e.ListView.EndUpdate();
            }
        }

        #endregion

        #region onFilterSelectNone

        public static void onFilterSelectNone(Object sender, ClickEventArgs e)
        {
            try
            {
                e.ListView.BeginUpdate();
                foreach (Object o in (sender as NlmButton).Parent.Controls)
                {
                    if (o is NlmCheckButton) ((NlmCheckButton)o).Checked = false;
                }
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                e.ListView.EndUpdate();
            }
        }

        #endregion

        #region onFilterInvert

        public static void onFilterInvert(Object sender, ClickEventArgs e)
        {
            try
            {
                e.ListView.BeginUpdate();
                foreach (Object o in (sender as NlmButton).Parent.Controls)
                {
                    if (o is NlmCheckButton) ((NlmCheckButton)o).Checked = !((NlmCheckButton)o).Checked;
                }
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                e.ListView.EndUpdate();
            }
        }

        #endregion

        #region onFilterBones

        public static void onFilterBones(Object sender, ClickEventArgs e) 
        {
            NlmCheckButton button = sender as NlmCheckButton;
            NlmTreeNodeFilterEngine nlmTreeNodeFilter = e.ListView.ModelFilter as NlmTreeNodeFilterEngine;
            if (button.Checked)
            {
                nlmTreeNodeFilter.AddFilter(TreeNodeFilter.Bone);
            }
            else
            {
                nlmTreeNodeFilter.RemoveFilter(TreeNodeFilter.Bone);
            }
            e.ListView.ModelFilter = nlmTreeNodeFilter;
        }

        #endregion

        #region onFilterCamera

        public static void onFilterCamera(Object sender, ClickEventArgs e)
        {
            NlmCheckButton button = sender as NlmCheckButton;
            NlmTreeNodeFilterEngine nlmTreeNodeFilter = e.ListView.ModelFilter as NlmTreeNodeFilterEngine;
            if (button.Checked)
            {
                nlmTreeNodeFilter.AddFilter(TreeNodeFilter.Camera);
            }
            else
            {
                nlmTreeNodeFilter.RemoveFilter(TreeNodeFilter.Camera);
            }
            e.ListView.ModelFilter = nlmTreeNodeFilter;
        }

        #endregion

        #region onFilterHelper

        public static void onFilterHelper(Object sender, ClickEventArgs e)
        {
            NlmCheckButton button = sender as NlmCheckButton;
            NlmTreeNodeFilterEngine nlmTreeNodeFilter = e.ListView.ModelFilter as NlmTreeNodeFilterEngine;
            if (button.Checked)
            {
                nlmTreeNodeFilter.AddFilter(TreeNodeFilter.Helper);
            }
            else
            {
                nlmTreeNodeFilter.RemoveFilter(TreeNodeFilter.Helper);
            }
            e.ListView.ModelFilter = nlmTreeNodeFilter;
        }

        #endregion

        #region onFilterLight

        public static void onFilterLight(Object sender, ClickEventArgs e)
        {
            NlmCheckButton button = sender as NlmCheckButton;
            NlmTreeNodeFilterEngine nlmTreeNodeFilter = e.ListView.ModelFilter as NlmTreeNodeFilterEngine;
            if (button.Checked)
            {
                nlmTreeNodeFilter.AddFilter(TreeNodeFilter.Light);
            }
            else
            {
                nlmTreeNodeFilter.RemoveFilter(TreeNodeFilter.Light);
            }
            e.ListView.ModelFilter = nlmTreeNodeFilter;
        }

        #endregion

        #region onFilterObject

        public static void onFilterObject(Object sender, ClickEventArgs e)
        {
            NlmCheckButton button = sender as NlmCheckButton;
            NlmTreeNodeFilterEngine nlmTreeNodeFilter = e.ListView.ModelFilter as NlmTreeNodeFilterEngine;
            if (button.Checked)
            {
                nlmTreeNodeFilter.AddFilter(TreeNodeFilter.Object);
            }
            else
            {
                nlmTreeNodeFilter.RemoveFilter(TreeNodeFilter.Object);
            }
            e.ListView.ModelFilter = nlmTreeNodeFilter;
        }

        #endregion

        #region onFilterSpaceWarp

        public static void onFilterSpaceWarp(Object sender, ClickEventArgs e)
        {
            NlmCheckButton button = sender as NlmCheckButton;
            NlmTreeNodeFilterEngine nlmTreeNodeFilter = e.ListView.ModelFilter as NlmTreeNodeFilterEngine;
            if (button.Checked)
            {
                nlmTreeNodeFilter.AddFilter(TreeNodeFilter.SpaceWarp);
            }
            else
            {
                nlmTreeNodeFilter.RemoveFilter(TreeNodeFilter.SpaceWarp);
            }
            e.ListView.ModelFilter = nlmTreeNodeFilter;
        }

        #endregion

        #region onFilterShape

        public static void onFilterShape(Object sender, ClickEventArgs e)
        {
            NlmCheckButton button = sender as NlmCheckButton;
            NlmTreeNodeFilterEngine nlmTreeNodeFilter = e.ListView.ModelFilter as NlmTreeNodeFilterEngine;
            if (button.Checked)
            {
                nlmTreeNodeFilter.AddFilter(TreeNodeFilter.Shape);
            }
            else
            {
                nlmTreeNodeFilter.RemoveFilter(TreeNodeFilter.Shape);
            }
            e.ListView.ModelFilter = nlmTreeNodeFilter;
        }

        #endregion

        #region onFilterLayerHidden

        public static void onFilterLayerHidden(Object sender, ClickEventArgs e)
        {
            NlmCheckButton button = sender as NlmCheckButton;
            NlmTreeNodeFilterEngine nlmTreeNodeFilter = e.ListView.ModelFilter as NlmTreeNodeFilterEngine;
            if (button.Checked)
            {
                nlmTreeNodeFilter.AddFilter(TreeNodeFilter.LayerHidden);
            }
            else
            {
                nlmTreeNodeFilter.RemoveFilter(TreeNodeFilter.LayerHidden);
            }
            e.ListView.ModelFilter = nlmTreeNodeFilter;
        }

        #endregion

        #region onFilterLayerFrozen

        public static void onFilterLayerFrozen(Object sender, ClickEventArgs e)
        {
            NlmCheckButton button = sender as NlmCheckButton;
            NlmTreeNodeFilterEngine nlmTreeNodeFilter = e.ListView.ModelFilter as NlmTreeNodeFilterEngine;
            if (button.Checked)
            {
                nlmTreeNodeFilter.AddFilter(TreeNodeFilter.LayerFrozen);
            }
            else
            {
                nlmTreeNodeFilter.RemoveFilter(TreeNodeFilter.LayerFrozen);
            }
            e.ListView.ModelFilter = nlmTreeNodeFilter;
        }
  
        #endregion
    }
}
