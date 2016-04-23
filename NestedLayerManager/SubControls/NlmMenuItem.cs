using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NestedLayerManager.Events;
using NestedLayerManager.Events.CustomArgs;

namespace NestedLayerManager.SubControls
{
    public class NlmMenuItem : MenuItem
    {
        private NlmTreeListView ListView;
        public event EventHandler<ClickEventArgs> NlmClick;

        public NlmMenuItem(NlmTreeListView listView, String text, EventHandler<ClickEventArgs> clickEvents)
            : base(text)
        {
            ListView = listView;
            NlmClick += clickEvents;
            this.Click += new EventHandler(onClick);
        }

        private void onClick(Object o, EventArgs e)
        {
            if (NlmClick != null)
                NlmClick(this, new ClickEventArgs(ListView));
        }
    }
}
