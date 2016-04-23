using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Autodesk.Max;
using NestedLayerManager.SubControls;
using NestedLayerManager.Columns;
using NestedLayerManager.Filters;
using NestedLayerManager.Renderers;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxEvents;
using NestedLayerManager.Maps;
using NestedLayerManager.Imaging;
using NestedLayerManager.CellEdit;
using NestedLayerManager.NodeControl;
using NestedLayerManager.MouseClick;
using NestedLayerManager.IO;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.MaxInteractivity.MaxAnims;

namespace NestedLayerManager.SubControls
{
    public class NlmTreeListView : TreeListView
    {
        #region Properties

        // Store any disabled handles here, so they can be enabled again when disabled finishes.
        public List<UIntPtr> DisabledHandles;

        // Store smart folder mode boolean.
        public Boolean SmartFolderMode;

        // Store the Column header renderer here, to provide a nice looking column header
        public ColumnHeaderRenderer ColumnHeaderRenderer;

        // Store the columns in a custom column class.
        public NlmColumnCollection NlmColumns;

        // Store the class that handles a cell edit validation.
        public CellEditValidator CellEditValidator;

        // Store the node controller class here, controlling all aspects of a nodes' life
        public NodeController NodeControl;

        // Store the left click handler class here.
        public MouseLeftClick MouseLeftClick;

        //Store access to the buttons within NLM here.
        public NlmButtonPanelLeft ButtonPanelLeft;
        public NlmButtonPanelRight ButtonPanelRight;
        public NlmButtonPanelSide ButtonPanelSide;

        #endregion

        #region Constructor

        public NlmTreeListView()
        {
            DisabledHandles = new List<UIntPtr>();

            Margin = new Padding(0);
            Dock = DockStyle.Fill;

            // Apply look from 3ds Max colour scheme and set ImageList.
            MaxLook.ApplyLook(this);
            NodeClassImageList.Apply(this);

            // Set properties.
            AllowDrop = true;
            IsSimpleDragSource = true;
            IsSimpleDropSink = true;
            FullRowSelect = true;
            UseAlternatingBackColors = true;
            RowHeight = 20;
            UseCustomSelectionColors = true;
            HideSelection = false;
            CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
            UseFiltering = true;
            BorderStyle = BorderStyle.FixedSingle;
            View = View.Details;
            OwnerDrawnHeader = true;
            RevealAfterExpand = false; // After expanding, the expand node moves to the top of the listview. It's kinda jarring so turn it off.
            CanUseApplicationIdle = false; // 3ds Max appears to completley suppress application idle events.
            DisabledItemStyle = new DisabledNodeStyle(this);
            SelectColumnsOnRightClick = true;
            SelectColumnsOnRightClickBehaviour = ColumnSelectBehaviour.InlineMenu;
            ShowCommandMenuOnRightClick = false;
            ShowFilterMenuOnRightClick = false;

            // Instance feature expansion classes.
            TreeColumnRenderer = new NlmTreeColumnRenderer();
            ColumnHeaderRenderer = new ColumnHeaderRenderer(this);
            NlmColumns = new NlmColumnCollection(this);
            CellEditValidator = new CellEditValidator(this);
            MouseLeftClick = new MouseLeftClick(this);
            ModelFilter = new NlmTreeNodeFilterEngine(this);
            NodeControl = new NodeController(this);
            ContextMenu = new NlmContextMenu(this);

            // Load saved state if it exists
            MaxIO.LoadNlmData(this);

            this.CellEditStarting += new CellEditEventHandler(onBeforeLabelEdit);
        }

        #endregion

        #region Events

        // If the cell being edited is layer 0, cancel the cell edit.
        private void onBeforeLabelEdit(Object sender, CellEditEventArgs e)
        {
            if (e.RowObject is LayerTreeNode)
            {
                IILayer layer = MaxLayers.GetLayer(0);
                UIntPtr handle = MaxAnimatable.GetHandleByAnim(layer);

                if ((e.RowObject as LayerTreeNode).Handle == handle)
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion

        #region Object Appending

        // AddObject appends any new treenodes to the UI.
        // The method included with OLV appends to root only.
        // This has been written to append to the root or any parent.
        public void AddObject(BaseTreeNode treeNode, BaseTreeNode treeNodeParent)
        {
            if (IsCellEditing)
            {
                CancelCellEdit();
            }
            if (treeNodeParent == null)
            {
                AddObject(treeNode);
            }
            else
            {
                treeNode.Parent = treeNodeParent;
                treeNodeParent.Children.Add(treeNode);
                RefreshObject(treeNodeParent);
            }
        }

        #endregion

        #region Overrides

        // Override mouse and keyboard entry behaviour.
        protected override void WndProc(ref Message m)
        {
            bool handled = false;

            // WM_LBUTTONDOWN
            // Handle left mouse button down without changing selection, imortant for multi selected column aspect changing.
            // Using a MouseDown EventHandler unfortunately changes seletion.
            if (m.Msg == 0x0201)
            {
                int x = m.LParam.ToInt32() & 0xFFFF;
                int y = (m.LParam.ToInt32() >> 16) & 0xFFFF;
                OlvListViewHitTestInfo hti = this.OlvHitTest(x, y);
                handled = MouseLeftClick.Click(hti);
                //TODO: this method is being suppressed by override. may need it!
                //this.PossibleFinishCellEditing();
                //look at base.WndProc to see.
            }

            // WM_CONTEXTMENU
            // Temporarily disable all context menu creation until it is properly implemented.
            if (m.Msg == 0x7B)
            {
                //handled = true;
            }

            if (handled)
            {
                return;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        // Control.ProcessDialogKey Method is completely supressed in 3ds Max.
        // For this reason when editing a cell an override is requried to add an EventHandler.
        // This EventHandler then manually calls the ProcessDialogKey Method.
        // We also need to disable accellerators in 3ds Max.
        protected override Control GetCellEditor(OLVListItem item, int subItemIndex)
        {
            Control editor = base.GetCellEditor(item, subItemIndex);
            editor.KeyDown += new KeyEventHandler(CellEditKeyDown);
            editor.LostFocus += new EventHandler(CellEditLostFocus);
            GlobalInterface.Instance.DisableAccelerators();
            return editor;
        }

        // Manually fire ProcessDialogKey Method on a KeyDown event.
        private void CellEditKeyDown(Object o, KeyEventArgs e)
        {
            this.ProcessDialogKey(e.KeyData);
        }

        // Manually fire CellEditor lost focus
        private void CellEditLostFocus(Object o, EventArgs e)
        {
            GlobalInterface.Instance.EnableAccelerators();
            this.CancelCellEdit();
        }

        // Dispose of all classes that require disposing.
        protected override void Dispose(bool disposing)
        {
            MaxIO.SaveData(this, NodeControl);
            NlmColumns.Dispose();
            NodeControl.Dispose();
            //TODO:
            // Look through all properties that need disposing and dispose them.
            base.Dispose(disposing);
        }

        #endregion
    }
}