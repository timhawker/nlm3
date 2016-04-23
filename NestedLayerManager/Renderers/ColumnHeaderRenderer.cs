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
using NestedLayerManager.Columns;
using NestedLayerManager.Filters;
using NestedLayerManager.Renderers;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.Maps;
using NestedLayerManager.Imaging;
using NestedLayerManager.CellEdit;
using NestedLayerManager.NodeControl;

namespace NestedLayerManager.Renderers
{
    public class ColumnHeaderRenderer
    {
        private NlmTreeListView ListView;

        public ColumnHeaderRenderer(NlmTreeListView listView)
        {
            ListView = listView;

            // Add Event.
            ListView.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(HandleDrawColumnHeader);
        }

        // Draw column header event.
        // Draws column header manually to set theme.
        private void HandleDrawColumnHeader(Object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = false;
            Graphics g = e.Graphics;
            Rectangle b = e.Bounds;

            Point localPoint = ListView.PointToClient(Control.MousePosition);

            Color c = ListView.BackColor;
            int bcU = c.R + 15;
            int bcL1 = c.R + 9;
            int bcL2 = c.R + 4;
            int bU = c.R + 25;
            int bL = c.R - 12;

            if (b.Contains(localPoint))
            {
                if (NlmTreeListView.MouseButtons == MouseButtons.Left)
                {
                    bcU -= 10;
                    bcL1 -= 10;
                    bcL2 -= 10;
                    bU -= 27;
                    bL += 17;
                }
                else
                {
                    bcU += 10;
                    bcL1 += 10;
                    bcL2 += 10;
                }
            }

            Color backColorUpper = Color.FromArgb(bcU, bcU, bcU);
            Color backColorLower1 = Color.FromArgb(bcL1, bcL1, bcL1);
            Color backColorLower2 = Color.FromArgb(bcL2, bcL2, bcL2);
            Color borderUpper = Color.FromArgb(bU, bU, bU);
            Color borderLower = Color.FromArgb(bL, bL, bL);

            int topHeight = 12;
            Rectangle topRect = new Rectangle(b.Left, b.Top, b.Width, topHeight);
            RectangleF bottomRect = new RectangleF(b.Left, b.Top + topHeight, b.Width, b.Height - topHeight);

            using (SolidBrush brush = new SolidBrush(backColorUpper))
            {
                g.FillRectangle(brush, topRect);
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(
                bottomRect,
                backColorLower1,
                backColorLower2,
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, bottomRect);
            }

            ControlPaint.DrawBorder(g, b,
                borderUpper, 1, ButtonBorderStyle.Solid,
                borderUpper, 1, ButtonBorderStyle.Solid,
                borderLower, 1, ButtonBorderStyle.Solid,
                borderLower, 1, ButtonBorderStyle.Solid);

            if (e.ColumnIndex == ListView.NlmColumns.NameColumn.Index)
            {
                TextRenderer.DrawText(g, e.Header.Text, ListView.Font, new Rectangle(b.Left, b.Top, b.Width, b.Height - 2), ListView.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }
            else
            {
                TextRenderer.DrawText(g, e.Header.Text, ListView.Font, new Rectangle(b.Left, b.Top, b.Width, b.Height - 2), ListView.ForeColor);
            }

        }
    }
}
