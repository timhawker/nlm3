using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Autodesk.Max;
using BrightIdeasSoftware;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.MaxInteractivity.MaxEvents;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;

namespace NestedLayerManager.NodeControl.Engines
{
    public class NodeDragDropEngine
    {
        private NlmTreeListView ListView;
        private NodeParentEngine NodeParentEngine;
        private MaxEventEngine MaxEvents;

        public NodeDragDropEngine(NlmTreeListView listView, NodeParentEngine nodeParentEngine, MaxEventEngine maxEvents)
        {
            ListView = listView;
            NodeParentEngine = nodeParentEngine;
            MaxEvents = maxEvents;

            // Add Events.
            ListView.ModelCanDrop += new EventHandler<ModelDropEventArgs>(CanDrop);
            ListView.ModelDropped += new EventHandler<ModelDropEventArgs>(Dropped);
        }

        // Can drop event.
        // Allows or prevents drop.
        private void CanDrop(Object sender, ModelDropEventArgs e)
        {
            // Guilty until proven innocent
            e.Effect = DragDropEffects.None;
            e.DropSink.FeedbackColor = Color.IndianRed;

            //Is mouse inside control
            if (ListView.Bounds.Contains(e.MouseLocation))
            {
                // Get source and target tree nodes.
                IEnumerable<BaseTreeNode> sourceTreeNodes = e.SourceModels.Cast<BaseTreeNode>();
                BaseTreeNode target = e.TargetModel as BaseTreeNode;

                // Handle drag drop when target is a valid treeNode
                if (target != null)
                {
                    // Check to make sure drop target it not a child of drag nodes, and drop parent is not the same as drag parent.
                    if (!sourceTreeNodes.Any(x => target.IsAncestor(x)))
                    {
                        // If layers or folders being dropped on layers, allow move.
                        // The drop event handler will correctly handle shifting the target to the parent.
                        if (sourceTreeNodes.All(x => x is LayerTreeNode) && target is LayerTreeNode || sourceTreeNodes.All(x => x is FolderTreeNode) && target is LayerTreeNode)
                        {
                            e.DropSink.FeedbackColor = ListView.HighlightBackgroundColor;
                            e.Effect = DragDropEffects.Move;
                        }

                        // If all drag nodes are folders or layers, allow move.
                        if (sourceTreeNodes.All(x => x is LayerTreeNode) && target is FolderTreeNode || sourceTreeNodes.All(x => x is FolderTreeNode) && target is FolderTreeNode)
                        {
                            e.DropSink.FeedbackColor = ListView.HighlightBackgroundColor;
                            e.Effect = DragDropEffects.Move;
                        }
                        // If all drag nodes are iNodes and target is a layer, allow move.
                        if (sourceTreeNodes.All(x => x is ObjectTreeNode) && target is LayerTreeNode)
                        {
                            e.DropSink.FeedbackColor = ListView.HighlightBackgroundColor;
                            e.Effect = DragDropEffects.Move;
                        }
                        // If all drag nodes are iNodes and target is an iNode, allow move.
                        if (sourceTreeNodes.All(x => x is ObjectTreeNode) && target is ObjectTreeNode)
                        {
                            e.DropSink.FeedbackColor = ListView.HighlightBackgroundColor;
                            e.Effect = DragDropEffects.Move;
                        }
                    }
                }
                // Handle drag drop when target is null by moving drag nodes to root.
                else
                {
                    // If all drag nodes are folders or layers, allow move.
                    if (sourceTreeNodes.All(x => x is LayerTreeNode) || sourceTreeNodes.All(x => x is FolderTreeNode))
                    {
                        e.DropSink.FeedbackColor = ListView.HighlightBackgroundColor;
                        e.Effect = DragDropEffects.Move;
                    }
                }
            }
        }

        // Drop event.
        // Move Objects to new parents on drag success.
        private void Dropped(Object sender, ModelDropEventArgs e)
        {
            e.Handled = true;
            switch (e.DropTargetLocation)
            {
                case DropTargetLocation.None:
                    DragDropMoveNodes(null, e.SourceModels);
                    break;
                case DropTargetLocation.Item:
                    DragDropMoveNodes(e.TargetModel as BaseTreeNode, e.SourceModels);
                    break;

                default:
                    return;
            }
            
            // TODO:
            // We might not be able to use RefreshObjects() if we change the target parent.
            e.RefreshObjects();
        }

        // Move nodes to new parent after a drag drop operation.
        private void DragDropMoveNodes(BaseTreeNode target, IList dragNodes)
        {
            IList selection = ListView.SelectedObjects;
            ListView.BeginUpdate();
            
            MaxEvents.NodeEvents.Unregister();

            IEnumerable<BaseTreeNode> dragNodesEnum = dragNodes.Cast<BaseTreeNode>();
            // Are layers or folders being dropped on layers?
            // If so, make the target the parent of the target. 
            if (dragNodesEnum.All(x => x is LayerTreeNode) && target is LayerTreeNode || dragNodesEnum.All(x => x is FolderTreeNode) && target is LayerTreeNode)
            {
                target = target.Parent;
            }
            // Are objects being dropped on objects?
            // If so, make the target the parent of the target.
            if (dragNodesEnum.All(x => x is ObjectTreeNode) && target is ObjectTreeNode)
            {
                target = target.Parent;
            }

            // Do we move to the root or move under a treeNode?
            // If the target is null, we have to move to root.
            if (target != null && target is LayerTreeNode)
            {
                // Move the treenodes in the listView.
                NodeParentEngine.MoveTreeNodes(dragNodes.Cast<BaseTreeNode>().ToList(), target);

                //TODO:
                //IEnumerable<UIntPtr> handles = dragNodes.Cast<IEnumerable<BaseTreeNode>>()
                //     .Where(x => x is ObjectTreeNode)
                //     .Cast<ObjectTreeNode>()
                //     .Select(x => x.Handle);

                // Move nodes to new layer in max.
                List<UIntPtr> handles = new List<UIntPtr>();
                foreach (BaseTreeNode child in dragNodes)
                {
                    ObjectTreeNode objectChild = child as ObjectTreeNode;
                    if (objectChild != null)
                    {
                        handles.Add(objectChild.Handle);
                    }
                }
                IILayer layer = MaxAnimatable.GetAnimByHandle(((LayerTreeNode)target).Handle) as IILayer;
                if (layer != null)
                {
                    MaxNodes.MoveNodesToLayer(handles, layer);
                }
            }
            else
            {
                NodeParentEngine.MoveTreeNodes(dragNodes.Cast<BaseTreeNode>().ToList(), target);
            }

            MaxEvents.NodeEvents.Register();

            // Default after drop behaviour tries to select nodes based on itemIndex. As nodes have moved this does not work. 
            // Instead, return saved selection to SelectedObjects property, and make sure dropped Objects are visible.
            if (!ListView.IsExpanded(target)) ListView.Expand(target);
            ListView.Sort(ListView.NlmColumns.NameColumn, SortOrder.Ascending);
            ListView.SelectedObjects = selection;
            ListView.EndUpdate();
        }
    }
}
