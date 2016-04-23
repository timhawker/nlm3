using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Autodesk.Max;
using NestedLayerManager.SubControls;
using NestedLayerManager.NodeControl;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.MaxInteractivity.MaxEvents.Handlers;
using NestedLayerManager.MaxInteractivity.MaxEvents.Args;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;

namespace NestedLayerManager.MaxInteractivity.MaxEvents.Controllers
{

    public class MaxNodeEvents : NodeEventCallback
    {
        #region Properties
    
        private NlmTreeListView ListView;
        private NodeController NodeControl;
        public SystemNotificationAnimHandler NodeCreated;

        #endregion

        #region Constructor

        public MaxNodeEvents(NlmTreeListView listView, NodeController nodeControl)
            : base(listView, nodeControl)
        {
            ListView = listView;
            NodeControl = nodeControl;

            NodeCreated = new SystemNotificationAnimHandler(SystemNotificationCode.NodeCreated);
            NodeCreated.NotificationRaised += new EventHandler<SystemNotificationAnimEventArgs>(onNodeCreated);
        }

        #endregion

        #region Events

        private void onNodeCreated(object sender, SystemNotificationAnimEventArgs e)
        {
#if DEBUG
            MaxListener.PrintToListener("onNodeCreated");
#endif
            try
            {
                ListView.BeginUpdate();

                List<BaseTreeNode> refreshNodes = new List<BaseTreeNode>();
                foreach (UIntPtr handle in e.Handles)
                {

                    IAnimatable anim = MaxAnimatable.GetAnimByHandle(handle);
                    if (anim == null)
                        return;

                    IINode node = anim as IINode;
                    IILayer layer = MaxLayers.GetLayer(node);
                    UIntPtr layerHandle = MaxAnimatable.GetHandleByAnim(layer);
                    ObjectTreeNode.ObjectClass objectClass = MaxNodes.GetObjectClass(node);

                    List<BaseTreeNode> layerTreeNodes = NodeControl.HandleMap.GetTreeNodesByHandle(layerHandle);
                    foreach (BaseTreeNode layerTreeNode in layerTreeNodes)
                    {
                        ObjectTreeNode objectTreeNode = new ObjectTreeNode(objectClass, handle, NodeControl.HandleMap);
                        NodeControl.Parent.AddChild(objectTreeNode, layerTreeNode, false);

                        if (!refreshNodes.Contains(layerTreeNode))
                            refreshNodes.Add(layerTreeNode);
                    }
                }
                MaxListener.PrintToListener(refreshNodes.Count.ToString());
                ListView.RefreshObjects(refreshNodes);
                ListView.Sort(ListView.NlmColumns.NameColumn, SortOrder.Ascending);
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

        public override void Dispose()
        {
            NodeCreated.Dispose();
            base.Dispose();
        }

        #endregion
    }

}