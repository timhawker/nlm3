using System;
using NestedLayerManager.SubControls;
using NestedLayerManager.Filters;

namespace NestedLayerManager.Events.CustomArgs
{
    public class SearchBarEventArgs : EventArgs
    {
        public String Text;
        public NlmTreeListView ListView;

        public SearchBarEventArgs(NlmTreeListView listView, String text)
        {
            ListView = listView;
            Text = text;
        }
    }
}
