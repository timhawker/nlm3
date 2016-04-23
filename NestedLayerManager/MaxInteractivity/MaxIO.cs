using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Autodesk.Max;
using NestedLayerManager.IO;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.IO.Data;
using NestedLayerManager.NodeControl;
using NestedLayerManager.Maps;

namespace NestedLayerManager.MaxInteractivity
{

    // So, how do we save the data?
    //
    // We could save all data on the RootNode, but this will not be accessable on 
    // a file > merge. If only we could access the rootnode of the file being merged.
    //
    // We could save all data onto each layer, but where do we save the folder info?
    // Saving it also on the layer would make sense, but this would cause duplication, 
    // and empty folders would have to be handled differently. Perhaps stored on 
    // the RootNode.
    //
    // We could save layer data on the layer, and folder data on the RootNode.
    // But this would not allow folders to be accessable during a file > merge.
    //
    // We could create our own Folder node based on an Animatable, but this would 
    // mean that NLM has to be installed to work in the max file, and would cause
    // a missing plugin warning if not. Something I am really trying to avoid.
    // It also seems really complex to get a node to Merge automatically.
    //
    // We could store all data on a dummy node, with no transform or box in the
    // viewport, much like a Maya node approach. But it seems kind of amateur to 
    // do that.
    //
    // So, let's do this:
    // Each layer has it's own data appended as a data chunk.
    // Each layer also has every parent folder appended to a data chunk.
    // All folders are also stored on the RootNode. Only empty folders really need to be
    // saved, but it makes it far easier to save all folder data here for loading.
    // By doing this, all necessary folders are available when merging any layer.
    // All folders are available when opening the file, but empty are not when merging.
    // 

    public static class MaxIO
    {
        #region Saving

        public static void SaveData(NlmTreeListView listView, NodeController nodeControl)
        {
            // Create folder and layer treeNode arrays.
            IEnumerable<FolderTreeNode> folderNodes = nodeControl.Query.FolderNodes;
            IEnumerable<LayerTreeNode> layerNodes = nodeControl.Query.LayerNodes;

            // Save layer and folder data.
            SaveFolderData(folderNodes, listView);
            SaveLayerData(layerNodes, listView, nodeControl.HandleMap);

            // Save TreeListView data
            SaveNlmData(listView);
        }

        public static void SaveFolderData(IEnumerable<FolderTreeNode> treeNodes, NlmTreeListView owner)
        {
            List<FolderData> folderData = new List<FolderData>();
            foreach (FolderTreeNode treeNode in treeNodes)
            {
                folderData.Add(new FolderData(treeNode, owner));
            }
            IAnimatable rootNode = GlobalInterface.Instance.COREInterface.RootNode as IAnimatable;
            SetAppData(rootNode, DataAddress.FolderData, folderData);
        }

        public static void SaveLayerData(IEnumerable<LayerTreeNode> treeNodes, NlmTreeListView owner, HandleMap handleMap)
        {
            foreach (LayerTreeNode treeNode in treeNodes)
            {
                // Saving layer data is comprised of two sections.
                IAnimatable layerAnim = MaxAnimatable.GetAnimByHandle(treeNode.Handle);
                if (layerAnim != null)
                {
                    // 1. Saving the layer data itself
                    LayerData layerData = new LayerData(treeNode, owner, handleMap);
                    SetAppData(layerAnim, DataAddress.LayerData, layerData);

                    // 2. Saving all folder parents of the layer, in case it is merged and the data is needed.
                    SaveParentFolderData(treeNode, layerAnim, owner);
                }
            }
        }

        private static void SaveParentFolderData(LayerTreeNode layerTreeNode, IAnimatable layerAnim, NlmTreeListView owner)
        {
            if (layerTreeNode.Parent != null)
            {
                List<FolderData> folderData = new List<FolderData>();
                FolderTreeNode parent = layerTreeNode.Parent as FolderTreeNode;
                while (parent != null)
                {
                    folderData.Add(new FolderData(parent, owner));
                    parent = parent.Parent as FolderTreeNode;
                }
                SetAppData(layerAnim, DataAddress.FolderData, folderData);
            }
        }

        public static void SaveNlmData(NlmTreeListView listView)
        {
            NlmData nlmData = new NlmData(listView);
            SetAppData(MaxNodes.RootNode, DataAddress.NlmData, nlmData);
        }

        #endregion

        #region Loading

        public static LayerData LoadLayerData(IILayer layer)
        {
            return GetAppData(layer as IAnimatable, DataAddress.LayerData) as LayerData;
        }

        public static void LoadNlmData(NlmTreeListView listView)
        {
            NlmData nlmData = GetAppData(MaxNodes.RootNode, DataAddress.NlmData) as NlmData;
            if (nlmData != null)
                 NlmDataIO.RestoreNlmData(listView, nlmData);
        }

        public static List<FolderData> LoadFolderRootNodeData()
        {
            IAnimatable rootNode = GlobalInterface.Instance.COREInterface.RootNode as IAnimatable;
            List<FolderData> folderDataNodes = GetAppData(rootNode, DataAddress.FolderData) as List<FolderData>;
            return folderDataNodes;
        }

        public static List<FolderData> LoadParentFolderData(IAnimatable layerAnim, NlmTreeListView owner)
        {
            List<FolderData> folderData = GetAppData(layerAnim, DataAddress.FolderData) as List<FolderData>;
            return folderData;
        }

        #endregion

        #region AppData

        private static void SetAppData(IAnimatable anim, UInt32 address, Object obj)
        {
            RemoveAppData(anim, address);
            BinaryFormatter binForm = new BinaryFormatter();
            using (MemoryStream memStream = new MemoryStream())
            {
                binForm.Serialize(memStream, obj);
                anim.AddAppDataChunk(ClassID, SuperClassID, address, memStream.ToArray());
            }
        }

        private static void RemoveAppData(IAnimatable anim, UInt32 address)
        {
            Boolean result = anim.RemoveAppDataChunk(ClassID, SuperClassID, address);
        }

        private static Object GetAppData(IAnimatable anim, UInt32 address)
        {
            IAppDataChunk chunk = anim.GetAppDataChunk(ClassID, SuperClassID, address);

            if (chunk == null)
                return null;

            BinaryFormatter binForm = new BinaryFormatter();
            using (MemoryStream memStream = new MemoryStream())
            {
                memStream.Write(chunk.Data, 0, chunk.Data.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                return binForm.Deserialize(memStream);
            }
        }

        #endregion

        #region Static ID Methods / Properties

        public static SClass_ID SuperClassID
        {
            get
            {
                return SClass_ID.Gup;
            }
        }

        public static IClass_ID ClassID
        {
            get
            {
                uint ClassIDA = 0x787a44fa;
                uint ClassIDB = 0x6c451cb1;
                return GlobalInterface.Instance.Class_ID.Create(ClassIDA, ClassIDB);
            }
        }

        public static class DataAddress
        {
            public const UInt32 LayerData = 1;
            public const UInt32 FolderData = 2;
            public const UInt32 NlmData = 3;
        }

        #endregion
    }
}
