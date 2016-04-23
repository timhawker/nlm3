using System;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using NestedLayerManager.Maps;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.IO.Data;
using Autodesk.Max;
using NestedLayerManager.MaxInteractivity.MaxAnims;

namespace NestedLayerManager.Nodes
{
    public class LayerTreeNode : BaseTreeNode
    {
        #region Properties 

        private HandleMap HandleMap;
        public UIntPtr Handle;

        #endregion

        #region Read-Only Properties

        public Boolean IsInstanced
        {
            get
            {
                return HandleMap.GetTreeNodesByHandle(Handle).Count() > 1;
            }
        }

        #endregion

        #region Constructors

        public LayerTreeNode(UIntPtr handle, HandleMap handleMap)
        {
            Handle = handle;
            HandleMap = handleMap;
            handleMap.AddTreeNode(handle, this);
        }

        public LayerTreeNode(LayerData data, UIntPtr handle, HandleMap handleMap)
        {
            Visible = data.Visible;
            Freeze = data.Freeze;
            Render = data.Render;
            Box = data.Box;
            WasExpanded = data.Expanded;
            Handle = handle;
            HandleMap = handleMap;
            handleMap.AddTreeNode(handle, this);
        }

        #endregion
    }
}