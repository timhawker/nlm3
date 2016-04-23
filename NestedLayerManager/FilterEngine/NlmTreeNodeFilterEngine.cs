using System;
using System.Collections.Generic;
using System.Linq;
using BrightIdeasSoftware;
using System.Windows.Forms;
using NestedLayerManager.Renderers;
using NestedLayerManager.SubControls;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.Interfaces;
using NestedLayerManager.Events.CustomArgs;

namespace NestedLayerManager.Filters
{
    public enum TreeNodeFilter { Bone, Camera , Helper, Light, Object, Shape, SpaceWarp, LayerHidden, LayerFrozen }

    // Main Filter class that is added to NlmTreeListView.
    public class NlmTreeNodeFilterEngine : CompositeAllFilter
    {
        private NlmTreeListView ListView;
        private TextMatchFilter StringFilter;

        #region Constructors

        public NlmTreeNodeFilterEngine(NlmTreeListView tlv)
            : base(new List<IModelFilter>())
        {
            ListView = tlv;
            this.Filters = new List<IModelFilter>();
        }

        #endregion

        #region Node Property Filtering

        public void AddFilter(TreeNodeFilter filter)
        {
            switch (filter)
            {
                case (TreeNodeFilter.Bone) :
                    {
                        Filters.Add(new NodeClassFilter(ObjectTreeNode.ObjectClass.Bone, filter));
                        break;
                    }
                case (TreeNodeFilter.Camera) :
                    {
                        Filters.Add(new NodeClassFilter(ObjectTreeNode.ObjectClass.Camera, filter));
                        break;
                    }
                case (TreeNodeFilter.Helper):
                    {
                        Filters.Add(new NodeClassFilter(ObjectTreeNode.ObjectClass.Helper, filter));
                        break;
                    }
                case (TreeNodeFilter.Light):
                    {
                        Filters.Add(new NodeClassFilter(ObjectTreeNode.ObjectClass.Light, filter));
                        break;
                    }
                case (TreeNodeFilter.Object):
                    {
                        Filters.Add(new NodeClassFilter(ObjectTreeNode.ObjectClass.Object, filter));
                        break;
                    }
                case (TreeNodeFilter.Shape):
                    {
                        Filters.Add(new NodeClassFilter(ObjectTreeNode.ObjectClass.Shape, filter));
                        break;
                    }
                case (TreeNodeFilter.SpaceWarp):
                    {
                        Filters.Add(new NodeClassFilter(ObjectTreeNode.ObjectClass.SpaceWarp, filter));
                        break;
                    }
                case (TreeNodeFilter.LayerHidden):
                    {
                        Filters.Add(new LayerHiddenFilter(TreeNodeFilter.LayerHidden));
                        break;
                    }
                case (TreeNodeFilter.LayerFrozen):
                    {
                        Filters.Add(new LayerFrozenFilter(TreeNodeFilter.LayerFrozen));
                        break;
                    }
            }
        }

        public void RemoveFilter(TreeNodeFilter filter)
        {
             for (int i = Filters.Count; i-- > 0; )
             {
                 BaseTreeNodeFilter treeNodeFilter = Filters[i] as BaseTreeNodeFilter;
                 if (treeNodeFilter.TreeNodeFilter == filter)
                     Filters.Remove(treeNodeFilter);
             }
        }

        public Boolean IsFilterActive(TreeNodeFilter filter)
        {
            for (int i = Filters.Count; i-- > 0; )
            {
                BaseTreeNodeFilter treeNodeFilter = Filters[i] as BaseTreeNodeFilter;
                if (treeNodeFilter.TreeNodeFilter == filter)
                    return true;
            }
            return false;
        }

        #endregion

        #region String Filtering

        // Search by text, expanding any parent nodes to reveal child match.
        internal void AddStringFilter(String text)
        {
            // If string filter is not null, try and remove it from active filter list
            if (StringFilter != null)
            {
                if (this.Filters.Contains(StringFilter)) this.Filters.Remove(StringFilter);
                StringFilter = null;
            }

            // Only search if string is not empty, and ensure all matched children are showing by expanding parents.
            if (!String.IsNullOrEmpty(text))
            {
                StringFilter = TextMatchFilter.Contains(this.ListView, text);
                StringFilter.Columns = new OLVColumn[] { ListView.NlmColumns.NameColumn };
                ExpandParentsOfMatchedChildren(text);
                this.Filters.Add(StringFilter);
            }

            // Set the filter to correspone to the text being searched. If text is "", nothing is filtered.
            HighlightTextRenderer highLightingRenderer = ListView.NlmColumns.NameColumn.Renderer as HighlightTextRenderer;
            if (highLightingRenderer != null)
                highLightingRenderer.Filter = StringFilter;
        }

        // Filtering does not take into consideration nodes that are not currently visible. 
        // This ensures that all nodes are visible that match the search string.        
        private void ExpandParentsOfMatchedChildren(String text)
        {
            HashSet<BaseTreeNode> NodesToExpand = new HashSet<BaseTreeNode>();
            foreach (BaseTreeNode treeNode in ListView.Roots)
            {
                SearchChildrenAndExpandByName(treeNode, text, NodesToExpand);
            }

            if (NodesToExpand.Count > 0)
            {
                foreach (BaseTreeNode nodeToExpand in NodesToExpand.ToList<BaseTreeNode>())
                {
                    if (!ListView.IsExpanded(nodeToExpand))
                        ListView.Expand(nodeToExpand);
                }
            }
        }

        // We have to recursively loop through each child.
        // If we find a match, obtain a list of all parents, and add to the hash check backwards.
        // Adding backwards is important because you have to expand the top node, working your way down.
        // Expanding a parent node that is not visible does not work, as it is not yet added to the treelistview.
        // A hashtable is used to speed up the .Contains() method, which is extremely slow in a list.
        private void SearchChildrenAndExpandByName(BaseTreeNode treeNode, String text, HashSet<BaseTreeNode> nodesToExpand)
        {
            foreach (BaseTreeNode childNode in treeNode.Children)
            {
                if (((String)ListView.NlmColumns.NameColumn.GetValue(childNode)).IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    BaseTreeNode whileNode = childNode;
                    List<BaseTreeNode> parents = new List<BaseTreeNode>();
                    while (whileNode.Parent != null)
                    {
                        if (nodesToExpand.Contains(whileNode.Parent)) break;
                        else parents.Add(whileNode.Parent);
                        whileNode = whileNode.Parent;
                    }
                    for (int i = parents.Count; i-- > 0; ) nodesToExpand.Add(parents[i]);
                }
                if (childNode.Children.Count > 0)
                {
                    SearchChildrenAndExpandByName(childNode, text, nodesToExpand);
                }
            }
        }

        #endregion
    }
}
