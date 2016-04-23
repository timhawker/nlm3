using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NestedLayerManager.SubControls;
using NestedLayerManager.Maps;
using NestedLayerManager.NodeControl.Engines;
using NestedLayerManager.IO;
using NestedLayerManager.MaxInteractivity.MaxEvents;

namespace NestedLayerManager.NodeControl
{
    /// <summary>
    /// This class is used as a master class, containing sub classes that provide
    ///  extensive node controlling. The sub classes require access to other sub
    ///  classes, which is all provided by the constructor. 
    ///  Max events are stored as a property here as they are so interlinked.
    /// </summary>
    public class NodeController : IDisposable
    {
        public MaxEventEngine MaxEvents;
        public HandleMap HandleMap;
        public NodeCollapseExpandEngine CollapseExpand;
        public NodeCreateEngine Create;
        public NodeDeleteEngine Destroy;
        public NodeDragDropEngine DragDrop;
        public NodeParentEngine Parent;
        public NodeQueryEngine Query;

        public NodeController(NlmTreeListView listView)
        {
            MaxEvents = new MaxEventEngine(listView, this);
            HandleMap = new HandleMap();
            Query = new NodeQueryEngine(listView);
            Create = new NodeCreateEngine(listView, HandleMap);
            Destroy = new NodeDeleteEngine(listView, Query, HandleMap);
            CollapseExpand = new NodeCollapseExpandEngine(listView, Query);
            Parent = new NodeParentEngine(listView);
            DragDrop = new NodeDragDropEngine(listView, Parent, MaxEvents);
            
            
        }

        public void Dispose()
        {
            MaxEvents.Dispose();
        }
    }
}
