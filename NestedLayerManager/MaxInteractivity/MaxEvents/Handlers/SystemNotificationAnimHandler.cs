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
    // The 3ds Max Notification system alerts of an event for every item that changes.
    // This is not desired behaviour as it requires a lot of control updating/refreshing.
    // This class is used to wrap all notification events for a particular notification in one event.
    // The timer is reset if the notificaion is raised again, indicating the user is looping something.
    // This allows a callback to work on a list of changed objects, rather than one at a time.

    // NOTE:
    // This is the Event Handler Class for all notifications that return Ianimatable in CallParam.
    // The handles are obtained from each Ianimatable and stored for raising the notification.
    // This is a much safter method becuase GetAnimByHandle will simply return null if the Ianimatable
    // no longer exists. Passing the Ianimatable through to a slightly delayed event handling method
    // may result in a null pointer exception if the Ianimatable is deleted in between. This DOES 
    // happen, which is why handles are used.
    public class SystemNotificationAnimHandler : IDisposable
    {
        private DispatcherTimer Timer;
        private List<UIntPtr> Handles;
        private GlobalDelegates.Delegate5 EventProc;
        public SystemNotificationCode NotificationCode;

        public event EventHandler<SystemNotificationAnimEventArgs> NotificationRaised;

        public SystemNotificationAnimHandler(SystemNotificationCode code)
        {
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(10);
            Timer.Tick += new EventHandler(OnTimerTick);
            Handles = new List<UIntPtr>();
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
            Timer.Stop();
            INotifyInfo notifyInfo = GlobalInterface.Instance.NotifyInfo.Marshal(info);
            IAnimatable anim = notifyInfo.CallParam as IAnimatable;
            UIntPtr handle = MaxAnimatable.GetHandleByAnim(anim);
            if (handle != null)
            {
                Handles.Add(handle);
            }
            Timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            Timer.Stop();
            this.NotificationRaised(this, new SystemNotificationAnimEventArgs(Handles));
            Handles.Clear();
        }

        public void Dispose()
        {
            UnregisterNotification();
        }
    }
}
