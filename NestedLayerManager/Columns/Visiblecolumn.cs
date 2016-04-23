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
using NestedLayerManager.Imaging;
using NestedLayerManager.MaxInteractivity;

namespace NestedLayerManager.Columns
{
    public class VisibleColumn : OLVColumn, INLMColumn
    {
        public IAspectEngine AspectEngine { get; set; }

        public VisibleColumn(NlmTreeListView listView)
        {
            Text = "Visible";
            Width = 40;
            TriStateCheckBoxes = true;
            TextAlign = HorizontalAlignment.Center;

            AspectEngine = new VisibleAspectEngine(listView, this);

            Bitmap visibleCheckedBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.VisibleChecked.png"));
            Bitmap visibleUncheckedBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.VisibleUnchecked.png"));
            Bitmap visibleIndeterminateBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.VisibleIndeterminate.png"));
            
            Renderer = new MappedActiveImageRenderer(
                visibleCheckedBitmap,
                visibleUncheckedBitmap,
                visibleIndeterminateBitmap
            );
        }
    }
}
