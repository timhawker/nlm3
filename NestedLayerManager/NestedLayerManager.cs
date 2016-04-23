using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Max;
using MaxCustomControls;
using NestedLayerManager.SubControls;
using NestedLayerManager.Events;
using NestedLayerManager.Filters;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.IO;
using NestedLayerManager.MaxInteractivity;
using NestedLayerManager.MaxInteractivity.MaxAnims;

// The following variables are defined for easy searching:
// OLVBUGFIX    -   Any bug fixes to the OLV.
// DEBUG        -   Operations for debug mode only.
// Max2013      -   Max 2013 specific code.
// Max2014      -   Max 2014 specific code.
// Max2015      -   Max 2015 specific code.
// TODO:        -   Things to do / look into.

namespace NestedLayerManager
{

    public class NestedLayerManager : TableLayoutPanel
    {
        NlmSearchBar SearchBar;
        NlmTreeListView ListView;

        NlmButtonPanelLeft ButtonPanelLeft;
        NlmButtonPanelRight ButtonPanelRight;
        NlmButtonPanelSide ButtonPanelSide;

        public NestedLayerManager()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            ListView = new NlmTreeListView();
            SearchBar = new NlmSearchBar(ListView);

            ButtonPanelLeft = new NlmButtonPanelLeft(ListView);
            ButtonPanelRight = new NlmButtonPanelRight(ListView);
            ButtonPanelSide = new NlmButtonPanelSide(ListView);

            MaxLook.ApplyLook(this);

            ColumnCount = 3;
            RowCount = 3;
            Padding = new Padding(3);
            Dock = DockStyle.Fill;

            Controls.Add(ButtonPanelLeft, 1, 0);
            SetColumnSpan(ButtonPanelLeft, 1);

            Controls.Add(ButtonPanelRight, 2, 0);

            Controls.Add(SearchBar, 1, 1);
            SetColumnSpan(SearchBar, 2);

            Controls.Add(ButtonPanelSide, 0, 2);

            Controls.Add(ListView, 1, 2);
            SetColumnSpan(ListView, 2);

            RowStyles.Add(new RowStyle(SizeType.Absolute, ButtonPanelLeft.Controls[0].Height + 2));
            RowStyles.Add(new RowStyle(SizeType.Absolute, SearchBar.Height + 2));
            RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, ButtonPanelSide.Controls[0].Width + 2));
            ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, ButtonPanelLeft.Controls.Count * ButtonPanelLeft.Controls[0].Width));
            ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, ButtonPanelRight.Controls.Count * ButtonPanelRight.Controls[0].Width));

            ListView.NodeControl.Create.BuildSceneTree();

            stopwatch.Stop();
            string listenerMessage = "Loaded in " + stopwatch.ElapsedMilliseconds + " milliseconds.";
            MaxListener.PrintToListener(listenerMessage);
        }

        protected override void Dispose(bool disposing)
        {
            MaxUI.RefreshButtonStates();
            base.Dispose(disposing);
        }
    }
}
