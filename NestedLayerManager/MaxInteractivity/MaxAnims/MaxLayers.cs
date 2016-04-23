using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;
using NestedLayerManager.MaxInteractivity;

namespace NestedLayerManager.MaxInteractivity.MaxAnims
{

    public static class MaxLayers
    {
        #region Query

        /// <summary>
        /// Returns the layer count of the 3ds Max scene.
        /// </summary>
        public static Int32 LayerCount
        {
            get 
            {
                return MaxInterfaces.Instance.FPLayerManager.Count;
            }
        }

        /// <summary>
        /// Returns all child IINode objects of provided IILayer.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static IEnumerable<IINode> GetChildNodes(IILayer layer)
        {
            IILayerProperties layerProps = GetLayerProperties(layer);
            ITab<IINode> layerNodes = GlobalInterface.Instance.Tab.Create<IINode>();
            layerProps.Nodes(layerNodes);
            for (int i = 0; i < layerNodes.Count; i++)
            {
                yield return layerNodes[(IntPtr)i];
            }
        }

        /// <summary>
        /// Returns all IILayer objects in the 3ds Max scene.
        /// </summary>
        public static IEnumerable<IILayer> Layers
        {
            get
            {
                IILayerManager LayerManager = GlobalInterface.Instance.COREInterface13.LayerManager;
                for (int i = 0; i < LayerManager.LayerCount; i++)
                {
                    IILayer layer = LayerManager.GetLayer(i);
                    yield return layer;
                }
            }
        }

        /// <summary>
        /// Returns IILayer object from provided index. 
        /// Index must be less than total count of layers in scene.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static IILayer GetLayer(Int32 index)
        {
            return GlobalInterface.Instance.COREInterface14.LayerManager.GetLayer(index);
        }

        /// <summary>
        /// Returns IILayer obect from provided IINode.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IILayer GetLayer(IINode node)
        {
            return node.GetReference(6) as IILayer;
        }

        /// <summary>
        /// Returns IILayer object from provided string.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IILayer GetLayer(String name)
        {
#if Max2013 || Max2014
            return GlobalInterface.Instance.COREInterface14.LayerManager.GetLayer(ref name);
#endif
#if Max2015
            return GlobalInterface.Instance.COREInterface14.LayerManager.GetLayer(name);
#endif
        }

        /// <summary>
        /// Returns layer properties interface of provided IILayer.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static IILayerProperties GetLayerProperties(IILayer layer)
        {
            return MaxInterfaces.Instance.FPLayerManager.GetLayer(layer.Name);
        }

        /// <summary>
        /// Returns layer properties interface of provided IILayer.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static IILayerProperties GetLayerProperties(Int32 index)
        {
            return MaxInterfaces.Instance.FPLayerManager.GetLayer(index);
        }

        #endregion

        #region Creation

        /// <summary>
        /// Creates an IILayer in 3ds Max.
        /// </summary>
        /// <returns>Returns the created IILayer</returns>
        public static IILayer CreateLayer(Boolean makeCurrent, Boolean addSelection)
        {
            IILayer layer = GlobalInterface.Instance.COREInterface14.LayerManager.CreateLayer();

            if (makeCurrent)
            {
                SetCurrentLayer(layer);
            }

            if (addSelection)
            {
                MaxNodes.MoveNodesToLayer(MaxNodes.SelectedNodes, layer);
            }

            return layer;
        }

        #endregion

        #region Interaction

        /// <summary>
        /// Sets the provided IILayer by index to the current layer.
        /// </summary>
        /// <param name="index"></param>
        public static void SetCurrentLayer(int index)
        {
            using (IILayerProperties layerProps = GetLayerProperties(index))
            {
                layerProps.Current = true;
            }
        }

        /// <summary>
        /// Sets the provided IILayer to the current layer.
        /// </summary>
        /// <param name="layer"></param>
        public static void SetCurrentLayer(IILayer layer)
        {
            IILayerProperties layerProps = MaxLayers.GetLayerProperties(layer);
            layerProps.Current = true;
        }

        /// <summary>
        /// Hides / Unhides all IILayer objects.
        /// </summary>
        public static void HideUnhideAll()
        {
            Boolean isHidden = false;
            IILayerManager LayerManager = GlobalInterface.Instance.COREInterface13.LayerManager;

            for (int i = 0; i < LayerManager.LayerCount; i++)
            {
                IILayer layer = LayerManager.GetLayer(i);
                if (!layer.IsHidden)
                {
                    isHidden = true;
                }
            }
            for (int i = 0; i < LayerManager.LayerCount; i++)
            {
                IILayer layer = LayerManager.GetLayer(i);

#if Max2013 || Max2014
                layer.IsHidden = isHidden;
#endif

#if Max2015
                    layer.Hide(isHidden, false);
#endif
            }

            MaxUI.RedrawViewportsNow();
        }

        /// <summary>
        /// Freezes / Unfreezes all IILayer objects.
        /// </summary>
        public static void FreezeUnfreezeAll()
        {
            Boolean isFrozen = false;
            IILayerManager LayerManager = GlobalInterface.Instance.COREInterface13.LayerManager;

            for (int i = 0; i < LayerManager.LayerCount; i++)
            {
                IILayer layer = LayerManager.GetLayer(i);
                if (!layer.IsFrozen)
                {
                    isFrozen = true;
                }
            }
            for (int i = 0; i < LayerManager.LayerCount; i++)
            {
                IILayer layer = LayerManager.GetLayer(i);

#if Max2013 || Max2014
                layer.IsFrozen = isFrozen;
#endif

#if Max2015
                    layer.Freeze(isFrozen, false);
#endif
            }

            MaxUI.RedrawViewportsNow();
        }

        #endregion

        #region Deletion

        /// <summary>
        /// Deletes the provided IILayer object, and optionally all child nodes.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="deleteNodes"></param>
        public static void DeleteLayer(IILayer layer, Boolean deleteNodes)
        {
            // Check if layer has any child nodes.
            // If layer has children, this prevents the layer from being deleted.
            // Loop through all the children and delete them.
            if (deleteNodes)
            {
                using (IILayerProperties layerProps = GetLayerProperties(layer))
                {
                    IEnumerable<IINode> maxNodes = GetChildNodes(layer);
                    foreach (IINode maxNode in maxNodes)
                    {
                        MaxNodes.DeleteNode(maxNode);
                    }
                }
            }
            
            MaxInterfaces.Instance.FPLayerManager.DeleteLayer(layer.Name);
        }

        public static void DeleteLayer(UIntPtr layerHandle)
        {
            IILayer layer = MaxAnimatable.GetAnimByHandle(layerHandle) as IILayer;

            if (layer == null)
                return;

            IILayerProperties layerProperties = MaxLayers.GetLayerProperties(layer);
            if (layerProperties.Current)
                MaxLayers.SetCurrentLayer(0);

            MaxInterfaces.Instance.FPLayerManager.DeleteLayer(layer.Name);
        }

        #endregion
    }
}
