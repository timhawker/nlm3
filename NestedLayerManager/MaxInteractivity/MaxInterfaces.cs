using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;

namespace NestedLayerManager.MaxInteractivity
{
    // This singleton class is used to create instances of classes that are not instanced already.
    // Interfaces can be created by providing the interface ID.
    public class MaxInterfaces
    {
        # region Singleton Construction

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<MaxInterfaces> _instance = new Lazy<MaxInterfaces>(() => new MaxInterfaces());

        // accessor for instance
        public static MaxInterfaces Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        # endregion

        #region Properties

        //public IInterface_ID LayerPropertiesID;
        public IIFPLayerManager FPLayerManager;

        #endregion

        #region Constructor

        // Constructor is private in order to prevent direct instantiation.
        private MaxInterfaces()
        {
            //LayerPropertiesID = GlobalInterface.Instance.Interface_ID.Create();
            //LayerPropertiesID.PartA = (UInt32)BuiltInInterfaceIDA.LAYERPROPERTIES_INTERFACE;
            //LayerPropertiesID.PartB = (UInt32)BuiltInInterfaceIDB.LAYERPROPERTIES_INTERFACE;

            IInterface_ID IIFPLayerManagerID = GlobalInterface.Instance.Interface_ID.Create();
            IIFPLayerManagerID.PartA = (UInt32)BuiltInInterfaceIDA.LAYERMANAGER_INTERFACE;
            IIFPLayerManagerID.PartB = (UInt32)BuiltInInterfaceIDB.LAYERMANAGER_INTERFACE;
            FPLayerManager = (IIFPLayerManager)GlobalInterface.Instance.GetCOREInterface(IIFPLayerManagerID);
        }

        #endregion
    }
}
