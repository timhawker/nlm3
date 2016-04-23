using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.Maps;
using NestedLayerManager.NodeControl;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;

namespace NestedLayerManager.MaxInteractivity.MaxAnims
{
    public static class MaxNodes
    {
        #region Query

        /// <summary>
        /// Gets the root IINode in the current 3ds Max scene.
        /// </summary>
        public static IINode RootNode
        {
            get 
            {
                return GlobalInterface.Instance.COREInterface.RootNode;
            }
        }

        /// <summary>
        /// Returns ObjectClass of IINode.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static ObjectTreeNode.ObjectClass GetObjectClass(IINode node)
        {
            IObjectState ObjectState = node.EvalWorldState(0, true);
            switch (ObjectState.Obj.SuperClassID)
            {
                case SClass_ID.Geomobject:
                    if (ObjectState.Obj.ClassName == "TargetObject")
                    {
                        return ObjectTreeNode.ObjectClass.Helper;
                    }
                    if (ObjectState.Obj.ClassName == "Bone")
                    {
                        return ObjectTreeNode.ObjectClass.Bone;
                    }
                    return ObjectTreeNode.ObjectClass.Object;
                case SClass_ID.Shape:
                    return ObjectTreeNode.ObjectClass.Shape;
                case SClass_ID.Light:
                    return ObjectTreeNode.ObjectClass.Light;
                case SClass_ID.Camera:
                    return ObjectTreeNode.ObjectClass.Camera;
                case SClass_ID.Helper:
                    return ObjectTreeNode.ObjectClass.Helper;
                case SClass_ID.WsmObject:
                    return ObjectTreeNode.ObjectClass.SpaceWarp;
                default:
                    return ObjectTreeNode.ObjectClass.Object;
            }
        }

        /// <summary>
        /// Returns all child IINode objects of provided IINode.
        /// </summary>
        /// <param name="maxNode"></param>
        /// <returns></returns>
        private static IEnumerable<IINode> Children(IINode maxNode)
        {
            for (int i = 0; i < maxNode.NumberOfChildren; i++)
            {
                yield return maxNode.GetChildNode(i);
            }
        }

        /// <summary>
        /// Recursive Linq method to obtain ALL children (including nested children) of provided IINode.
        /// </summary>
        /// <param name="maxNode"></param>
        /// <returns></returns>
        private static IEnumerable<IINode> GetAllSelectedChildren(IINode maxNode)
        {
            if (maxNode.Selected)
                yield return maxNode;

            foreach (IINode child in Children(maxNode))
                foreach (IINode childChild in GetAllSelectedChildren(child))
                {
                    if (childChild.Selected)
                        yield return childChild;
                }
        }
        
        /// <summary>
        /// Gets currently selected IINode objects in 3ds Max Scene.
        /// </summary>
        public static IEnumerable<IINode> SelectedNodes
        {
            get
            {
                IEnumerable<IINode> selectedNodes = new List<IINode>() { RootNode };
                return selectedNodes.SelectMany(GetAllSelectedChildren);
            }
        }

        /// <summary>
        /// Gets currently selected IINode Anim handles in 3ds Max Scene.
        /// </summary>
        public static IEnumerable<UIntPtr> SelectedHandles
        {
            get
            {
                foreach (IINode maxNode in SelectedNodes)
                {
                     yield return MaxAnimatable.GetHandleByAnim(maxNode);
                }
            }
        }

        #endregion

        #region Interaction

        /// <summary>
        /// Selects the provided IINodeTab in 3ds Max.
        /// </summary>
        /// <param name="nodes"></param>
        public static void SelectNodes(IINodeTab nodes)
        {
            GlobalInterface.Instance.COREInterface13.SelectNodeTab(nodes, true, true);
        }

        /// <summary>
        /// Selects the provided IINode handles in 3ds Max.
        /// If the handle is not an IINode, it will be skipped.
        /// </summary>
        /// <param name="handles"></param>
        public static void SelectNodes(IEnumerable<UIntPtr> handles)
        {
            IINodeTab nodes = GlobalInterface.Instance.NodeTab.Create();
            foreach (UIntPtr handle in handles)
            {
                IINode maxNode = MaxAnimatable.GetAnimByHandle(handle) as IINode;
                if (maxNode != null)
                    nodes.AppendNode(maxNode, true, 0);
            }
            SelectNodes(nodes);
        }

        /// <summary>
        /// Select the provided IINode in 3ds Max.
        /// </summary>
        /// <param name="maxNode"></param>
        private static void SelectNode(IINode maxNode)
        {
            GlobalInterface.Instance.COREInterface13.SelectNode(maxNode, false);
        }

        /// <summary>
        /// Clear IINode selection in 3ds Max.
        /// </summary>
        public static void ClearNodeSelection()
        {
            GlobalInterface.Instance.COREInterface13.ClearNodeSelection(true);
        }

        /// <summary>
        /// Moves a provided IINode to a provided IILayer.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="layer"></param>
        public static void MoveNodeToLayer(IINode node, IILayer layer)
        {
            layer.AddToLayer(node);
        }

        /// <summary>
        /// Moves the provided IINode Anim handles to the provided layer.
        /// If the handle is not an IINode, it will be skipped.
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="layer"></param>
        public static void MoveNodesToLayer(IEnumerable<UIntPtr> handles, IILayer layer)
        {
            foreach (UIntPtr handle in handles)
            {
                IINode maxNode = MaxAnimatable.GetAnimByHandle(handle) as IINode;
                if (maxNode != null)
                    MoveNodeToLayer(maxNode as IINode, layer);
            }
        }

        /// <summary>
        /// Moves the provided IINode objects to the provided layer.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="layer"></param>
        public static void MoveNodesToLayer(IEnumerable<IINode> nodes, IILayer layer)
        {
            foreach (IINode node in nodes)
                MoveNodeToLayer(node, layer);
        }

        #endregion

        #region Deletion

        /// <summary>
        /// Deletes the provided IINode from the 3ds Max scene.
        /// </summary>
        /// <param name="node"></param>
        public static void DeleteNode(IINode node)
        {
            GlobalInterface.Instance.COREInterface.DeleteNode(node, false, false);
        }

        public static void DeleteNode(UIntPtr nodeHandle)
        {
            IAnimatable node = MaxAnimatable.GetAnimByHandle(nodeHandle);
            DeleteNode(node as IINode);
        }

        #endregion
    }
}
