using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NestedLayerManager.AspectEngines;

namespace NestedLayerManager.Interfaces
{
    public interface INLMColumn
    {
        IAspectEngine AspectEngine { get; set; }
    }
}
