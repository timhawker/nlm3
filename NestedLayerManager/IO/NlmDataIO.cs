using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NestedLayerManager;
using NestedLayerManager.SubControls;
using NestedLayerManager.Filters;
using NestedLayerManager.IO.Data;

namespace NestedLayerManager.IO
{
    public static class NlmDataIO
    {
        public static void RestoreNlmData(NlmTreeListView listView, NlmData nlmData)
        {
            listView.SmartFolderMode = nlmData.SmartFolderNode;
            NlmTreeNodeFilterEngine FilterEngine = listView.ModelFilter as NlmTreeNodeFilterEngine;

            if (nlmData.BonesFiltered)
                FilterEngine.AddFilter(TreeNodeFilter.Bone);

            if (nlmData.CameraFiltered)
                FilterEngine.AddFilter(TreeNodeFilter.Camera);

            if (nlmData.HelperFiltered)
                FilterEngine.AddFilter(TreeNodeFilter.Helper);

            if (nlmData.LightFiltered)
                FilterEngine.AddFilter(TreeNodeFilter.Light);

            if (nlmData.ObjectFiltered)
                FilterEngine.AddFilter(TreeNodeFilter.Object);

            if (nlmData.ShapeFiltered)
                FilterEngine.AddFilter(TreeNodeFilter.Shape);

            if (nlmData.SpaceWarpFiltered)
                FilterEngine.AddFilter(TreeNodeFilter.SpaceWarp);

            if (nlmData.LayerHiddenFiltered)
                FilterEngine.AddFilter(TreeNodeFilter.LayerHidden);

            if (nlmData.LayerFrozenFiltered)
                FilterEngine.AddFilter(TreeNodeFilter.LayerFrozen);
        }
    }
}
