using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using BrightIdeasSoftware;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using ManagedServices;
using Autodesk.Max;
using NestedLayerManager.Columns;
using NestedLayerManager.Filters;
using NestedLayerManager.Renderers;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;

namespace NestedLayerManager.Maps
{
    public class HandleMap
    {
        #region Properties and Constructor

        private Hashtable TreeNodeByHandle;

        public HandleMap()
        {
            TreeNodeByHandle = new Hashtable();
        }

        #endregion

        #region Query

        public Boolean ContainsHandle(UIntPtr handle)
        {
            return TreeNodeByHandle.ContainsKey(handle);
        }

        public List<BaseTreeNode> GetTreeNodesByHandle(UIntPtr handle)
        {
            List<BaseTreeNode> treeNodes = TreeNodeByHandle[handle] as List<BaseTreeNode>;
            if (treeNodes == null)
            {
                treeNodes = new List<BaseTreeNode>();
#if DEBUG
                MaxListener.PrintToListener("WARNING: GetTreeNodeByHandle returned null");
#endif
            }
            return treeNodes;
        }

        # endregion

        #region Adding

        public void AddTreeNode(UIntPtr handle, BaseTreeNode treeNode)
        {
            List<BaseTreeNode> treeNodes;

            if (TreeNodeByHandle.ContainsKey(handle))
            {
                treeNodes = GetTreeNodesByHandle(handle) as List<BaseTreeNode>;
                treeNodes.Add(treeNode);
                //TreeNodeByHandle[handle] = treeNodes;
            }
            else
            {
                treeNodes = new List<BaseTreeNode>();
                treeNodes.Add(treeNode);
                TreeNodeByHandle.Add(handle, treeNodes);
            }
        }

        #endregion

        #region Removing

        public void RemoveTreeNodeFromHandle(BaseTreeNode treeNode, UIntPtr handle)
        {
            List<BaseTreeNode> treeNodes = TreeNodeByHandle[handle] as List<BaseTreeNode>;
            if (treeNodes != null)
            {
                treeNodes.Remove(treeNode);
                TreeNodeByHandle[handle] = treeNodes;
            }
        }

        public void ClearAll()
        {
            TreeNodeByHandle.Clear();
        }

        #endregion
    }
}
