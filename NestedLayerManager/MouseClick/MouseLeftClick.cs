using System;
using System.Windows.Forms;
using BrightIdeasSoftware;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.Interfaces;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;

namespace NestedLayerManager.MouseClick
{
    public class MouseLeftClick
    {
        NlmTreeListView ListView;

        public MouseLeftClick(NlmTreeListView listView)
        {
            ListView = listView;
        }

        public Boolean Click(OlvListViewHitTestInfo hti)
        {
            if (hti.ColumnIndex > 0)
            {
                if (hti.Column == ListView.NlmColumns.ColorColumn)
                {
                    // Should the check states be changed on the selection, or on an item outside of selection.
                    // If inside selection, suppress change in selection by returning true.
                    // If outside selection, change the single item and return false.
                    if (hti.Item.Selected)
                    {
                        NlmMaxColorPicker colorPicker = new NlmMaxColorPicker(hti, true, ListView);
                        colorPicker.ShowModeless();
                        return true;
                    }
                    else
                    {
                        NlmMaxColorPicker colorPicker = new NlmMaxColorPicker(hti, false, ListView);
                        colorPicker.ShowModeless();
                        // Returning false annoyingly pushes the window to the back.
                        // TODO: Somehow make this dialog modal, or at least parented to 3ds Max, and sort out the focus issue.
                        return true;
                    }
                }
                if (hti.Column == ListView.NlmColumns.CurrentColumn)
                {
                    BaseTreeNode treeNode = hti.RowObject as BaseTreeNode;
                    if (treeNode is LayerTreeNode)
                    {
                        ListView.BeginUpdate();
                        hti.Column.PutValue(treeNode, true);
                        ListView.EndUpdate();
                    }
                    return hti.Item.Selected;
                }
                else
                {
                    Boolean htiItemSelected = hti.Item.Selected;
                    INLMColumn column = hti.Column as INLMColumn;

                    // Should the check states be changed on the selection, or on an item outside of selection.
                    // If inside selection, suppress change in selection by returning true.
                    // If outside selection, change the single item and return false.
                    if (htiItemSelected)
                    {
                        column.AspectEngine.ToggleCheckStates(hti.RowObject);
                        ListView.RefreshObjects(ListView.SelectedObjects);
                    }
                    else
                    {
                        column.AspectEngine.ToggleCheckState(hti.RowObject);
                        ListView.RefreshObject(hti.RowObject);
                    }

                    MaxUI.RedrawViewportsNow();
                    return htiItemSelected;
                }
            }
            return false;
            // Returning true means selection does not change, returning false means it does.
        }
    }
}
