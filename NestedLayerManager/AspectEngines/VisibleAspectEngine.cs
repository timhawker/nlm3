using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;
using Autodesk.Max;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.AspectEngines.Base;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.Nodes;

namespace NestedLayerManager.AspectEngines
{
    public class VisibleAspectEngine : TriStateAspectEngine
    {
        #region Constructors

        public VisibleAspectEngine(NlmTreeListView listView, OLVColumn column) 
            : base(listView, column)
        {
        }

        #endregion

        #region Override Methods

        public override Object GetTreeNodeValue(BaseTreeNode treeNode)
        {
            return treeNode.Visible;
        }
        public override void PutTreeNodeValue(BaseTreeNode treeNode, Object newValue)
        {
            treeNode.Visible = (Boolean?)newValue;
        }

        public override Object GetMaxValue(BaseTreeNode treeNode)
        {
            LayerTreeNode layerTreeNode = treeNode as LayerTreeNode;
            if (layerTreeNode != null)
            {
                IILayer layer = MaxAnimatable.GetAnimByHandle(layerTreeNode.Handle) as IILayer;
                if (layer != null)
                    return !layer.IsHidden;
            }
            ObjectTreeNode objectTreeNode = treeNode as ObjectTreeNode;
            if (objectTreeNode != null)
            {
                IINode maxNode = MaxAnimatable.GetAnimByHandle(objectTreeNode.Handle) as IINode;
                if (maxNode != null)
                    return !maxNode.IsObjectHidden;
            }
            return false;
        }

        public override void PutMaxValue(BaseTreeNode treeNode, Object newValue)
        {
            ObjectTreeNode objectTreeNode = treeNode as ObjectTreeNode;
            if (objectTreeNode != null)
            {
                IINode maxNode = MaxAnimatable.GetAnimByHandle(objectTreeNode.Handle) as IINode;
                if (maxNode != null)
                    maxNode.Hide(!(Boolean)newValue);
            }
            LayerTreeNode layerTreeNode = treeNode as LayerTreeNode;
            if (layerTreeNode != null)
            {
                IILayer maxLayer = MaxAnimatable.GetAnimByHandle(layerTreeNode.Handle) as IILayer;
#if Max2013 || Max2014
                if (maxLayer != null)
                    maxLayer.IsHidden = !(Boolean)newValue;
#endif
#if Max2015
                if (maxLayer != null)
                    maxLayer.Hide(!(Boolean)newValue, false);
#endif
            }
        }

        #endregion
    }
}
