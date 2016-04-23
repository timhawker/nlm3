using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Max;
using Autodesk.Max.Plugins;
using ManagedServices;
using NestedLayerManager.SubControls;
using NestedLayerManager.IO;
using NestedLayerManager.NodeControl;
using NestedLayerManager.Maps;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.Events;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;

namespace NestedLayerManager.MaxInteractivity.MaxEvents.Controllers

{

    public class NodeEventCallback : INodeEventCallback
    {
        #region Properties

        private UInt32 CallbackKey;
    
        private NlmTreeListView ListView;
        private NodeController NodeControl;
        private Boolean IsCallbackRegistered;

        #endregion

        #region Constructor

        public NodeEventCallback(NlmTreeListView listView, NodeController nodeControl)
        {
            ListView = listView;
            NodeControl = nodeControl;
            Register();
        }

        #endregion

        #region Callback Registering

        public void Register()
        {
            if (IsCallbackRegistered)
                return;

            GlobalInterface.Instance.ISceneEventManager.RegisterCallback(this, false, 0, false);
            CallbackKey = GlobalInterface.Instance.ISceneEventManager.GetKeyByCallback(this);

            IsCallbackRegistered = true;

            #if DEBUG
            MaxListener.PrintToListener("NodeEventCallback Registered");
            #endif
        }

        public void Unregister()
        {
            if (!IsCallbackRegistered)
                return;

            GlobalInterface.Instance.ISceneEventManager.UnRegisterCallback(CallbackKey);

            IsCallbackRegistered = false;

            #if DEBUG
            MaxListener.PrintToListener("NodeEventCallback Unregistered");
            #endif
        }

        #endregion

        #region NodeEventCallback Overrides

        public override void NameChanged(ITab<UIntPtr> nodes)
        {
#if DEBUG
            MaxListener.PrintToListener("NodeEventCallback > NameChanged");
#endif
            RefreshNodes(nodes);
        }

        public override void HideChanged(ITab<UIntPtr> nodes)
        {
#if DEBUG
            MaxListener.PrintToListener("NodeEventCallback > HideChanged");
#endif
            RefreshNodes(nodes);
        }

        public override void FreezeChanged(ITab<UIntPtr> nodes)
        {
#if DEBUG
            MaxListener.PrintToListener("NodeEventCallback > FreezeChanged");
#endif
            RefreshNodes(nodes);
        }

        public override void RenderPropertiesChanged(ITab<UIntPtr> nodes)
        {
#if DEBUG
            MaxListener.PrintToListener("NodeEventCallback > RenderPropertiesChanged");
#endif
            RefreshNodes(nodes);
        }

        public override void WireColorChanged(ITab<UIntPtr> nodes)
        {
#if DEBUG
            MaxListener.PrintToListener("NodeEventCallback > WireColorChanged");
#endif
            RefreshNodes(nodes);
        }

        public override void DisplayPropertiesChanged(ITab<UIntPtr> nodes)
        {
#if DEBUG
            MaxListener.PrintToListener("NodeEventCallback > DisplayPropertiesChanged");
#endif
            RefreshNodes(nodes);
        }

        private void RefreshNodes(ITab<UIntPtr> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                UIntPtr handle = nodes[(IntPtr)i];
                List<BaseTreeNode> treeNodes = NodeControl.HandleMap.GetTreeNodesByHandle(handle);
                foreach (BaseTreeNode treeNode in treeNodes)
                {
                    if (treeNode == null)
                        continue;
                    ListView.RefreshObject(treeNode);
                }
            }
        }

        public override void LayerChanged(ITab<UIntPtr> nodes)
        {
#if DEBUG
            MaxListener.PrintToListener("NodeEventCallback > LayerChanged");
#endif      
            List<BaseTreeNode> deleteNodes = new List<BaseTreeNode>();
            List<Tuple<BaseTreeNode, BaseTreeNode>> addNodes = new List<Tuple<BaseTreeNode, BaseTreeNode>>();

            for (int i = 0; i < nodes.Count; i++)
            {
                UIntPtr maxNodeHandle = nodes[(IntPtr)i];
                IINode maxNode = MaxAnimatable.GetAnimByHandle(maxNodeHandle) as IINode;
                IILayer maxLayer = MaxLayers.GetLayer(maxNode);
                UIntPtr layerHandle = MaxAnimatable.GetHandleByAnim(maxLayer);

                // We need to handle the following scenarios:
                //  An object being moved to another layer.
                //  Objects on instances layers moving to uninstanced layers.
                //  Objects on uninstanced layers moving to instanced layers.
                
                // The easiest way to do this is to remove old object nodes and create new ones.
                // This should be pretty fast, and this event should fire relatively rarely, 
                // but it may have to be rethought if it's too slow.

                // First we remove the old nodes.
                List<BaseTreeNode> objectTreeNodes = NodeControl.HandleMap.GetTreeNodesByHandle(maxNodeHandle);
                deleteNodes.AddRange(objectTreeNodes);

                // Then we add the object node to the new layer.
                List<BaseTreeNode> layerTreeNodes = NodeControl.HandleMap.GetTreeNodesByHandle(layerHandle);
                foreach (BaseTreeNode layerTreeNode in layerTreeNodes)
                {
                    ObjectTreeNode newObjectTreeNode = new ObjectTreeNode(MaxNodes.GetObjectClass(maxNode), maxNodeHandle, NodeControl.HandleMap);
                    addNodes.Add(new Tuple<BaseTreeNode, BaseTreeNode> (newObjectTreeNode, layerTreeNode));
                }
            }

            // And finally we actually do the update all at once.
            NodeControl.Destroy.DeleteTreeNodes(deleteNodes);
            foreach (Tuple<BaseTreeNode, BaseTreeNode> tuple in addNodes)
            {
                NodeControl.Parent.AddChild(tuple.Item1, tuple.Item2, false);
            }

            // And sort :)
            ListView.Sort(ListView.NlmColumns.NameColumn, SortOrder.Ascending);
        }

        public override void Deleted(ITab<UIntPtr> nodes)
        {
#if DEBUG
            MaxListener.PrintToListener("NodeEventCallback > Deleted");
#endif      
            List<BaseTreeNode> deletedNodes = new List<BaseTreeNode>();
            for (int i = 0; i < nodes.Count; i++)
            {
                UIntPtr nodeHandle = nodes[(IntPtr)i];

                List<BaseTreeNode> baseTreeNodes = NodeControl.HandleMap.GetTreeNodesByHandle(nodeHandle);
                foreach (BaseTreeNode baseTreeNode in baseTreeNodes)
                {
                    if (baseTreeNode != null)
                        deletedNodes.Add(baseTreeNode);
                }
            }
            try
            {
                ListView.BeginUpdate();
                NodeControl.Destroy.DeleteTreeNodes(deletedNodes);
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
            Unregister();
            base.Dispose();
        }

        #endregion
    }

}