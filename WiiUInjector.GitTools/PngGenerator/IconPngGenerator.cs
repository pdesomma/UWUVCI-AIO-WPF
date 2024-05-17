using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WiiUInjector.GitTools
{
    sealed class IconPngGenerator : PngGenerator
    {
        /// <summary>
        /// Create a new instance of the <see cref="IconPngGenerator"/> class.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="overlayPath"></param>
        public IconPngGenerator(string sourcePath, string overlayPath) 
        { 
            overlay = overlayPath;
            source = sourcePath;
            height = width = 128;
        }

        /// <summary>
        /// Creates an icon <see cref="Bitmap"/>.
        /// </summary>
        /// <returns></returns>
        protected override byte[] DoCreationWork(Graphics graphics, Bitmap img, MemoryStream stream, Bitmap overlayBitmap, Bitmap sourceBitmap)
        {
            graphics.Clear(Color.FromArgb(30, 30, 30));
            if (sourceBitmap != null)
            {
                if (overlayBitmap != null)
                {
                    graphics.DrawImage(sourceBitmap, FindEmptyRectangle(overlayBitmap));
                }
                else graphics.DrawImage(sourceBitmap, new Rectangle(0, 0, width, height));
            }
            if (overlayBitmap != null) graphics.DrawImage(overlayBitmap, new Rectangle(0, 0, width, height));

            img.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
    }
}