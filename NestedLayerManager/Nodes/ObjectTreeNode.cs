using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NestedLayerManager.Maps;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.Nodes.Base;
using Autodesk.Max;

namespace NestedLayerManager.Nodes
{
    public class ObjectTreeNode : BaseTreeNode
    {
        #region Properties & Enum

        public enum ObjectClass { Object, Helper, Bone, Shape, Light, Camera, SpaceWarp };

        public ObjectClass Class;
        public UIntPtr Handle;

        #endregion

        #region Constructor

        public ObjectTreeNode(ObjectClass objClass, UIntPtr handle, HandleMap handleMap)
        {
            Class = objClass;
            Handle = handle;
            handleMap.AddTreeNode(handle, this);
        }

        #endregion
    }
}