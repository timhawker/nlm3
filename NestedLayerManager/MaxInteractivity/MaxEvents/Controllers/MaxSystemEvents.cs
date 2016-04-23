using System;
using System.Linq;
using Autodesk.Max;
using ManagedServices;
using System.Collections;
using System.Collections.Generic;
using NestedLayerManager.SubControls;
using NestedLayerManager.IO;
using NestedLayerManager.IO.Data;
using NestedLayerManager.NodeControl;
using NestedLayerManager.Maps;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.Events;
using NestedLayerManager.MaxInteractivity.MaxEvents.Handlers;
using NestedLayerManager.MaxInteractivity.MaxEvents.Args;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;



namespace NestedLayerManager.MaxInteractivity.MaxEvents.Controllers
{
    public class MaxSystemEvents : IDisposable
    {
        #region Properties

        private NlmTreeListView ListView;
        private NodeController NodeControl;

        public SystemNotificationObjectHandler FilePreSave;
        public SystemNotificationObjectHandler SystemPreReset;
        public SystemNotificationObjectHandler SystemPostReset;
        public SystemNotificationObjectHandler SystemPreNew;
        public SystemNotificationObjectHandler SystemPostNew;
        public SystemNotificationObjectHandler SystemPreLoad;
        public SystemNotificationObjectHandler SystemPostLoad;
        public SystemNotificationObjectHandler SystemPostMerge;

        #endregion

        #region Constructor

        public MaxSystemEvents(NlmTreeListView listView, NodeController nodeControl)
        {
            // Set properties.
            ListView = listView;
            NodeControl = nodeControl;

            // Create Events
            FilePreSave = new SystemNotificationObjectHandler(SystemNotificationCode.FilePreSave);
            FilePreSave.NotificationRaised += new EventHandler<SystemNotificationObjectEventArgs>(onFilePreSave);

            SystemPreReset = new SystemNotificationObjectHandler(SystemNotificationCode.SystemPreReset);
            SystemPreReset.NotificationRaised += new EventHandler<SystemNotificationObjectEventArgs>(onSystemPreReset);

            SystemPostReset = new SystemNotificationObjectHandler(SystemNotificationCode.SystemPostReset);
            SystemPostReset.NotificationRaised += new EventHandler<SystemNotificationObjectEventArgs>(onSystemPostReset);

            SystemPreNew = new SystemNotificationObjectHandler(SystemNotificationCode.SystemPreNew);
            SystemPreNew.NotificationRaised += new EventHandler<SystemNotificationObjectEventArgs>(onSystemPreReset);

            SystemPostNew = new SystemNotificationObjectHandler(SystemNotificationCode.SystemPostNew);
            SystemPostNew.NotificationRaised += new EventHandler<SystemNotificationObjectEventArgs>(onSystemPostReset);

            SystemPreLoad = new SystemNotificationObjectHandler(SystemNotificationCode.FilePreOpen);
            SystemPreLoad.NotificationRaised += new EventHandler<SystemNotificationObjectEventArgs>(onSystemPreReset);

            SystemPostLoad = new SystemNotificationObjectHandler(SystemNotificationCode.FilePostOpen);
            SystemPostLoad.NotificationRaised += new EventHandler<SystemNotificationObjectEventArgs>(onSystemPostReset);

            SystemPostMerge = new SystemNotificationObjectHandler(SystemNotificationCode.FilePostMerge);
            SystemPostMerge.NotificationRaised += new EventHandler<SystemNotificationObjectEventArgs>(onSystemPostMerge);

            // import, dunno if we need that one
            // xref
        }

        #endregion

        #region Events

        private void onFilePreSave(Object sender, SystemNotificationObjectEventArgs e)
        {
#if DEBUG
            MaxListener.PrintToListener("onFilePreSave");
#endif
            MaxIO.SaveData(ListView, NodeControl);
        }

        private void onSystemPreReset(Object sender, SystemNotificationObjectEventArgs e)
        {
#if DEBUG
            MaxListener.PrintToListener("onSystemPreReset");
#endif
            NodeControl.Destroy.ClearSceneTree();
        }

        private void onSystemPostReset(Object sender, SystemNotificationObjectEventArgs e)
        {
#if DEBUG
            MaxListener.PrintToListener("onSystemPostReset");
#endif
            try
            {
                ListView.BeginUpdate();
                NodeControl.Create.BuildSceneTree();
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                ListView.EndUpdate();
            }
        }

        private void onSystemPostMerge(Object sender, SystemNotificationObjectEventArgs e)
        {
#if DEBUG
            MaxListener.PrintToListener("onSystemPostMerge");
#endif
            try
            {
                ListView.BeginUpdate();

                // Collect existing layer and folder nodes.
                IEnumerable<FolderTreeNode> existingFolderNodes = NodeControl.Query.FolderNodes;
                IEnumerable<LayerTreeNode> existingLayerNodes = NodeControl.Query.LayerNodes;

                // Unfortunately no event is fired for nodes that are added to layers that already exist.
                // For this reason we need to refresh the children in every layer.
                NodeControl.Create.AddMissingChildObjects(existingLayerNodes);
                ListView.RefreshObjects(existingLayerNodes.ToList());

                // After merge, get all new layers that need to be added to NLM.
                // Old layers are collected because no notification happens for nodes added to existing layers :/
                List<IILayer> newLayers = new List<IILayer>();
                foreach (IILayer layer in MaxLayers.Layers)
                {
                    UIntPtr handle = MaxAnimatable.GetHandleByAnim(layer as IAnimatable);
                    if (!NodeControl.HandleMap.ContainsHandle(handle))
                    {
                        newLayers.Add(layer);
                    }
                }

                // Check each new layer for any folder data saved on the attribute.
                // If any data exists, append it ot folderDataList.
                List<BaseTreeNode> treeNodeList = new List<BaseTreeNode>();
                List<FolderData> folderDataList = new List<FolderData>();
                foreach (IILayer layer in newLayers)
                {
                    List<FolderData> parentFolders = MaxIO.LoadParentFolderData(layer as IAnimatable, ListView);
                    if (parentFolders != null)
                    {
                        foreach (FolderData folderData in parentFolders)
                        {
                            if (!folderDataList.Contains(folderData))
                                folderDataList.Add(folderData);
                        }
                    }
                }

                // We only add new folders, so build a hashtable of already existing folder nodes.
                // This hashtable is passed to the Build methods, which checks before creating a new node.
                Hashtable folderNodeIdMap = new Hashtable();
                foreach (FolderTreeNode folderNode in existingFolderNodes)
                {
                    folderNodeIdMap.Add(folderNode.ID, folderNode);
                }

                // Build new folder and layer nodes, and append them to the treeNodeList.
                NodeControl.Create.BuildFolderNodes(treeNodeList, folderDataList, folderNodeIdMap);
                NodeControl.Create.BuildLayerAndObjectNodes(treeNodeList, newLayers, folderNodeIdMap);

                // Finally, add the new objects.
                ListView.AddObjects(treeNodeList);
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                ListView.EndUpdate();
            }
        }

        #endregion

        #region Disposing

        public void Dispose()
        {
            FilePreSave.Dispose();
            SystemPreReset.Dispose();
            SystemPostReset.Dispose();
            SystemPreNew.Dispose();
            SystemPostNew.Dispose();
            SystemPreLoad.Dispose();
            SystemPostLoad.Dispose();
            //SystemPreMerge.Dispose();
            SystemPostMerge.Dispose();
        }

        #endregion
    }
}