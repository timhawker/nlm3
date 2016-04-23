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
    public class NameAspectEngine : BaseAspectEngine
    {
        #region Constructors

        public NameAspectEngine(NlmTreeListView listView, OLVColumn column)
            : base(listView, column)
        {
        }

        #endregion

        #region Public Overrides

        public override Object GetAspect(Object rowObject)
        {
            FolderTreeNode folderTreeNode = rowObject as FolderTreeNode;
            if (folderTreeNode != null)
            {
                return folderTreeNode.Name;
            }
            LayerTreeNode layerTreeNode = rowObject as LayerTreeNode;
            if (layerTreeNode != null)
            {
                IILayer layer = MaxAnimatable.GetAnimByHandle(layerTreeNode.Handle) as IILayer;
                if (layer != null)
                    return layer.Name;
            }
            ObjectTreeNode objectTreeNode = rowObject as ObjectTreeNode;
            if (objectTreeNode != null)
            {
                IINode maxNode = MaxAnimatable.GetAnimByHandle(objectTreeNode.Handle) as IINode;
                if (maxNode != null)
                    return maxNode.Name;
            }
            return null;
        }

        public override void PutAspect(Object rowObject, Object newValue)
        {
            FolderTreeNode folderTreeNode = rowObject as FolderTreeNode;
            if (folderTreeNode != null)
            {
                folderTreeNode.Name = newValue as String;
                return;
            }
            LayerTreeNode layerTreeNode = rowObject as LayerTreeNode;
            if (layerTreeNode != null)
            {
                IILayer maxLayer = MaxAnimatable.GetAnimByHandle(layerTreeNode.Handle) as IILayer;
#if Max2013 || Max2014
                String newString = newValue as String;
                if (maxLayer != null)
                    maxLayer.SetName(ref newString);
#endif
#if Max2015
                if (maxLayer != null)
                    maxLayer.Name = (newValue as String);
#endif
                return;
            }
            ObjectTreeNode objectTreeNode = rowObject as ObjectTreeNode;
            if (objectTreeNode != null)
            {
                IINode maxNode = MaxAnimatable.GetAnimByHandle(objectTreeNode.Handle) as IINode;
                if (maxNode != null)
                    maxNode.Name = newValue as String;
                return;
            }
        }

        #endregion
    }
}
