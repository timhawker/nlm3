using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using NestedLayerManager.Maps;
using NestedLayerManager.MaxInteractivity;

namespace NestedLayerManager.Nodes.Base
{
    public class BaseTreeNode
    {
        public Guid ID;

        public Boolean? Visible;
        public Boolean? Freeze;
        public Boolean? Render;
        public Boolean? Box;
        public Boolean WasExpanded;

        public BaseTreeNode Parent;
        public List<BaseTreeNode> Children;

        /// <summary>
        /// Checks whether this node an ancestor of the provided node.
        /// </summary>
        /// <param name="baseNode">The potential ancestor node.</param>
        /// <returns>True if this node is an ancestor, False if not.</returns>
        public bool IsAncestor(BaseTreeNode baseNode)
        {
            if (this == baseNode)
                return true;
            if (this.Parent == null)
                return false;
            return this.Parent.IsAncestor(baseNode);
        }

        public BaseTreeNode()
        {
            ID = Guid.NewGuid();
            Children = new List<BaseTreeNode>();
        }
    }
}
