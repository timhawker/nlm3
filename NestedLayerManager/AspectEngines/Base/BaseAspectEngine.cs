using System;
using BrightIdeasSoftware;
using System.Windows.Forms;
using NestedLayerManager.SubControls;
using NestedLayerManager.Interfaces;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.Nodes.Base;

namespace NestedLayerManager.AspectEngines.Base
{
    // This provides core functionalitiy for AspectEngine classes.
    // It doesn't exactly do much without being inherited.
    // It is used to provide common methods and properties for specific aspect getters.
    public class BaseAspectEngine : IAspectEngine
    {
        #region Properties

        public OLVColumn Column;
        public NlmTreeListView ListView;

        #endregion

        #region Constructors

        public BaseAspectEngine(NlmTreeListView listView, OLVColumn column)
        {
            ListView = listView;
            Column = column;
            InitialiseAspectDelegates();
        }

        #endregion

        //TODO: Should these not be abstract?
        #region Override Methods

        // Get and set TreeNode property associated with column.
        public virtual Object GetTreeNodeValue(BaseTreeNode treeNode) { return null; }
        public virtual void PutTreeNodeValue(BaseTreeNode treeNode, Object newValue) { }

        // Get and set MaxValue property associated with column.
        public virtual Object GetMaxValue(BaseTreeNode treeNode) { return null; }
        public virtual void PutMaxValue(BaseTreeNode treeNode, Object newValue) { }

        // Main Aspect getters and setters. 
        // Without inheriting the BaseAspectEngine and overriding these, there won't be much to render.
        public virtual Object GetAspect(Object RowObject) { return null; }
        public virtual void PutAspect(Object rowObject, Object newValue) { }

        #endregion

        #region Public Interface

        // Obtain folder check state for hierarchy folder mode.
        public Boolean? getFolderCheckState(BaseTreeNode treeNode)
        {
            if (treeNode.Children.Count < 1)
            {
                return true;
            }
            else
            {
                CheckState firstItem = Column.GetCheckState(treeNode.Children[0]);
                for (int i = 1; i < treeNode.Children.Count; i++)
                {
                    if (Column.GetCheckState(treeNode.Children[i]) != firstItem)
                    {
                        firstItem = CheckState.Indeterminate;
                        break;
                    }
                }
                switch (firstItem)
                {
                    case CheckState.Checked: return true;
                    case CheckState.Unchecked: return false;
                    default: return null;
                }
            }
        }

        public Boolean IsSubItemUnchecked(Object rowObject)
        {
            if (Column == null || rowObject == null || !Column.CheckBoxes)
                return true;
            return (Column.GetCheckState(rowObject) == CheckState.Unchecked);
        }

        public void ToggleCheckState(Object rowObject)
        {
            bool val = IsSubItemUnchecked(rowObject);
            Column.PutValue(rowObject, val);
        }

        public void ToggleCheckStates(Object rowObject)
        {
            bool val = IsSubItemUnchecked(rowObject);
            foreach (Object RowObject in ListView.SelectedObjects)
            {
                Column.PutValue(RowObject, val);
            }
        }

        #endregion

        #region Private Methods

        private void InitialiseAspectDelegates()
        {
            Column.AspectGetter = delegate(Object rowObject)
            {
                return GetAspect(rowObject);
            };
            Column.AspectPutter = delegate(Object rowObject, Object newValue)
            {
                PutAspect(rowObject, newValue);
            };
        }

        #endregion
    }
}
