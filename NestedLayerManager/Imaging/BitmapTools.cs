using System.Drawing;
using System.Drawing.Imaging;

namespace NestedLayerManager.Imaging
{
    internal class BitmapTools
    {
        // Return a bitmap with a different opacity.
        internal static Bitmap ChangeOpacity(Bitmap bitmap, float opacityvalue)
        {
            Bitmap bmp = new Bitmap(bitmap.Width, bitmap.Height);
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new ColorMatrix();
            colormatrix.Matrix33 = opacityvalue;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(bitmap, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();

            return bmp;
        }

        // Imagelists have a really horrible bug with partial pixel transparency.
        // When you add an image to an ImageList, (I think) it is presumed that the background colour is mid gray (RGB 128).
        // This causes problems with any pixel that is partially transparent, as the colour it displays is not the desired and correct value.
        // This method detects any pixel that is partially transparent, and bakes the backColor into the pixel, and then gives the pixel 0 transparency.
        internal static Bitmap AlphaBake(Bitmap originalBitmap, Color backColor)
        {
            Bitmap newBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height);

            for (int y = 0; y < originalBitmap.Height; ++y)
            {
                for (int x = 0; x < originalBitmap.Width; ++x)
                {
                    Color originalColor = originalBitmap.GetPixel(x, y);

                    if (originalColor.A != 0 && originalColor.A != 255)
                    {

                        // 255 = fully opaque
                        // 0 = fully transparent

                        int a = 255;
                        int r = ((originalColor.R * originalColor.A) + (backColor.R * (255 - originalColor.A))) / 255;
                        int g = ((originalColor.G * originalColor.A) + (backColor.G * (255 - originalColor.A))) / 255;
                        int b = ((originalColor.B * originalColor.A) + (backColor.B * (255 - originalColor.A))) / 255;

                        Color newColor = Color.FromArgb(a, r, g, b);
                        newBitmap.SetPixel(x, y, newColor);
                    }
                    else
                    {
                        newBitmap.SetPixel(x, y, originalColor);
                    }
                }
            }

            return newBitmap;
        }
    }
}
