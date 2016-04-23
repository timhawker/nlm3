using System;
using NestedLayerManager.SubControls;

namespace NestedLayerManager.Events.CustomArgs
{
    public class ClickEventArgs : EventArgs
    {
        public ClickEventArgs(NlmTreeListView listView)
        {
            ListView = listView;
        }
        public NlmTreeListView ListView { get; set; }
    }
}
