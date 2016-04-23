using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BrightIdeasSoftware;
using System.Windows.Forms;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity;

namespace NestedLayerManager.Columns
{
    // The NlmColumns class contains each column in NLM.
    // The constructur will instance each column and add them to the owner control.
    public class NlmColumnCollection : IDisposable
    {
        public NameColumn NameColumn;
        public CurrentColumn CurrentColumn;
        public VisibleColumn VisibleColumn;
        public FreezeColumn FreezeColumn;
        public RenderColumn RenderColumn;
        public ColorColumn ColorColumn;
        public BoxColumn BoxColumn;

        public NlmColumnCollection(NlmTreeListView listView)
        {
            // Instance columns
            NameColumn = new NameColumn(listView);
            CurrentColumn = new CurrentColumn(listView);
            VisibleColumn = new VisibleColumn(listView);
            FreezeColumn = new FreezeColumn(listView);
            RenderColumn = new RenderColumn(listView);
            ColorColumn = new ColorColumn(listView);
            BoxColumn = new BoxColumn(listView);

            // Add columns to owner
            listView.AllColumns.Add(NameColumn);
            listView.AllColumns.Add(CurrentColumn);
            listView.AllColumns.Add(VisibleColumn);
            listView.AllColumns.Add(FreezeColumn);
            listView.AllColumns.Add(RenderColumn);
            listView.AllColumns.Add(ColorColumn);
            listView.AllColumns.Add(BoxColumn);

            listView.RebuildColumns();
        }

        public void Dispose()
        {
            NameColumn.Dispose();
            CurrentColumn.Dispose();
            VisibleColumn.Dispose();
            FreezeColumn.Dispose();
            RenderColumn.Dispose();
            ColorColumn.Dispose();
            BoxColumn.Dispose();
        }
    }
}
