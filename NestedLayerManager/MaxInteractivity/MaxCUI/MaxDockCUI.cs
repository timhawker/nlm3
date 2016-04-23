using System;
using MaxCustomControls;
using UiViewModels.Actions;
using UiViewModels;
using Autodesk.Max;
using System.Reflection;

// This class provides the integration into 3ds Max 'Customise User Interface' section.
// NLM can be loaded and unloaded from the UI in a dockable ICUIFrame window.

// TODO: How can we create an ICUIFrame manually, and add NLM to this?
// IntPtr framePtr = GlobalInterface.Instance.CreateCUIFrameWindow(MaxForm.Handle, "title", 0, 0, 100, 100);
// GlobalInterface.Instance.CUIFrameMgr.FloatCUIWindow(framePtr, new System.Drawing.Rectangle(0, 0, 100, 100), 0);
// IICUIFrame frame = GlobalInterface.Instance.CUIFrameMgr.GetICUIFrame("title");

namespace NestedLayerManager.MaxInteractivity.MaxCUI
{
    // This does not currently work because you can't put a form within a form ;)
    // UI needs to be changed to a panel (name is crap, and also background will need to be set manually)
    // Maxform can handle undocked mode
    // This class can handle docked mode
    public class MaxDockCUI : CuiDockableContentAdapter, IDisposable
    {
        private NestedLayerManager NestedLayerManager;

        public override string ActionText
        {
            get
            {
                return MaxCUIProperties.ActionTextDockable;
            }
        }
        public override string Category
        {
            get
            {
                return MaxCUIProperties.Category;
            }
        }

        public override bool DestroyOnClose
        {
            get
            {
                // Unfortunately there does not seem to be a way of catching the window closing other than this.
                // When the ICUIWindow is disposed, this property is queried, so we use it to detect when to dispose.
                if (NestedLayerManager != null)
                {
                    if (!NestedLayerManager.IsDisposed)
                    {
                        NestedLayerManager.Dispose();
                    }
                }
                return true;
            }
            set
            {
                base.DestroyOnClose = value;
            }
        }

        public override Type ContentType
        {
            get
            {
                return typeof(NestedLayerManager);
            }
        }

        public override String WindowTitle
        {
            get
            {
                return MaxCUIProperties.WindowTitle;
            }
        }

        public override string ButtonText
        {
            get
            {
                return MaxCUIProperties.ButtonTextDockable;
            }
        }

        public override object CreateDockableContent()
        {
            NestedLayerManager = new NestedLayerManager();
            return NestedLayerManager;
        }

        public override DockStates.Dock DockingModes
        {
            get
            {
                return DockStates.Dock.Left | DockStates.Dock.Right | DockStates.Dock.Floating | DockStates.Dock.Viewport;
            }
        }

        public void Dispose()
        {
            NestedLayerManager.Dispose();
        }
    }
}
