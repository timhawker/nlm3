using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;
using Autodesk.Max;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.Nodes;

namespace NestedLayerManager.AspectEngines.Base
{
    public class TriStateAspectEngine : BaseAspectEngine
    {

        #region Constructors

        public TriStateAspectEngine(NlmTreeListView listView, OLVColumn column)
            : base(listView, column)
        {
        }

        #endregion

        #region Aspect Getting

        public override Object GetAspect(Object rowObject)
        {
            BaseTreeNode treeNode = rowObject as BaseTreeNode;
            if (ListView.SmartFolderMode)
            {
                // Convert indeterminate checkstate to on for smart folder mode.
                if (treeNode is FolderTreeNode)
                {
                    if (GetTreeNodeValue(treeNode) == null)
                    {
                        PutTreeNodeValue(treeNode, true);
                    }
                }
                // This section checks to see if max node state matches NLM state. If it does not, NLM state is updated.
                if (treeNode is LayerTreeNode || treeNode is ObjectTreeNode)
                {
                    Boolean maxValue = (Boolean)GetMaxValue(treeNode);
                    Boolean? nodeValue = GetTreeNodeValue(treeNode) as Boolean?;

                    // If nodeValue FALSE, and maxValue TRUE, raise PUTVALUE on all parents and change self value.
                    if (!(Boolean)getTreeNodeCellActive(treeNode, nodeValue != false) && maxValue)
                    {
                        PutTreeNodeValue(treeNode, true);
                        if (treeNode is LayerTreeNode && treeNode.Parent != null) putValueAllParents(treeNode, true);
                    }

                    // If nodeValue TRUE, and maxValue FALSE, change self value.
                    if ((Boolean)getTreeNodeCellActive(treeNode, nodeValue != false) && !maxValue)
                    {
                        PutTreeNodeValue(treeNode, false);
                    }

                    // If nodeValue NULL, set nodeValue to maxValue.
                    if (nodeValue == null)
                    {
                        PutTreeNodeValue(treeNode, GetMaxValue(treeNode));
                    }
                }
                // Finally, return value.
                return GetTreeNodeValue(treeNode) as Boolean?;
            }
            else // Hierarchy folder mode.
            {
                // If folder, calculate check state based on children.
                if (treeNode is FolderTreeNode)
                {
                    PutTreeNodeValue(treeNode, getFolderCheckState(treeNode));
                }
                // If layer or Object, set value from layer or Object respectively.
                if (treeNode is LayerTreeNode || treeNode is ObjectTreeNode)
                {
                    PutTreeNodeValue(treeNode, GetMaxValue(treeNode));
                }
                // Finally, return value.
                return GetTreeNodeValue(treeNode) as Boolean?;
            }
        }

        // Find out if any parent nodes are NOT active, and if they are not return FALSE.
        private Object getTreeNodeCellActive(BaseTreeNode treeNode, Boolean value)
        {
            while (treeNode.Parent != null)
            {
                if ((bool?)Column.GetValue(treeNode.Parent) == false)
                {
                    value = false;
                    break;
                }
                treeNode = treeNode.Parent;
            }
            return value;
        }

        #endregion

        #region Aspect Putting

        public override void PutAspect(Object rowObject, Object newValue)
        {
            BaseTreeNode treeNode = rowObject as BaseTreeNode;
            if (ListView.SmartFolderMode)
            {
                // If turning on, the parent folders need to be turned on too.
                if ((Boolean)newValue) putValueAllParents(treeNode, true);

                ObjectTreeNode objectTreeNode = treeNode as ObjectTreeNode;
                if (objectTreeNode != null)
                {
                    IAnimatable anim = MaxAnimatable.GetAnimByHandle(objectTreeNode.Handle);
                    // Set nodeValue and maxValue to new value.
                    PutMaxValue(treeNode, newValue);
                    PutTreeNodeValue(treeNode, newValue);
                    // Avoid unnecessary casting by returning.
                    return;
                }
                // Update max state with NLM state, if max node, set new value and update tree node value.
                LayerTreeNode layerTreeNode = treeNode as LayerTreeNode;
                if (layerTreeNode != null)
                {
                    IAnimatable anim = MaxAnimatable.GetAnimByHandle(layerTreeNode.Handle);
                    // Set nodeValue and maxValue to new value.
                    PutMaxValue(treeNode, newValue);
                    PutTreeNodeValue(treeNode, newValue);
                    // Avoid unnecessary casting by returning.
                    return;
                }
                // If folder, set tree node visiblity, and change layer value to match.
                if (treeNode is FolderTreeNode)
                {
                    PutTreeNodeValue(treeNode, newValue);
                    PutLayerChildrenMaxValue(treeNode, (Boolean)newValue);
                    // Avoid unnecessary casting by returning.
                    return;
                }
                
                
            }
            else // Hierarchy folder mode.
            {
                // If folder, set tree node visiblity, and change all child layer values to match.
                if (treeNode is FolderTreeNode)
                {
                    // Set nodeValue to new value, and recursivly set all children to new value.
                    PutTreeNodeValue(treeNode, newValue);
                    SetFolderCheckState(treeNode, (Boolean)newValue);
                    // Avoid unnecessary casting by returning.
                    return;
                }
                // Update max state with NLM state, if max node, set new value and update tree node value.
                if (treeNode is LayerTreeNode || treeNode is ObjectTreeNode)
                {
                    // Set maxValue to new value.
                    PutMaxValue(treeNode, newValue);
                    // Avoid unnecessary casting by returning.
                    return;
                }
            }
        }

        // Put value in all parent nodes.
        private void putValueAllParents(BaseTreeNode treeNode, Boolean value)
        {
            while (treeNode.Parent != null)
            {
                Column.PutValue(treeNode.Parent, value);
                treeNode = treeNode.Parent;
            }
        }

        // Set all child layers to value provided recursively.
        private void PutLayerChildrenMaxValue(BaseTreeNode treeNode, Boolean value)
        {

            for (int i = 0; i < treeNode.Children.Count; i++)
            {
                BaseTreeNode child = treeNode.Children[i];
                if (child is LayerTreeNode)
                {
                    // If value is FALSE, make all maxValues FALSE.
                    if (!value) PutMaxValue(child, false);
                    // If value is TRUE, set all maxValues to nodeValue.
                    else PutMaxValue(child, (Boolean?)GetTreeNodeValue(child) == true);
                }
                if (child is FolderTreeNode && child.Children.Count > 0)
                {
                    // If value is TRUE, and node value is TRUE, run recursive function.
                    // If value is TRUE, and node value is FALSE, do nothing (otherwise deeply nested layers will turn on when they should not).
                    // If value is FALSE, run recursive function.
                    if (value)
                    {
                        if (GetTreeNodeValue(child) as Boolean? == true) PutLayerChildrenMaxValue(child, value);
                    }
                    else
                    {
                        PutLayerChildrenMaxValue(child, value);
                    }

                }
            }
        }

        // If folder value changes in hierarchy mode, recursively change all child layer node values to match.
        private void SetFolderCheckState(BaseTreeNode treeNode, Boolean value)
        {
            for (int i = 0; i < treeNode.Children.Count; i++)
            {
                BaseTreeNode child = treeNode.Children[i];
                if (child is LayerTreeNode)
                {
                    Column.PutValue(child, value);
                }
                if (child is FolderTreeNode && child.Children.Count > 0)
                {
                    SetFolderCheckState(child, value);
                }
            }
        }

        #endregion
    }
}
