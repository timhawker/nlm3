using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NestedLayerManager;
using NestedLayerManager.Events;
using NestedLayerManager.Events.CustomArgs;

namespace NestedLayerManager.SubControls
{
    public class NlmButtonPanelRight : FlowLayoutPanel
    {
        private NlmTreeListView ListView;

        public NlmButton InformationButton;
        public NlmButton SettingsButton;

        public NlmButtonPanelRight(NlmTreeListView listView)
        {
            ListView = listView;
            listView.ButtonPanelRight = this;

            FlowDirection = FlowDirection.RightToLeft;
            Anchor = AnchorStyles.Right;
            Margin = new Padding(0);
            Dock = DockStyle.Fill;

            InformationButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Information",
                "NestedLayerManager.Resources.Icons.Buttons.Information.png",
                ListView
            );
            InformationButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onInformation);
            this.Controls.Add(InformationButton);

            SettingsButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Settings",
                "NestedLayerManager.Resources.Icons.Buttons.Settings.png",
                ListView
            );
            SettingsButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onSettings);
            this.Controls.Add(SettingsButton);
        }
    }
}
