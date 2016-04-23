using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;

namespace NestedLayerManager.MaxInteractivity
{
    public static class MaxListener
    {
        public static void PrintToListener(string message)
        {
            string messageToSend = "[NLM] " + message + "\n";
            GlobalInterface.Instance.TheListener.EditStream.Wputs(messageToSend);
            GlobalInterface.Instance.TheListener.EditStream.Flush();
        }
    }
}
