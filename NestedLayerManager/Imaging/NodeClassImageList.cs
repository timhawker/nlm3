using System.Windows.Forms;
using System.Drawing;
using System;
using NestedLayerManager.SubControls;

namespace NestedLayerManager.Imaging
{
    public static class NodeClassImageList
    {
        public static void Apply(NlmTreeListView listView)
        {
            ImageList il = new ImageList();

            il.ColorDepth = ColorDepth.Depth32Bit;
            il.ImageSize = new Size(18, 18);

            Type type = typeof(NlmTreeListView);

            // TODO: 
            // Sort this out so you dont add an index any more (like layer)
            // Make BitmapTools accept a bitmap, not an image.

            Bitmap bone = BitmapTools.AlphaBake(new Bitmap(type.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.NodeClass.Bone.png")), listView.BackColor);
            il.Images.Add("Bone", bone);
            il.Images.Add("BoneDisabled", BitmapTools.ChangeOpacity(bone, 0.5F)); 

            Bitmap camera = BitmapTools.AlphaBake(new Bitmap(type.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.NodeClass.Camera.png")), listView.BackColor);
            il.Images.Add("Camera", camera);
            il.Images.Add("CameraDisabled", BitmapTools.ChangeOpacity(camera, 0.5F)); 

            Bitmap folder = BitmapTools.AlphaBake(new Bitmap(type.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.NodeClass.Folder.png")), listView.BackColor);
            il.Images.Add("Folder", folder);
            il.Images.Add("FolderDisabled", BitmapTools.ChangeOpacity(folder, 0.5F)); 

            Bitmap helper = BitmapTools.AlphaBake(new Bitmap(type.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.NodeClass.Helper.png")), listView.BackColor);
            il.Images.Add("Helper", helper);
            il.Images.Add("HelperDisabled", BitmapTools.ChangeOpacity(helper, 0.5F)); 

            Bitmap layer = BitmapTools.AlphaBake(new Bitmap(type.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.NodeClass.Layer.png")), listView.BackColor);
            il.Images.Add("Layer", layer);
            il.Images.Add("LayerDisabled", BitmapTools.ChangeOpacity(layer, 0.5F)); 

            Bitmap layerInstance = BitmapTools.AlphaBake(new Bitmap(type.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.NodeClass.LayerInstance.png")), listView.BackColor);
            il.Images.Add("LayerInstance", layerInstance);
            il.Images.Add("LayerInstanceDisabled", BitmapTools.ChangeOpacity(layerInstance, 0.5F)); 

            Bitmap light = BitmapTools.AlphaBake(new Bitmap(type.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.NodeClass.Light.png")), listView.BackColor);
            il.Images.Add("Light", light);
            il.Images.Add("LightDisabled", BitmapTools.ChangeOpacity(light, 0.5F)); 

            Bitmap obj = BitmapTools.AlphaBake(new Bitmap(type.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.NodeClass.Object.png")), listView.BackColor);
            il.Images.Add("Object", obj);
            il.Images.Add("ObjectDisabled", BitmapTools.ChangeOpacity(obj, 0.5F)); 

            Bitmap spaceWarp = BitmapTools.AlphaBake(new Bitmap(type.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.NodeClass.SpaceWarp.png")), listView.BackColor);
            il.Images.Add("SpaceWarp", spaceWarp);
            il.Images.Add("SpaceWarpDisabled", BitmapTools.ChangeOpacity(spaceWarp, 0.5F)); 

            Bitmap shape = BitmapTools.AlphaBake(new Bitmap(type.Assembly.GetManifestResourceStream("NestedLayerManager.Resources.Icons.NodeClass.Shape.png")), listView.BackColor);
            il.Images.Add("Shape", shape);
            il.Images.Add("ShapeDisabled", BitmapTools.ChangeOpacity(shape, 0.5F)); 

            listView.SmallImageList = il;
        }
    }
}
