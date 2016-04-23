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
    public class BoxAspectEngine : TriStateAspectEngine
    {
        #region Constructors

        public BoxAspectEngine(NlmTreeListView listView, OLVColumn column) 
            : base(listView, column)
        {
        }

        #endregion

        #region Override Methods

        public override Object GetTreeNodeValue(BaseTreeNode treeNode)
        {
            return treeNode.Box;
        }
        public override void PutTreeNodeValue(BaseTreeNode treeNode, Object newValue)
        {
            treeNode.Box = (Boolean?)newValue;
        }

        public override Object GetMaxValue(BaseTreeNode treeNode)
        {
            ObjectTreeNode objectTreeNode = treeNode as ObjectTreeNode;
            if (objectTreeNode != null)
            {
                IINode maxNode = MaxAnimatable.GetAnimByHandle(objectTreeNode.Handle) as IINode;
                if (maxNode != null)
                    return maxNode.BoxMode_ < 0;
            }

            LayerTreeNode layerTreeNode = treeNode as LayerTreeNode;
            if (layerTreeNode != null)
            {
                IILayer maxLayer = MaxAnimatable.GetAnimByHandle(layerTreeNode.Handle) as IILayer;

#if Max2013 || Max2014
                if (maxLayer != null)
                    return !maxLayer.BoxMode;
#endif
#if Max2015
                if (maxLayer != null) 
                    return !maxLayer.BoxMode_;
#endif
            }

            return false;
        }
        public override void PutMaxValue(BaseTreeNode treeNode, Object newValue)
        {
            LayerTreeNode layerTreeNode = treeNode as LayerTreeNode;
            if (layerTreeNode != null)
            {
                IILayer maxLayer = MaxAnimatable.GetAnimByHandle(layerTreeNode.Handle) as IILayer;

#if Max2013 || Max2014
                if (maxLayer != null) maxLayer.BoxMode = !(Boolean)newValue;
#endif
#if Max2015
                if (maxLayer != null) maxLayer.BoxMode(!(Boolean)newValue, false);
#endif
                return;
            }

            ObjectTreeNode objectTreeNode = treeNode as ObjectTreeNode;
            if (objectTreeNode != null)
            {
                IINode maxNode = MaxAnimatable.GetAnimByHandle(objectTreeNode.Handle) as IINode;

                if (maxNode != null) maxNode.BoxMode(!(Boolean)newValue);
                return;
            }
        }

        #endregion
    }
}
