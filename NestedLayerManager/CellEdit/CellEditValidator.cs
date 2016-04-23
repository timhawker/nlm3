using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Autodesk.Max;
using NestedLayerManager.SubControls;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxAnims;

namespace NestedLayerManager.CellEdit
{
    public class CellEditValidator
    {
        private NlmTreeListView ListView;

        public CellEditValidator(NlmTreeListView listView)
        {
            ListView = listView;
            ListView.CellEditValidating += new CellEditEventHandler(ValidateCellEdit);
        }

        private void ValidateCellEdit(Object o, CellEditEventArgs e)
        {
            String oldValue = e.Value as String;
            String newValue = e.NewValue as String;

            if (oldValue != newValue)
            {
                IILayer layer = MaxLayers.GetLayer(newValue);

                if (layer != null)
                {
                    e.Cancel = true;
                    MessageBox.Show("A Layer with that name already Exists.");
                }
            }
        }

    }

}
