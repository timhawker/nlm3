using System;
using BrightIdeasSoftware;
using System.Drawing;

namespace NestedLayerManager.Renderers
{
    internal class ColorRenderer : BaseRenderer
    {

        public ColorRenderer()
            : base() {}

        public override void Render(Graphics g, Rectangle r)
        {
            this.DrawBackground(g, r);

            if (this.Aspect == null)
                return;

            int padding = 5;
            int maximumWidth = 24;
            int maximumHeight = 12;

            r = this.ApplyCellPadding(r);

            Rectangle frameRect = Rectangle.Inflate(r, 0 - padding, 0 - padding);
            frameRect.Width = Math.Min(frameRect.Width, maximumWidth);
            frameRect.Height = Math.Min(frameRect.Height, maximumHeight);
            frameRect = AlignRectangle(r, frameRect);

            Brush brush;
            Pen pen;
            if (this.ListItem.Enabled)
            {
                // If the listItem is enabled, draw normally.
                Color color = (Color)this.Aspect;
                brush = new SolidBrush(color);
                pen = new Pen(Color.FromArgb(50, 50, 50), 1);
            }
            else
            {
                // If the listItem is disabled, calculate the greyscale colour and draw with 50% opacity.
                Color color = (Color)this.Aspect;
                int colorAverage = (color.R + color.G + color.B) / 3;
                brush = new SolidBrush(Color.FromArgb(128, colorAverage, colorAverage, colorAverage));
                pen = new Pen(Color.FromArgb(50, 50, 50, 50));
            }

            g.FillRectangle(brush, frameRect);
            g.DrawRectangle(pen, frameRect);        
        }

        /// Handle the GetEditRectangle request.
        protected override Rectangle HandleGetEditRectangle(Graphics g, Rectangle cellBounds, OLVListItem item, int subItemIndex, Size preferredSize)
        {
            return this.CalculatePaddedAlignedBounds(g, cellBounds, preferredSize);
        }
    }
}
