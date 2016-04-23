using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;
using NestedLayerManager.SubControls;
using NestedLayerManager.IO;
using NestedLayerManager.NodeControl;
using NestedLayerManager.MaxInteractivity.MaxEvents.Controllers;

namespace NestedLayerManager.MaxInteractivity.MaxEvents
{
    public class MaxEventEngine : IDisposable
    {
        public MaxNodeEvents NodeEvents;
        public MaxLayerEvents LayerEvents;
        public MaxSystemEvents SystemEvents;

        public MaxEventEngine(NlmTreeListView listView, NodeController nodeControl)
        { 
            NodeEvents = new MaxNodeEvents(listView, nodeControl);
            LayerEvents = new MaxLayerEvents(listView, nodeControl);
            SystemEvents = new MaxSystemEvents(listView, nodeControl);
        }

        public void Dispose()
        {
            SystemEvents.Dispose();
            LayerEvents.Dispose();
            NodeEvents.Dispose();
        }
    }
}
