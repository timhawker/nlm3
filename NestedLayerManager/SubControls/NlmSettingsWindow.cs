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
    public class NlmSettingsWindow : MaxForm
    {
        public NlmSettingsWindow(Control parent)
        {
            // Set window properties.
            this.Text = MaxCUIProperties.WindowTitle + " | Settings";
            this.ClientSize = new Size(250, 250);
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
                "Nothing much to see here at the moment. Stay tuned :)"
            );
            testLabel.Width = this.ClientSize.Width;
            testLabel.Height = this.ClientSize.Height;
            testLabel.Location = new Point(0,0);
            testLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(testLabel);
        }
    }
}
