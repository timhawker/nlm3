using System;
using System.Windows.Forms;
using ManagedServices;
using System.Windows.Threading;
using Autodesk.Max;
using NestedLayerManager.SubControls;
using NestedLayerManager.Events;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.Events.CustomArgs;

namespace NestedLayerManager
{
    public class NlmSearchBar : TextBox
    {
        private DispatcherTimer DispatchTimer;
        private NlmTreeListView ListView;

        public event EventHandler<SearchBarEventArgs> DelayedTextChanged;
        public event EventHandler<SearchBarEventArgs> EnterKeyDown;

        public NlmSearchBar(NlmTreeListView listView)
        {
            ListView = listView;
            MaxLook.ApplyLook(this);
            Margin = new Padding(0);
            Dock = DockStyle.Fill;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Height = 20;
            this.DispatchTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(400) };
            this.TextChanged += new EventHandler(OnTextChanged);
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            this.KeyPress += new KeyPressEventHandler(StopThatDing);
            this.DispatchTimer.Tick += new EventHandler(OnTimerTick);
            this.GotFocus += new EventHandler(onGotFocus);
            this.LostFocus += new EventHandler(onLostFocus);

            DelayedTextChanged += new EventHandler<SearchBarEventArgs>(KeyEvents.SearchBarTextChanged);
            EnterKeyDown += new EventHandler<SearchBarEventArgs>(KeyEvents.SearchBarEnterPressed);
        }

        private void onGotFocus(Object o, EventArgs e)
        {
            GlobalInterface.Instance.DisableAccelerators();
        }

        private void onLostFocus(Object o, EventArgs e)
        {
            GlobalInterface.Instance.EnableAccelerators();
        }

        // Searching hundreds of thousands of nodes can sometimes take a little time.
        // To avoid locking the UI while searching, a timer is used to delay the search.
        // Once the text has not changed for the desired interval, the DelayedTextChanged event is fired.
        private void OnTextChanged(Object sender, EventArgs e)
        {
            DispatchTimer.Stop();
            DispatchTimer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            DispatchTimer.Stop();
            if (this.DelayedTextChanged != null)
            {
                this.DelayedTextChanged(this, new SearchBarEventArgs(ListView, this.Text));
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.Text) && (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return))
            {
                if (DispatchTimer.IsEnabled)
                    DispatchTimer.Stop();
                this.DelayedTextChanged(this, new SearchBarEventArgs(ListView, this.Text));
                this.EnterKeyDown(this, new SearchBarEventArgs(ListView, this.Text));
            }
        }
        private void StopThatDing(Object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return || e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
            }
        }
    }
}
