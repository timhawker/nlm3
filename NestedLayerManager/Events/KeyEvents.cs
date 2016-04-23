using System;
using System.Windows.Forms;
using System.Drawing;
using MaxCustomControls;
using Autodesk.Max;
using System.Collections;
using System.Collections.Generic;
using NestedLayerManager.Events;
using NestedLayerManager.Filters;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.IO;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxAnims;
using NestedLayerManager.Events.CustomArgs;

namespace NestedLayerManager.Events
{
    public static class KeyEvents
    {
        #region Search Bar Events

        public static void SearchBarTextChanged(Object sender, SearchBarEventArgs e)
        {
            try 
            { 
                e.ListView.BeginUpdate();

                // TODO: Why did I do this selection memory thing?
                IList selection = e.ListView.SelectedObjects;

                NlmTreeNodeFilterEngine nlmNodeFilter = e.ListView.ModelFilter as NlmTreeNodeFilterEngine;
                nlmNodeFilter.AddStringFilter(e.Text);
                e.ListView.ModelFilter = nlmNodeFilter;

                e.ListView.SelectObjects(selection);
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                e.ListView.EndUpdate();
            }
        }

        public static void SearchBarEnterPressed(Object sender, SearchBarEventArgs e)
        {
            // Using filtered objects will include parents which are not string matched.
            // It is necessary to remove them from the array first.

            NlmTreeListView listView = e.ListView;
            String searchString = e.Text;
            IList filteredNodes = new List<BaseTreeNode>();

            foreach (BaseTreeNode treeNode in listView.FilteredObjects)
            {
                String name = listView.NlmColumns.NameColumn.AspectEngine.GetAspect(treeNode) as String;
                if (name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    filteredNodes.Add(treeNode);
                }
            }

            listView.SelectObjects(filteredNodes);
        }

        #endregion
    }
}
