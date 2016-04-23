using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NestedLayerManager.Events;
using NestedLayerManager.Events.CustomArgs;

namespace NestedLayerManager.SubControls
{
    public class NlmContextMenu : ContextMenu
    {
        NlmTreeListView ListView;

        NlmMenuItem CreateLayer;
        NlmMenuItem CreateLayerAddSelection;
        
        NlmMenuItem CreateFolder;
        NlmMenuItem CreateFolderAddSelection;

        NlmMenuItem MergeSelectedLayers;
        NlmMenuItem MergeSelectedFolders;

        NlmMenuItem DuplicateSelectionCopyCopy;
        NlmMenuItem DuplicateSelectionCopyInstance;
        NlmMenuItem DuplicateSelectionInstance;

        NlmMenuItem DeleteSelection;
        NlmMenuItem DeleteObjectsInSelection;
        NlmMenuItem DeleteEmptyLayers;
        NlmMenuItem DeleteEmptyFolders;

        NlmMenuItem IsolateSelection;

        public NlmContextMenu(NlmTreeListView listView)
        {
            ListView = listView;

            CreateLayer = new NlmMenuItem(
                ListView, 
                "Create Layer", 
                ClickEvents.onCreateLayer
                );
            CreateLayerAddSelection = new NlmMenuItem(
                ListView, 
                "Create Layer (Add Selection)", 
                ClickEvents.onCreateLayerAddSelection
                );

            CreateFolder = new NlmMenuItem(
                ListView, 
                "Create Folder", 
                ClickEvents.onCreateFolder
                );
            CreateFolderAddSelection = new NlmMenuItem(
                listView,
                "Create Folder (Add Selection)",
                ClickEvents.onCreateFolderAddSelection
                );

            MergeSelectedLayers = new NlmMenuItem(
                ListView,
                "Merge Selected Layers",
                ClickEvents.onMergeSelectedLayers
                );
            MergeSelectedFolders = new NlmMenuItem(
                ListView,
                "Merge Selected Folders",
                ClickEvents.onMergeSelectedFolders
                );

            DuplicateSelectionCopyCopy = new NlmMenuItem(
               ListView,
               "Duplicate Selection (Copy Layers, Copy Objects)",
               ClickEvents.onDuplicateSelectionCopyCopy
               );
            DuplicateSelectionCopyInstance = new NlmMenuItem(
               ListView,
               "Duplicate Selection (Copy Layers, Instance Objects)",
               ClickEvents.onDuplicateSelectionCopyInstance
               );
            DuplicateSelectionInstance = new NlmMenuItem(
               ListView,
               "Duplicate Selection (Instance Layers)",
               ClickEvents.onDuplicateSelectionInstance
               );

            DeleteSelection = new NlmMenuItem(
               ListView,
               "Delete Selection",
               ClickEvents.onDeleteSelection
               );
            DeleteObjectsInSelection = new NlmMenuItem(
               ListView,
               "Delete Objects in Selection",
               ClickEvents.onDeleteObjectsInSelection
               );
            DeleteEmptyLayers = new NlmMenuItem(
               ListView,
               "Delete Empty Layers",
               ClickEvents.onDeleteEmptyLayers
               );
            DeleteEmptyFolders = new NlmMenuItem(
               ListView,
               "Delete Empty Folders",
               ClickEvents.onDeleteEmptyFolders
               );

            IsolateSelection = new NlmMenuItem(
               ListView,
               "Isolate Selection",
               ClickEvents.onIsolateSelection
               );

            //Tempoarily disable unimplemented menuitems.
            MergeSelectedLayers.Enabled = false;
            MergeSelectedFolders.Enabled = false;
            DuplicateSelectionCopyCopy.Enabled = false;
            DuplicateSelectionCopyInstance.Enabled = false;
            DuplicateSelectionInstance.Enabled = false;
            DeleteObjectsInSelection.Enabled = false;
            DeleteEmptyLayers.Enabled = false;
            DeleteEmptyFolders.Enabled = false;

            // Add the MenuItem objects to the ContextMenu control.
            MenuItems.Add(CreateLayer);
            MenuItems.Add(CreateLayerAddSelection);
            MenuItems.Add(new MenuItem("-"));
            MenuItems.Add(CreateFolder);
            MenuItems.Add(CreateFolderAddSelection);
            MenuItems.Add(new MenuItem("-"));
            MenuItems.Add(MergeSelectedLayers);
            MenuItems.Add(MergeSelectedFolders);
            MenuItems.Add(new MenuItem("-"));
            MenuItems.Add(DuplicateSelectionCopyCopy);
            MenuItems.Add(DuplicateSelectionCopyInstance);
            MenuItems.Add(DuplicateSelectionInstance);
            MenuItems.Add(new MenuItem("-"));
            MenuItems.Add(DeleteSelection);
            MenuItems.Add(DeleteObjectsInSelection);
            MenuItems.Add(DeleteEmptyLayers);
            MenuItems.Add(DeleteEmptyFolders);
            MenuItems.Add(new MenuItem("-"));
            MenuItems.Add(IsolateSelection);
        }
    }
}
