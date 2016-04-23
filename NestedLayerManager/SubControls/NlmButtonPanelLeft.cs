using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using NestedLayerManager;
using NestedLayerManager.Events;
using NestedLayerManager.Events.CustomArgs;

namespace NestedLayerManager.SubControls
{
    public class NlmButtonPanelLeft : FlowLayoutPanel
    {
        private NlmTreeListView ListView;

        public NlmButton CreateLayerButton;
        public NlmButton InstanceLayersButton;
        public NlmButton CreateFolderButton;
        public NlmCheckButton SmartFolderModeCheckButton;
        public NlmCheckButton IsolateSelectedButton;
        public NlmButton DeleteSelectedButton;
        public NlmButton AddSelectedObjectsToLayerButton;
        public NlmButton SelectObjectsFromHighlightButton;
        public NlmButton SelectLayersFromSelectedObjectsButton;
        public NlmButton HideUnhideAllButton;
        public NlmButton FreezeUnfreezeAllButton;
        public NlmButton CollapseExpandAllButton;

        public NlmButtonPanelLeft(NlmTreeListView listView)
        {
            ListView = listView;
            listView.ButtonPanelLeft = this;

            Margin = new Padding(0);
            Dock = DockStyle.Fill;

            CreateLayerButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Create Layer",
                "NestedLayerManager.Resources.Icons.Buttons.CreateLayer.png",
                listView
            );
            CreateLayerButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onCreateLayerAddSelection);
            this.Controls.Add(CreateLayerButton);

            InstanceLayersButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Instance Layer(s)",
                "NestedLayerManager.Resources.Icons.Buttons.InstanceLayer.png",
                listView
            );
            InstanceLayersButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onInstanceSelectedLayers);
            this.Controls.Add(InstanceLayersButton);

            CreateFolderButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Create Folder",
                "NestedLayerManager.Resources.Icons.Buttons.CreateFolder.png",
                listView
            );
            CreateFolderButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onCreateFolder);
            this.Controls.Add(CreateFolderButton);

            SmartFolderModeCheckButton = new NlmCheckButton(
                new Padding(0),
                new Size(22, 22),
                "Smart Folder Mode",
                "NestedLayerManager.Resources.Icons.Buttons.SmartFolderMode.png",
                false,
                listView
            );
            SmartFolderModeCheckButton.Checked = listView.SmartFolderMode;
            SmartFolderModeCheckButton.NlmCheckedChanged += new EventHandler<ClickEventArgs>(ClickEvents.onSmartFolderMode);
            this.Controls.Add(SmartFolderModeCheckButton);

            IsolateSelectedButton = new NlmCheckButton(
                new Padding(0),
                new Size(22, 22),
                "Isolate Selection",
                "NestedLayerManager.Resources.Icons.Buttons.IsolateSelection.png",
                false,
                listView
            );
            IsolateSelectedButton.NlmCheckedChanged += new EventHandler<ClickEventArgs>(ClickEvents.onIsolateSelection);
            this.Controls.Add(IsolateSelectedButton);

            DeleteSelectedButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Delete Selected",
                "NestedLayerManager.Resources.Icons.Buttons.DeleteSelected.png",
                listView
            );
            DeleteSelectedButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onDeleteSelection);
            this.Controls.Add(DeleteSelectedButton);

            AddSelectedObjectsToLayerButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Add Selected Objects To Layer",
                "NestedLayerManager.Resources.Icons.Buttons.AddSelectedObjectsToLayer.png",
                listView
            );
            AddSelectedObjectsToLayerButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onAddSelectedObjectsToLayer);
            this.Controls.Add(AddSelectedObjectsToLayerButton);

            SelectObjectsFromHighlightButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Select Objects Within Highlighted Selection",
                "NestedLayerManager.Resources.Icons.Buttons.SelectObjectsFromHighlight.png",
                listView
            );
            SelectObjectsFromHighlightButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onSelectObjectsFromHighlight);
            this.Controls.Add(SelectObjectsFromHighlightButton);

            SelectLayersFromSelectedObjectsButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Select Layers Of Selected Objects",
                "NestedLayerManager.Resources.Icons.Buttons.SelectLayersFromSelectedObjects.png",
                listView
            );
            SelectLayersFromSelectedObjectsButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onSelectLayersFromSelectedObjects);
            this.Controls.Add(SelectLayersFromSelectedObjectsButton);

            HideUnhideAllButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Hide / Unhide All",
                "NestedLayerManager.Resources.Icons.Buttons.HideUnhideAll.png",
                listView
            );
            HideUnhideAllButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onHideUnhideAll);
            this.Controls.Add(HideUnhideAllButton);

            FreezeUnfreezeAllButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Freeze / Unfreeze All",
                "NestedLayerManager.Resources.Icons.Buttons.FreezeUnfreezeAll.png",
                listView
            );
            FreezeUnfreezeAllButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onFreezeUnfreezeAll);
            this.Controls.Add(FreezeUnfreezeAllButton);

            CollapseExpandAllButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Collapse / ExpandAll",
                "NestedLayerManager.Resources.Icons.Buttons.CollapseExpandAll.png",
                listView
            );
            CollapseExpandAllButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onCollapseExpandAll);
            this.Controls.Add(CollapseExpandAllButton);
        }
    }
}
