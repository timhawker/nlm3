using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using NestedLayerManager.SubControls;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.Filters;

namespace NestedLayerManager.IO.Data
{
    [Serializable]
    public class NlmData
    {
        public Boolean SmartFolderNode;

        public Boolean BonesFiltered;
        public Boolean CameraFiltered;
        public Boolean HelperFiltered;
        public Boolean LightFiltered;
        public Boolean ObjectFiltered;
        public Boolean ShapeFiltered;
        public Boolean SpaceWarpFiltered;
        public Boolean LayerHiddenFiltered;
        public Boolean LayerFrozenFiltered;


        public NlmData(NlmTreeListView listView)
        {
            SmartFolderNode = listView.SmartFolderMode;

            NlmTreeNodeFilterEngine FilterEngine = listView.ModelFilter as NlmTreeNodeFilterEngine;

            BonesFiltered = FilterEngine.IsFilterActive(TreeNodeFilter.Bone);
            CameraFiltered = FilterEngine.IsFilterActive(TreeNodeFilter.Camera);
            HelperFiltered = FilterEngine.IsFilterActive(TreeNodeFilter.Helper);
            LightFiltered = FilterEngine.IsFilterActive(TreeNodeFilter.Light);
            ObjectFiltered = FilterEngine.IsFilterActive(TreeNodeFilter.Object);
            ShapeFiltered = FilterEngine.IsFilterActive(TreeNodeFilter.Shape);
            SpaceWarpFiltered = FilterEngine.IsFilterActive(TreeNodeFilter.SpaceWarp);
            LayerHiddenFiltered = FilterEngine.IsFilterActive(TreeNodeFilter.LayerHidden);
            LayerFrozenFiltered = FilterEngine.IsFilterActive(TreeNodeFilter.LayerFrozen);
        }
    }
}
