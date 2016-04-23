using System;
using System.Collections.Generic;

namespace NestedLayerManager.MaxInteractivity.MaxEvents.Args
{
    public class SystemNotificationAnimEventArgs : EventArgs
    {
        public SystemNotificationAnimEventArgs(List<UIntPtr> handles)
        {
            Handles = handles;
        }
        public List<UIntPtr> Handles { get; set; }
    }
}
