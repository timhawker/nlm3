using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using MaxCustomControls;
using NestedLayerManager.MaxInteractivity.MaxCUI;

namespace NestedLayerManager.SubControls
{
    public class NlmAboutWindow : MaxForm
    {
        public NlmAboutWindow(Control parent)
        {
            // Create header image and add to control.
            Bitmap NlmAboutHeader = new Bitmap(typeof(NlmTreeListView).Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Logos.NestedLayerManagerLogo.png"));
            PictureBox picBox = new PictureBox();
            picBox.Image = NlmAboutHeader;
            picBox.Size = NlmAboutHeader.Size;
            this.Controls.Add(picBox);

            // Set window properties.
            this.Text = MaxCUIProperties.WindowTitle + " | About";
            this.ClientSize = new Size(NlmAboutHeader.Width, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.Manual;
            Point parentLocation = parent.PointToScreen(Point.Empty);
            this.Left = parentLocation.X + (parent.ClientSize.Width / 2) - (this.ClientSize.Width / 2);
            this.Top = parentLocation.Y + (parent.ClientSize.Height / 2) - (this.ClientSize.Height / 2);

            // Create label, set properties and add to control.
            Label testLabel = new Label();
            testLabel.Text = String.Join(
                Environment.NewLine,
                MaxCUIProperties.WindowTitle + " " + MaxCUIProperties.Version,
                Environment.NewLine,
                "All content copyright 2014 Tim Hawker. Any trade names or logos shown are the trademarks of their respective owners.",
                Environment.NewLine,
                "www.timsportfolio.co.uk"
            );
            testLabel.Width = this.ClientSize.Width - 10;
            testLabel.Height = this.ClientSize.Height - NlmAboutHeader.Height;
            testLabel.Location = new Point(5, NlmAboutHeader.Height);
            testLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(testLabel);
        }
    }
}
