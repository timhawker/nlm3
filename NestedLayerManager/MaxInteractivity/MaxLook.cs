using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedServices;
using Autodesk.Max;
using System.Drawing;
using System.Windows.Forms;
using NestedLayerManager.SubControls;

namespace NestedLayerManager.MaxInteractivity
{
    // This class acts as a wrapper around the CuiUpdater to easily theme NLM
    // Look at MaxStartUI file for more info on what numbers relate to.
    public static class MaxLook
    {
        #region Colour Getting

        public static Color GetHighlightColor()
        {
            using (CuiUpdater cuiUpdater = CuiUpdater.GetInstance())
            {
                return cuiUpdater.GetMaxColor(13);
            }
        }

        #endregion

        #region Look Setting

        public static void ApplyLook(NlmTreeListView listView)
        {
            using (CuiUpdater cuiUpdater = CuiUpdater.GetInstance())
            {
                listView.BackColor = cuiUpdater.GetMaxColor(7);
                listView.ForeColor = cuiUpdater.GetMaxColor(1);
                listView.AlternateRowBackColor = Color.FromArgb(listView.BackColor.R + 5, listView.BackColor.G + 5, listView.BackColor.B + 5);
                listView.HighlightBackgroundColor = cuiUpdater.GetMaxColor(13);
                listView.UnfocusedHighlightBackgroundColor = cuiUpdater.GetMaxColor(13);
                listView.HighlightForegroundColor = cuiUpdater.GetMaxColor(11);
                listView.UnfocusedHighlightForegroundColor = cuiUpdater.GetMaxColor(11);
            }
        }

        public static void ApplyLook(NlmSearchBar searchBar)
        {
            using (CuiUpdater cuiUpdater = CuiUpdater.GetInstance())
            {
                searchBar.BackColor = cuiUpdater.GetMaxColor(7);
                searchBar.ForeColor = cuiUpdater.GetMaxColor(1);
            }
        }

        public static void ApplyLook(NestedLayerManager nlm)
        {
            using (CuiUpdater cuiUpdater = CuiUpdater.GetInstance())
            {
                nlm.BackColor = cuiUpdater.GetMaxColor(0);
            }
        }

        #endregion
    }
}
