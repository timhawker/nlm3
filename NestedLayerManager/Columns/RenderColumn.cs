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
using NestedLayerManager.Interfaces;
using NestedLayerManager.AspectEngines;
using NestedLayerManager.Renderers;
using NestedLayerManager.Imaging;
using NestedLayerManager.MaxInteractivity;

namespace NestedLayerManager.Columns
{
    public class RenderColumn : OLVColumn, INLMColumn
    {
        public IAspectEngine AspectEngine { get; set; }

        public RenderColumn(NlmTreeListView listView)
        {
            Text = "Render";
            Width = 40;
            TriStateCheckBoxes = true;
            TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            AspectEngine = new RenderAspectEngine(listView, this);

            Bitmap renderCheckedBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.RenderChecked.png"));
            Bitmap renderUncheckedBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.Unchecked.png"));
            Bitmap renderIndeterminateBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.RenderIndeterminate.png"));

            Renderer = new MappedActiveImageRenderer(
                renderCheckedBitmap,
                renderUncheckedBitmap,
                renderIndeterminateBitmap
            );
        }
    }
}
