using System;
using System.Drawing;
using BrightIdeasSoftware;
using NestedLayerManager.SubControls;
using NestedLayerManager.Columns;
using NestedLayerManager.Interfaces;
using NestedLayerManager.Nodes;
using NestedLayerManager.Nodes.Base;
using NestedLayerManager.Imaging;

namespace NestedLayerManager.Renderers
{
    class MappedActiveImageRenderer : BaseRenderer
    {
        // Store images here.
        private Image TrueActiveImage;
        private Image TrueInactiveImage;
        private Image FalseActiveImage;
        private Image FalseInactiveImage;
        private Image NullActiveImage;
        private Image NullInactiveImage;

        // Constructors.
        public MappedActiveImageRenderer(Bitmap trueImage, Bitmap falseImage, Bitmap nullImage)
        {
            TrueActiveImage = trueImage;
            TrueInactiveImage = BitmapTools.ChangeOpacity(trueImage, 0.3F);
            FalseActiveImage = falseImage;
            FalseInactiveImage = BitmapTools.ChangeOpacity(falseImage, 0.3F);
            NullActiveImage = nullImage;
            NullInactiveImage = BitmapTools.ChangeOpacity(nullImage, 0.3F);
        }

        // Methods to find correct image, based on a Boolean? provided.
        public Image GetActiveImage(bool? aspectValue)
        {
            switch (aspectValue)
            {
                case true:
                    return TrueActiveImage;
                case false:
                    return FalseActiveImage;
                default:
                    return NullActiveImage;
            }
        }

        public Image GetInactiveImage(bool? aspectValue)
        {
            switch (aspectValue)
            {
                case true:
                    return TrueInactiveImage;
                case false:
                    return FalseInactiveImage;
                default:
                    return NullInactiveImage;
            }
        }

        // The main work is done here.
        // Find out if smart folder mode is being used
        // If false, return active image based on aspectValue.
        // If true, calculate correct icon to show depending on aspectValue and all parent values.
        // If a parent is true, the icon will be active and based on aspectValue.
        // If a parent is false, the icon will be inactive and based on aspectValue.
        public override void Render(Graphics g, Rectangle r)
        {
            this.DrawBackground(g, r);
            r = this.ApplyCellPadding(r);

            Image image = null;
            bool? aspectValue = this.Aspect as bool?;

            // Is the listItem enabled? If so, we need to calculate the correct active/inactive icon.
            if (ListItem.Enabled)
            {
                NlmTreeListView ListView = this.ListView as NlmTreeListView;
                if (ListView.SmartFolderMode)
                {
                    BaseTreeNode baseTreeNode = this.RowObject as BaseTreeNode;
                    bool parentValue = true;
                    while (baseTreeNode.Parent != null)
                    {
                        INLMColumn nlmColumn = this.Column as INLMColumn;
                        if ((Boolean?)nlmColumn.AspectEngine.GetAspect(baseTreeNode.Parent) == false)
                        {
                            parentValue = false;
                            break;
                        }
                        baseTreeNode = baseTreeNode.Parent;
                    }
                    if (parentValue)
                    {
                        image = GetActiveImage(aspectValue);
                    }
                    else
                    {
                        image = GetInactiveImage(aspectValue);
                    }
                }
                // Dumb folder mode is a lot simpler :)
                else
                {
                    image = GetActiveImage(aspectValue);
                }
            }
            // If the item is disabled we want it to have 50% opacity, so we always use the inactive image.
            else 
            {
                image = GetInactiveImage(aspectValue);
            }

            // Finally, after all that work, let's draw the image.
            if (image != null)
                this.DrawAlignedImage(g, r, image);
        }
    }
}
