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
    public class FreezeColumn : OLVColumn, INLMColumn
    {
        public IAspectEngine AspectEngine { get; set; }

        public FreezeColumn(NlmTreeListView listView)
        {
            Text = "Freeze";
            Width = 40;
            TriStateCheckBoxes = true;
            TextAlign = HorizontalAlignment.Center;

            AspectEngine = new FreezeAspectEngine(listView, this);

            Bitmap freezeCheckedBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.FreezeChecked.png"));
            Bitmap freezeUncheckedBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.Unchecked.png"));
            Bitmap freezeIndeterminateBitmap = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.FreezeIndeterminate.png"));

            Renderer = new MappedActiveImageRenderer(
                freezeUncheckedBitmap,
                freezeCheckedBitmap,
                freezeIndeterminateBitmap
            );
        }
    }
}
