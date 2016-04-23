using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;

namespace NestedLayerManager.MaxInteractivity.MaxAnims
{
    public static class MaxAnimatable
    {
        public static IAnimatable GetAnimByHandle(UIntPtr handle)
        {
            IAnimatable anim = GlobalInterface.Instance.Animatable.GetAnimByHandle(handle);
#if DEBUG
            if (anim == null)
                MaxListener.PrintToListener("ERROR: GetAnimByHandle anim is null");
#endif
            return anim;
        }
        
        public static UIntPtr GetHandleByAnim(IAnimatable anim)
        {
            return GlobalInterface.Instance.Animatable.GetHandleByAnim(anim);
        }
    }
}
