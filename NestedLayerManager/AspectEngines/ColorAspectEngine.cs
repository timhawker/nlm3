using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;
using Autodesk.Max;
using System.Drawing;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.AspectEngines.Base;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.Nodes;
using NestedLayerManager.SubControls;

namespace NestedLayerManager.AspectEngines
{
    public class ColorAspectEngine : BaseAspectEngine
    {
        NlmTreeListView NlmTreeListView;

        #region Constructors

        public ColorAspectEngine(NlmTreeListView listView, OLVColumn column)
            : base(listView, column)
        {
            NlmTreeListView = listView;
        }

        #endregion

        #region Public Interface

        public override Object GetAspect(Object rowObject)
        {
            LayerTreeNode layerTreeNode = rowObject as LayerTreeNode;
            if (layerTreeNode != null)
            {
                IILayer maxLayer = MaxAnimatable.GetAnimByHandle(layerTreeNode.Handle) as IILayer;
                if (maxLayer != null)
                {
                    // Erm, looks like max has BGR, not RGB. 
                    // Because of this, swap B and R to display properly in renderer.
                    Color bgr = maxLayer.WireColor;
                    return Color.FromArgb(bgr.B, bgr.G, bgr.R);
                }
            }
            ObjectTreeNode objectTreeNode = rowObject as ObjectTreeNode;
            if (objectTreeNode != null)
            {
                IINode maxNode = MaxAnimatable.GetAnimByHandle(objectTreeNode.Handle) as IINode;
                if (maxNode != null)
                {
                    // Erm, looks like max has BGR, not RGB. 
                    // Because of this, swap B and R to display properly in renderer.
                    Color bgr = maxNode.WireColor;
                    return Color.FromArgb(bgr.B, bgr.G, bgr.R);
                }
            }
            FolderTreeNode folderTreeNode = rowObject as FolderTreeNode;
            if (folderTreeNode != null)
            {
                return folderTreeNode.Color;
            }
            return null;
        }

        public override void PutAspect(Object rowObject, Object newValue)
        {
            FolderTreeNode folderTreeNode = rowObject as FolderTreeNode;
            if (folderTreeNode != null)
            {
                folderTreeNode.Color = (Color)newValue;
                return;
            }
            LayerTreeNode layerTreeNode = rowObject as LayerTreeNode;
            if (layerTreeNode != null)
            {
                
                IILayer maxLayer = MaxAnimatable.GetAnimByHandle(layerTreeNode.Handle) as IILayer;
                if (maxLayer != null)
                {
#if Max2013 || Max2014
                    maxLayer.WireColor = (Color)newValue;
#endif
#if Max2015
                   maxLayer.SetWireColor((Color)newValue, false);
#endif
                }
                return;
            }
            ObjectTreeNode objectTreeNode = rowObject as ObjectTreeNode;
            if (objectTreeNode != null)
            {
                IINode maxNode = MaxAnimatable.GetAnimByHandle(objectTreeNode.Handle) as IINode;
                if (maxNode != null)
                {
                    maxNode.WireColor = (Color)newValue;
                }
                return;
            }
        }

        #endregion
    }
}
