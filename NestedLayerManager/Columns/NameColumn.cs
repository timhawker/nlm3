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
using NestedLayerManager.Renderers;
using NestedLayerManager.SubControls;
using NestedLayerManager.AspectEngines;
using NestedLayerManager.Interfaces;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.Nodes;

namespace NestedLayerManager.Columns
{
    public class NameColumn : OLVColumn, INLMColumn
    {
        public IAspectEngine AspectEngine { get; set; }

        public NameColumn(NlmTreeListView listView)
        {
            Text = "Name";
            IsEditable = true;
            Searchable = true;
            FillsFreeSpace = true;

            ImageGetter = delegate(Object rowObject)
            {
                LayerTreeNode layerTreeNode = rowObject as LayerTreeNode;
                if (layerTreeNode != null)
                {
                    if (layerTreeNode.IsInstanced)
                    {
                        return "LayerInstance";
                    }
                    else
                    {
                        return "Layer";
                    }
                }
                ObjectTreeNode objectTreeNode = rowObject as ObjectTreeNode;
                if (objectTreeNode != null)
                {
                    return objectTreeNode.Class.ToString();
                }
                if (rowObject is FolderTreeNode)
                {
                    return "Folder";
                }
                return null;
            };

            AspectEngine = new NameAspectEngine(listView, this);
            Renderer = new NlmTreeColumnRenderer();
        }
    }
}
