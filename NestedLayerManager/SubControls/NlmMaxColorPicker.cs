using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ManagedServices;
using BrightIdeasSoftware;
using NestedLayerManager.Nodes.Base;

namespace NestedLayerManager.SubControls
{
    public class NlmMaxColorPicker : MaxColorPicker
    {
        private NlmTreeListView ListView;
        private List<BaseTreeNode> TreeNodes;

        public NlmMaxColorPicker(OlvListViewHitTestInfo hti, Boolean applyToSelection, NlmTreeListView listView)
        {
            ListView = listView;

            this.CurrentColor = (Color)ListView.NlmColumns.ColorColumn.GetValue(hti.RowObject);

            TreeNodes = new List<BaseTreeNode>();
            if (applyToSelection)
            {
                TreeNodes.AddRange(listView.NodeControl.Query.SelectedNodes);
            }
            else
            {
                TreeNodes.Add(hti.RowObject as BaseTreeNode);
            }

            this.ColorConfirmed += new ConfirmColorEventHandler(onColorChanged);
        }

        private void onColorChanged(Object sender, EventArgs e)
        {
            Color newColor = this.CurrentColor;
            if (!newColor.IsEmpty)
            {
                foreach (BaseTreeNode treeNode in TreeNodes)
                {
                    ListView.NlmColumns.ColorColumn.PutValue(treeNode, newColor);
                }
                ListView.RefreshObjects(TreeNodes);
            }
        }
    }
}
