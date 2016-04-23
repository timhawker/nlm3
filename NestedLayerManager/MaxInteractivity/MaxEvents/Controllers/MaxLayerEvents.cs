using System;
using Autodesk.Max;
using ManagedServices;
using System.Windows.Forms;
using System.Collections.Generic;
using NestedLayerManager.SubControls;
using NestedLayerManager.IO;
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
    public class MaxLayerEvents : IDisposable
    {
        #region Properties

        private NlmTreeListView ListView;
        private NodeController NodeControl;

        public SystemNotificationAnimHandler LayerCreated;
        public SystemNotificationAnimHandler LayerDeleted;

        #endregion

        #region Constructor

        public MaxLayerEvents(NlmTreeListView listView, NodeController nodeControl)
        {
            // Set properties.
            ListView = listView;
            NodeControl = nodeControl;

            // Register notifications
            LayerCreated = new SystemNotificationAnimHandler(SystemNotificationCode.LayerCreated);
            LayerCreated.NotificationRaised += new EventHandler<SystemNotificationAnimEventArgs>(onLayerCreated);

            LayerDeleted = new SystemNotificationAnimHandler(SystemNotificationCode.LayerDeleted);
            LayerDeleted.NotificationRaised += new EventHandler<SystemNotificationAnimEventArgs>(onLayerDeleted);

            // TODO: how do we handle these events? There don't appear to be any notifications for them.
            // layer name changed
            // layer current changed
            // layer hidden changed
            // layer frozen changed
            // layer render changed
            // layer color changed
            // layer box changed
        }

        #endregion

        #region Event Methods

        private void onLayerCreated(Object sender, SystemNotificationAnimEventArgs e)
        {
#if DEBUG
            MaxListener.PrintToListener("onLayerCreated");
#endif
            List<LayerTreeNode> createdNodes = new List<LayerTreeNode>();
            foreach(UIntPtr handle in e.Handles)
            {
                // TODO: Add handlemap ref to this class.
                // LayerTreeNode newNode = NodeControl.Create.CreateTreeNode(NlmTreeNode.NodeClass.Layer, handle);
                LayerTreeNode newNode = new LayerTreeNode(handle, NodeControl.HandleMap); 
                createdNodes.Add(newNode);
            }
            ListView.AddObjects(createdNodes);
            ListView.Sort(ListView.NlmColumns.NameColumn, SortOrder.Ascending);
        }

        private void onLayerDeleted(Object sender, SystemNotificationAnimEventArgs e)
        {
#if DEBUG
            MaxListener.PrintToListener("onLayerDeleted");
#endif
            List<BaseTreeNode> deletedNodes = new List<BaseTreeNode>();
            foreach (UIntPtr handle in e.Handles)
            {
                List<BaseTreeNode> treeNodes = NodeControl.HandleMap.GetTreeNodesByHandle(handle);
                foreach (BaseTreeNode treeNode in treeNodes)
                {
                    deletedNodes.Add(treeNode);
                }
            }
            NodeControl.Destroy.DeleteTreeNodes(deletedNodes);
        }

        #endregion

        #region Disposing

        public void Dispose()
        {
            LayerCreated.Dispose();
            LayerDeleted.Dispose();
        }

        #endregion
    }
}