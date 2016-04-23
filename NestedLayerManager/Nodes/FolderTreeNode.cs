using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using NestedLayerManager.Maps;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.IO.Data;

namespace NestedLayerManager.Nodes
{
    public class FolderTreeNode : BaseTreeNode
    {
        public String Name;
        public Color? Color;

        public FolderTreeNode(String name)
        {
            Name = name;
            Random random = new Random();
            Color = System.Drawing.Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
        }

        public FolderTreeNode(FolderData data)
        {
            Name = data.Name;
            Visible = data.Visible;
            Freeze = data.Freeze;
            Render = data.Render;
            Color = data.Color;
            Box = data.Box;
            WasExpanded = data.Expanded;
            ID = data.ID;
        }
    }
}