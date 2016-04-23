using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using NestedLayerManager;
using NestedLayerManager.Events;
using NestedLayerManager.Events.CustomArgs;
using NestedLayerManager.Filters;

namespace NestedLayerManager.SubControls
{
    public class NlmButtonPanelSide : FlowLayoutPanel
    {
        private NlmTreeListView ListView;

        public NlmButton FilterSelectAllButton;
        public NlmButton FilterSelectNoneButton;
        public NlmButton FilterInvertButton;

        public NlmCheckButton BonesCheckButton;
        public NlmCheckButton CameraCheckButton;
        public NlmCheckButton HelperCheckButton;
        public NlmCheckButton LightCheckButton;
        public NlmCheckButton ObjectCheckButton;
        public NlmCheckButton ShapeCheckButton;
        public NlmCheckButton SpaceWarpCheckButton;

        public NlmCheckButton LayerHiddenCheckButton;
        public NlmCheckButton LayerFrozenCheckButton;

        public NlmButtonPanelSide(NlmTreeListView listView)
        {
            ListView = listView;
            listView.ButtonPanelSide = this;

            FlowDirection = FlowDirection.TopDown;
            Margin = new Padding(0);
            Dock = DockStyle.Fill;

            FilterSelectAllButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Filter Select All",
                "NestedLayerManager.Resources.Icons.Buttons.FilterAll.png",
                ListView
            );
            FilterSelectAllButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onFilterSelectAll);
            this.Controls.Add(FilterSelectAllButton);

            FilterSelectNoneButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Filter Select None",
                "NestedLayerManager.Resources.Icons.Buttons.FilterNone.png",
                ListView
            );
            FilterSelectNoneButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onFilterSelectNone);
            this.Controls.Add(FilterSelectNoneButton);

            FilterInvertButton = new NlmButton(
                new Padding(0),
                new Size(22, 22),
                "Filter Invert",
                "NestedLayerManager.Resources.Icons.Buttons.FilterInvert.png",
                ListView
            );
            FilterInvertButton.NlmClick += new EventHandler<ClickEventArgs>(ClickEvents.onFilterInvert);
            this.Controls.Add(FilterInvertButton);

            BonesCheckButton = new NlmCheckButton(
                new Padding(0),
                new Size(22, 22),
                "Bones",
                "NestedLayerManager.Resources.Icons.NodeClass.Bone.png",
                true,
                ListView
            );
            BonesCheckButton.Checked = (listView.ModelFilter as NlmTreeNodeFilterEngine).IsFilterActive(TreeNodeFilter.Bone);
            BonesCheckButton.NlmCheckedChanged += new EventHandler<ClickEventArgs>(ClickEvents.onFilterBones);
            this.Controls.Add(BonesCheckButton);

            CameraCheckButton = new NlmCheckButton(
                new Padding(0),
                new Size(22, 22),
                "Camera",
                "NestedLayerManager.Resources.Icons.NodeClass.Camera.png",
                true,
                ListView
            );
            CameraCheckButton.Checked = (listView.ModelFilter as NlmTreeNodeFilterEngine).IsFilterActive(TreeNodeFilter.Camera);
            CameraCheckButton.NlmCheckedChanged += new EventHandler<ClickEventArgs>(ClickEvents.onFilterCamera);
            this.Controls.Add(CameraCheckButton);

            HelperCheckButton = new NlmCheckButton(
                new Padding(0),
                new Size(22, 22),
                "Helper",
                "NestedLayerManager.Resources.Icons.NodeClass.Helper.png",
                true,
                ListView
            );
            HelperCheckButton.Checked = (listView.ModelFilter as NlmTreeNodeFilterEngine).IsFilterActive(TreeNodeFilter.Helper);
            HelperCheckButton.NlmCheckedChanged += new EventHandler<ClickEventArgs>(ClickEvents.onFilterHelper);
            this.Controls.Add(HelperCheckButton);

            LightCheckButton = new NlmCheckButton(
                new Padding(0),
                new Size(22, 22),
                "Light",
                "NestedLayerManager.Resources.Icons.NodeClass.Light.png",
                true,
                ListView
            );
            LightCheckButton.Checked = (listView.ModelFilter as NlmTreeNodeFilterEngine).IsFilterActive(TreeNodeFilter.Light);
            LightCheckButton.NlmCheckedChanged += new EventHandler<ClickEventArgs>(ClickEvents.onFilterLight);
            this.Controls.Add(LightCheckButton);

            ObjectCheckButton = new NlmCheckButton(
                new Padding(0),
                new Size(22, 22),
                "Object",
                "NestedLayerManager.Resources.Icons.NodeClass.Object.png",
                true,
                ListView
            );
            ObjectCheckButton.Checked = (listView.ModelFilter as NlmTreeNodeFilterEngine).IsFilterActive(TreeNodeFilter.Object);
            ObjectCheckButton.NlmCheckedChanged += new EventHandler<ClickEventArgs>(ClickEvents.onFilterObject);
            this.Controls.Add(ObjectCheckButton);

            ShapeCheckButton = new NlmCheckButton(
                new Padding(0),
                new Size(22, 22),
                "Spline",
                "NestedLayerManager.Resources.Icons.NodeClass.Shape.png",
                true,
                ListView
            );
            ShapeCheckButton.Checked = (listView.ModelFilter as NlmTreeNodeFilterEngine).IsFilterActive(TreeNodeFilter.Shape);
            ShapeCheckButton.NlmCheckedChanged += new EventHandler<ClickEventArgs>(ClickEvents.onFilterShape);
            this.Controls.Add(ShapeCheckButton);

            SpaceWarpCheckButton = new NlmCheckButton(
                new Padding(0),
                new Size(22, 22),
                "SpaceWarp",
                "NestedLayerManager.Resources.Icons.NodeClass.SpaceWarp.png",
                true,
                ListView
            );
            SpaceWarpCheckButton.Checked = (listView.ModelFilter as NlmTreeNodeFilterEngine).IsFilterActive(TreeNodeFilter.SpaceWarp);
            SpaceWarpCheckButton.NlmCheckedChanged += new EventHandler<ClickEventArgs>(ClickEvents.onFilterSpaceWarp);
            this.Controls.Add(SpaceWarpCheckButton);

            LayerHiddenCheckButton = new NlmCheckButton(
                new Padding(0),
                new Size(22, 22),
                "Hidden",
                "NestedLayerManager.Resources.Icons.Buttons.FilterHiddenLayers.png",
                true,
                ListView
            );
            LayerHiddenCheckButton.Checked = (listView.ModelFilter as NlmTreeNodeFilterEngine).IsFilterActive(TreeNodeFilter.LayerHidden);
            LayerHiddenCheckButton.NlmCheckedChanged += new EventHandler<ClickEventArgs>(ClickEvents.onFilterLayerHidden);
            this.Controls.Add(LayerHiddenCheckButton);

            LayerFrozenCheckButton = new NlmCheckButton(
                new Padding(0),
                new Size(22, 22),
                "Frozen",
                "NestedLayerManager.Resources.Icons.Buttons.FilterFrozenLayers.png",
                true,
                ListView
            );
            LayerFrozenCheckButton.Checked = (listView.ModelFilter as NlmTreeNodeFilterEngine).IsFilterActive(TreeNodeFilter.LayerFrozen);
            LayerFrozenCheckButton.NlmCheckedChanged += new EventHandler<ClickEventArgs>(ClickEvents.onFilterLayerFrozen);
            this.Controls.Add(LayerFrozenCheckButton);
        }
    }
}
