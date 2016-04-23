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
    public class CurrentAspectEngine : BaseAspectEngine
    {

        #region Constructors

        public CurrentAspectEngine(OLVColumn column, NlmTreeListView listView)
            : base(listView, column)
        {
        }

        #endregion

        #region Public Overrides

        public override Object GetAspect(Object rowObject)
        {
            LayerTreeNode layerTreeNode = rowObject as LayerTreeNode;
            if (layerTreeNode != null)
            {
                IILayer layer = MaxAnimatable.GetAnimByHandle(layerTreeNode.Handle) as IILayer;
                if (layer != null)
                {
                    IILayerProperties layerProps = MaxLayers.GetLayerProperties(layer);
                    return layerProps.Current;
                }
            }
            FolderTreeNode folderTreeNode = rowObject as FolderTreeNode;
            if (folderTreeNode != null)
            {
                // Default behaviour of getFolderCheckState returns true.
                // Only get folder check state if necessary as it is
                // more expensive than returning false.
                if (folderTreeNode.Children.Count > 0)
                {
                    Boolean? checkState = getFolderCheckState(folderTreeNode);
                    if (checkState == true || checkState == null)
                        return null;
                }
            }
            return false;
        }

        public override void PutAspect(Object rowObject, Object newValue)
        {
            LayerTreeNode layerTreeNode = rowObject as LayerTreeNode;
            if (layerTreeNode != null)
            {
                IILayer layer = MaxAnimatable.GetAnimByHandle(layerTreeNode.Handle) as IILayer;
                if (layer != null)
                {
                    IILayerProperties layerProps = MaxLayers.GetLayerProperties(layer);
                    layerProps.Current = (Boolean)newValue;
                }
            }
        }

        #endregion
    }
}
