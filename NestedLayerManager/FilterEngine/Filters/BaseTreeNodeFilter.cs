using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;
using NestedLayerManager.Filters;

namespace NestedLayerManager.Interfaces
{
    // All filters have an enum property, which we can't add to an interface.
    // Instead, inherit from this.
    public abstract class BaseTreeNodeFilter : IModelFilter
    {
        public TreeNodeFilter TreeNodeFilter;

        public abstract bool Filter(object modelObject);
    }
}
