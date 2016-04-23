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
using NestedLayerManager.MaxInteractivity;

namespace NestedLayerManager.Columns
{
    public class CurrentColumn : OLVColumn, INLMColumn
    {
        public IAspectEngine AspectEngine { get; set; }

        public CurrentColumn(NlmTreeListView listView)
        {
            Text = "Current";
            Width = 40;
            TriStateCheckBoxes = true;
            TextAlign = HorizontalAlignment.Center;

            AspectEngine = new CurrentAspectEngine(this, listView);

            Renderer = new MappedImageRenderer(new Object[] { 
                true, new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.CurrentChecked.png")), 
                false, null,
                null, new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.CheckState.CurrentIndeterminate.png")),
            });
        }
    }
}
