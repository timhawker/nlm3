using System.Drawing;
using BrightIdeasSoftware;
using NestedLayerManager.MaxInteractivity;

using System;

namespace NestedLayerManager.Renderers
{
    internal class NlmTreeColumnRenderer : TreeListView.TreeRenderer
    {
        private Bitmap Expand;
        private Bitmap Collapse;

        internal NlmTreeColumnRenderer()
        {
            this.Expand = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.TreeListView.TreeExpanded.png"));
            this.Collapse = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.TreeListView.TreeCollapsed.png"));
            this.IsShowLines = false;

            // The TreeRenderer inherits a HighlightTextRenderer for filtering.
            // Customise the colours here.

            Color hightlightColor = MaxLook.GetHighlightColor();

            //this.FramePen = new Pen(Color.FromArgb(43, 120, 197));
            //this.FillBrush = new SolidBrush(Color.FromArgb(100, 43, 120, 197));
            this.FramePen = new Pen(hightlightColor);
            this.FillBrush = new SolidBrush(Color.FromArgb(128, hightlightColor));
        }

        protected override int DrawImage(Graphics g, Rectangle r, Object imageSelector)
        {
            if (!ListItem.Enabled && imageSelector is String)
                imageSelector += "Disabled";
            return base.DrawImage(g, r, imageSelector);
        }

        // The standard behaviour of the treelistview has a delayed render for the left side of the expand icons.
        // This is made worse in 3ds Max that takes a second mouse click for that area to update.
        // There is also a bug with the imageList icons in a ListView where the alpha is not calculated properly.
        // This solves both problems by always rendering the indent area, expand area, and imageList area with the background colour.
        // Annoyingly the bit we add is right in the middle of the function, so this function was lifted entirely from base class.

        // Commented out with OLV 2.8.0 update as this is now fixed.

        //public override void Render(System.Drawing.Graphics g, System.Drawing.Rectangle r)
        //{
        //    this.DrawBackground(g, r);

        //    TreeListView.Branch br = this.TreeListView.TreeModel.GetBranch(this.RowObject);

        //    Rectangle paddedRectangle = this.ApplyCellPadding(r);

        //    Rectangle expandGlyphRectangle = paddedRectangle;
        //    expandGlyphRectangle.Offset((br.Level - 1) * PIXELS_PER_LEVEL, 0);
        //    expandGlyphRectangle.Width = PIXELS_PER_LEVEL;
        //    expandGlyphRectangle.Height = PIXELS_PER_LEVEL;
        //    expandGlyphRectangle.Y = this.AlignVertically(paddedRectangle, expandGlyphRectangle);
        //    int expandGlyphRectangleMidVertical = expandGlyphRectangle.Y + (expandGlyphRectangle.Height / 2);

        //    // Add the background drawing we want here.
        //    DrawIndentBackground(g, r, br);

        //    if (this.IsShowLines)
        //        this.DrawLines(g, r, this.LinePen, br, expandGlyphRectangleMidVertical);

        //    if (br.CanExpand)
        //        this.DrawExpansionGlyph(g, expandGlyphRectangle, br.IsExpanded);

        //    int indent = br.Level * PIXELS_PER_LEVEL;
        //    paddedRectangle.Offset(indent, 0);
        //    paddedRectangle.Width -= indent;

        //    this.DrawImageAndText(g, paddedRectangle);
        //}

        // The windows expand/collapse icons look crap on a dark background.
        // Let's get rid of them and put something there that looks nicer :)
        protected override void DrawExpansionGlyphStyled(Graphics g, Rectangle r, bool isExpanded) 
        {
            int x = r.X + (this.Expand.Width / 2) - 2;
            int y = r.Y + (this.Expand.Height / 2) - 2;

            if (isExpanded)
            {
                g.DrawImage(this.Expand, x, y);
            }
            else
            {
                g.DrawImage(this.Collapse, x, y);
            }
        }

        private void DrawIndentBackground(Graphics g, Rectangle r, TreeListView.Branch br)
        {
            using (Brush brush = new SolidBrush(ListView.BackColor))
            {
                g.FillRectangle(brush, r.X - 1, r.Y - 1, (br.Level * PIXELS_PER_LEVEL) + 20, r.Height + 2);
            }
        }
    }
}
