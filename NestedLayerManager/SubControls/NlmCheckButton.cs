using System;
using System.Windows.Forms;
using System.Drawing;
using BrightIdeasSoftware;
using NestedLayerManager.Imaging;
using NestedLayerManager.Events;
using NestedLayerManager.Events.CustomArgs;
using NestedLayerManager.Filters;

namespace NestedLayerManager.SubControls
{
    public class NlmCheckButton : CheckBox
    {
        private ToolTip Tt;
        private String ToolTipText;
        private Bitmap IconFull;
        private Bitmap IconHalf;
        private Boolean FadeOnChecked;
        public NlmTreeListView ListView;
        public event EventHandler<ClickEventArgs> NlmCheckedChanged;

        public NlmCheckButton(Padding padding, Size size, String toolTipText, String iconDir, Boolean fadeOnChecked, NlmTreeListView listView)
        {
            ListView = listView;

            IconFull = new Bitmap(GetType().Module.Assembly.GetManifestResourceStream(iconDir));
            IconHalf = BitmapTools.ChangeOpacity((IconFull), (float)0.3);

            Appearance = Appearance.Button;
            FlatStyle = FlatStyle.Flat;
            BackColor = Color.FromArgb(0, 255, 255, 255);
            FlatAppearance.BorderSize = 0;
            FlatAppearance.BorderColor = Color.FromArgb(43, 120, 197);
            FlatAppearance.MouseDownBackColor = Color.FromArgb(154, 184, 225);
            FlatAppearance.MouseOverBackColor = Color.FromArgb(174, 204, 235);
            FlatAppearance.CheckedBackColor = Color.FromArgb(174, 204, 235);
            Size = size;
            ImageAlign = ContentAlignment.MiddleCenter;
            BackgroundImage = IconFull;
            BackgroundImageLayout = ImageLayout.Center;
            Margin = padding;
            Padding = new Padding(0);
            ToolTipText = toolTipText;
            FadeOnChecked = fadeOnChecked;
            SetStyle(ControlStyles.Selectable, false);

            MouseEnter += new EventHandler(ButtonMouseEnter);
            MouseLeave += new EventHandler(ButtonMouseLeave);
            MouseDown += new MouseEventHandler(ButtonMouseDown);
            MouseUp += new MouseEventHandler(ButtonMouseUp);
            CheckedChanged += new EventHandler(CheckChanged);
        }

        private void ButtonMouseEnter(Object sender, EventArgs e)
        {
            CheckBox Button = sender as CheckBox;
            FlatAppearance.BorderSize = 1;

            if (Button.Checked)
            {
                FlatAppearance.MouseOverBackColor = Color.FromArgb(194, 224, 255);
            }
            else
            {
                FlatAppearance.MouseOverBackColor = Color.FromArgb(174, 204, 235);
            }

            // Tooltips are created on mouse enter and removed on mouse leave. 
            // This is due to a bug in 3ds Max where child windows are sent behind main window after showing a tooltip.
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this, ToolTipText);
            Tt = tt;
        }

        private void ButtonMouseLeave(Object sender, EventArgs e)
        {
            CheckBox Button = sender as CheckBox;
            if (!Button.Checked)
            {
                FlatAppearance.BorderSize = 0;
            }
            Tt.RemoveAll();
            Tt.Dispose();
        }

        private void ButtonMouseDown(Object sender, EventArgs e)
        {
            BackgroundImage = null;
            if (FadeOnChecked && Checked)
            {
                Image = IconHalf;
            }
            else
            {
                Image = IconFull;
            }
        }

        private void ButtonMouseUp(Object sender, EventArgs e)
        {
            Image = null;
            if (FadeOnChecked && Checked)
            {
                BackgroundImage = IconHalf;
            }
            else
            {
                BackgroundImage = IconFull;
            }
        }

        private void CheckChanged(Object sender, EventArgs e)
        {
            if (Checked)
            {
                FlatAppearance.BorderSize = 1;
                Image = null;
                if (FadeOnChecked)
                    BackgroundImage = IconHalf;
            }
            else
            {
                FlatAppearance.BorderSize = 0;
                Image = null;
                if (FadeOnChecked)
                    BackgroundImage = IconFull;
            }
            if (NlmCheckedChanged != null)
                NlmCheckedChanged(this, new ClickEventArgs(ListView));
        }
    }
}
