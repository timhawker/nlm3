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
using NestedLayerManager.SubControls;
using NestedLayerManager.AspectEngines;
using NestedLayerManager.Interfaces;
using NestedLayerManager.Renderers;
using NestedLayerManager.MaxInteractivity;

namespace NestedLayerManager.Columns
{
    public class ColorColumn : OLVColumn, INLMColumn
    {
        public IAspectEngine AspectEngine { get; set; }

        public ColorColumn(NlmTreeListView listView)
        {
            Text = "Color";
            Width = 40;
            TextAlign = HorizontalAlignment.Center;

            AspectEngine = new ColorAspectEngine(listView, this);

            Renderer = new ColorRenderer();
        }
    }
}
