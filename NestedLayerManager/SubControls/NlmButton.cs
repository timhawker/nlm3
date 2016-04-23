using System;
using System.Windows.Forms;
using System.Drawing;
using NestedLayerManager.Events;
using NestedLayerManager.Events.CustomArgs;

namespace NestedLayerManager.SubControls
{
    // Button classes.
    public class NlmButton : Button
    {
        private ToolTip Tt;
        private String ToolTipText;
        private Bitmap Icon;
        private NlmTreeListView ListView;
        public event EventHandler<ClickEventArgs> NlmClick;

        public NlmButton(Padding padding, Size size, String toolTipText, String iconDir, NlmTreeListView listView)
        {
            ListView = listView;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.FromArgb(0, 255, 255, 255);
            FlatAppearance.BorderColor = Color.FromArgb(43, 120, 197);
            FlatAppearance.MouseDownBackColor = Color.FromArgb(154, 184, 225);
            FlatAppearance.MouseOverBackColor = Color.FromArgb(174, 204, 235);
            ImageAlign = ContentAlignment.MiddleCenter;
            Size = size;
            Margin = padding;
            Icon = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream(iconDir));
            BackgroundImage = this.Icon;
            BackgroundImageLayout = ImageLayout.Center;
            ToolTipText = toolTipText;
            SetStyle(ControlStyles.Selectable, false);
            MouseEnter += new EventHandler(ButtonMouseEnter);
            MouseLeave += new EventHandler(ButtonMouseLeave);
            MouseDown += new MouseEventHandler(ButtonMouseDown);
            MouseUp += new MouseEventHandler(ButtonMouseUp);
            Click += new EventHandler(ButtonClick);
        }

        private void ButtonClick(Object o, EventArgs e)
        {
            if (NlmClick != null)
                NlmClick(this, new ClickEventArgs(ListView));
        }

        private void ButtonMouseEnter(Object sender, EventArgs e)
        {
            Button Button = sender as Button;
            this.FlatAppearance.BorderSize = 1;
            // Tooltips are created on mouse enter and removed on mouse leave. 
            // This is due to a bug in 3ds Max where child windows are sent behind main window after showing a tooltip.
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this, this.ToolTipText);
            Tt = tt;
        }

        private void ButtonMouseLeave(Object sender, EventArgs e)
        {
            this.FlatAppearance.BorderSize = 0;
            Tt.RemoveAll();
            Tt.Dispose();
        }

        private void ButtonMouseDown(Object sender, EventArgs e)
        {
            this.BackgroundImage = null;
            this.Image = this.Icon;
        }

        private void ButtonMouseUp(Object sender, EventArgs e)
        {
            this.Image = null;
            this.BackgroundImage = this.Icon;
        }
    }
}
