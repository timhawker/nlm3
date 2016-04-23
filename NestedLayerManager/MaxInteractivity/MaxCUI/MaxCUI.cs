using System;
using System.Windows.Forms;
using UiViewModels.Actions;
using MaxCustomControls;
using NestedLayerManager.IO;
using Autodesk.Max;

// This class provides the integration into 3ds Max 'Customise User Interface' section.
// NLM can be loaded and unloaded from the UI via this.
namespace NestedLayerManager.MaxInteractivity.MaxCUI
{
    public class MaxCUI : CuiActionCommandAdapter, IDisposable
    {

        private NestedLayerManager NestedLayerManager;
        private MaxForm MaxForm;

        public override string Category
        {
            get
            {
                return MaxCUIProperties.Category;
            }
        }

        public override string InternalCategory
        {
            get 
            { 
                return MaxCUIProperties.Category; 
            }
        }

        public override string ActionText
        {
            get
            {
                return MaxCUIProperties.ActionText;
            }
        }

        public override string InternalActionText
        {
            get
            {
                return MaxCUIProperties.ActionText;
            }
        }

        public override string ButtonText
        {
            get
            {
                return MaxCUIProperties.ButtonText;
            }
        }

        public override void Execute(object parameter)
        {
            if (this.NestedLayerManager == null || this.NestedLayerManager != null && this.NestedLayerManager.IsDisposed)
            {
                this.NestedLayerManager = new NestedLayerManager();
                this.MaxForm = new MaxForm();
                this.MaxForm.Text = MaxCUIProperties.WindowTitle;
                XmlIO XmlIO = new XmlIO(MaxForm);
                this.MaxForm.Controls.Add(NestedLayerManager);
                this.MaxForm.ShowModeless();
            }
            else
            {
                this.MaxForm.Close();
                this.MaxForm = null;
                this.NestedLayerManager = null;
            }
        }

        public override bool IsChecked
        {
            get 
            {
                if (this.MaxForm != null)
                {
                    return this.MaxForm.Visible;
                }
                else 
                {
                    return false;
                }
            }
        }

        public void Dispose()
        {
            NestedLayerManager.Dispose();
            MaxForm.Dispose();
        }
    }
}
