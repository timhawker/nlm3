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
    public class BoxColumn : OLVColumn, INLMColumn
    {
        public IAspectEngine AspectEngine { get; set; }

        public BoxColumn(NlmTreeListView listView)
        {
            Text = "Box";
            Width = 40;
            TriStateCheckBoxes = true;
            TextAlign = HorizontalAlignment.Center;

            AspectEngine = new BoxAspectEngine(listView, this);

            Bitmap boxCheckedBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.BoxChecked.png"));
            Bitmap boxUncheckedBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.Unchecked.png"));
            Bitmap boxIndeterminateBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.BoxIndeterminate.png"));

            Renderer = new MappedActiveImageRenderer(
                boxUncheckedBitmap,
                boxCheckedBitmap,
                boxIndeterminateBitmap
            );
        }
    }
}
