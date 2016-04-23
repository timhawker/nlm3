using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Autodesk.Max;
using NestedLayerManager.Events;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.MaxInteractivity.MaxEvents.Handlers;
using NestedLayerManager.MaxInteractivity.MaxEvents.Args;

namespace NestedLayerManager.MaxInteractivity.MaxEvents.Handlers
{
    // This is the Event Handler Class for all notifications where event arguments are passed as generic objects.
    public class SystemNotificationObjectHandler : IDisposable
    {
        private GlobalDelegates.Delegate5 EventProc;
        public SystemNotificationCode NotificationCode;

        public event EventHandler<SystemNotificationObjectEventArgs> NotificationRaised;

        public SystemNotificationObjectHandler(SystemNotificationCode code)
        {
            EventProc = new GlobalDelegates.Delegate5(SystemNotification);
            NotificationCode = code;
            RegisterNotification();
        }

        public void RegisterNotification()
        {
            GlobalInterface.Instance.RegisterNotification(EventProc, null, NotificationCode);
#if DEBUG
            MaxListener.PrintToListener(NotificationCode.ToString() + " Notification Registered");
#endif
        }

        public void UnregisterNotification()
        {
            GlobalInterface.Instance.UnRegisterNotification(EventProc, null);
#if DEBUG
            MaxListener.PrintToListener(NotificationCode.ToString() + " Notification Unregistered");
#endif
        }

        private void SystemNotification(IntPtr Obj, IntPtr info)
        {
            INotifyInfo notifyInfo = GlobalInterface.Instance.NotifyInfo.Marshal(info);
            this.NotificationRaised(this, new SystemNotificationObjectEventArgs(notifyInfo.CallParam));
        }

        public void Dispose()
        {
            UnregisterNotification();
        }
    }
}
