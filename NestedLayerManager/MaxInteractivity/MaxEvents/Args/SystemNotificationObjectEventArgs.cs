using System;
using System.Collections.Generic;

namespace NestedLayerManager.MaxInteractivity.MaxEvents.Args
{
    public class SystemNotificationObjectEventArgs : EventArgs
    {
        public SystemNotificationObjectEventArgs(Object callParam)
        {
            CallParam = callParam;
        }
        public Object CallParam { get; set; }
    }
}
