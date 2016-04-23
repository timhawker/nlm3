using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;

namespace NestedLayerManager.MaxInteractivity
{
    public static class MaxUI
    {
        public static void RedrawViewportsNow()
        {
            // 0x0000 DEFAULT_RENDER and REDRAW_INTERACTIVE
 	        // 0x0001 VP_DONT_RENDER   
            // 0x0002 VP_DONT_SIMPLIFY and REDRAW_NORMAL   
            // 0x0004 VP_START_SEQUENCE   
            // 0x0008 VP_END_SEQUENCE and REDRAW_END   
            // 0x0010 VP_SECOND_PASS   

            // We want to redraw normally, so we pick 0x0002

#if DEBUG
            MaxListener.PrintToListener("Redrawing Viewports");
#endif

            Int32 time = GlobalInterface.Instance.COREInterface14.Time;
            GlobalInterface.Instance.COREInterface14.RedrawViewportsNow(time, 0x0002);
        }

        public static void RefreshButtonStates()
        {
            GlobalInterface.Instance.CUIFrameMgr.SetMacroButtonStates(false);
        }
    }
}
