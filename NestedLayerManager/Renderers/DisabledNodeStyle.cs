using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BrightIdeasSoftware;
using NestedLayerManager.SubControls;

namespace NestedLayerManager.Renderers
{
    public class DisabledNodeStyle : SimpleItemStyle
    {
        public DisabledNodeStyle(NlmTreeListView listView)
        {
            Color disableForeColor = Color.FromArgb(
                (listView.ForeColor.R / 2) + (listView.BackColor.R / 2),
                (listView.ForeColor.G / 2) + (listView.BackColor.G / 2),
                (listView.ForeColor.B / 2) + (listView.BackColor.B / 2)
                );
            ForeColor = disableForeColor;
        }
    }
}
