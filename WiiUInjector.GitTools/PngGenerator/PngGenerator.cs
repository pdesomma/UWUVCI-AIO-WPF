using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;

namespace WiiUInjector.GitTools
{
    abstract class PngGenerator : IPngGenerator
    {
        protected int width;
        protected int height;
        protected string overlay;
        protected string source;

        /// <summary>
        /// Creates bytes that represent a bitmap.
        /// </summary>
        /// <returns></returns>
        public byte[] Create()
        {
            using (var overlayBitmap = string.IsNullOrWhiteSpace(overlay) ? null : new Bitmap(overlay))
            using (var sourceBitmap = string.IsNullOrWhiteSpace(source) ? null : new Bitmap(source))
            using (var stream = new MemoryStream())
            using (var img = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(img))
            {
                graphics.PixelOffsetMode = PixelOffsetMode.Half;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                return DoCreationWork(graphics, img, stream, overlayBitmap, sourceBitmap);
            }
        }

        /// <summary>
        /// Do image-specific creation work.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="img"></param>
        /// <param name="overlayBitmap"></param>
        /// <param name="sourceBitmap"></param>
        /// <returns></returns>
        protected abstract byte[] DoCreationWork(Graphics graphics, Bitmap img, MemoryStream stream, Bitmap overlayBitmap, Bitmap sourceBitmap); 

        /// <summary>
        /// Gets the first transparent rectangle in a bitmap starting from the top left corner
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        protected static Rectangle FindEmptyRectangle(Bitmap bitmap)
        {
            int width = 0, height = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, y).A == 0)
                    {
                        for (int i = x; i < bitmap.Width; i++)
                        {
                            if (bitmap.GetPixel(i, y).A == 0)
                            {
                                width++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        for (int i = y; i < bitmap.Height; i++)
                        {
                            if (bitmap.GetPixel(x, i).A == 0) 
                                height++;
                            else
                                break;
                        }
                        return new Rectangle(x, y, width, height);
                    }
                }
            }
            return Rectangle.Empty;
        }
    }
}
