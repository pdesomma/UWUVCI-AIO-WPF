using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace WiiUInjector.GitTools
{
    /// <summary>
    /// Makes logos
    /// </summary>
    internal sealed class LogoPngGenerator : PngGenerator
    {
        private readonly string _text;
        private readonly float _fontSize;

        /// <summary>
        /// Creates a new instance of the <see cref="LogoPngGenerator"/> class.
        /// </summary>
        /// <param name="overlayPath"></param>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        public LogoPngGenerator(string overlayPath, string text, float fontSize)
        {
            width = 170;
            height = 42;
            overlay = overlayPath;
            _text = text;
            _fontSize = fontSize;
        }

        /// <summary>
        /// Creates a logo <see cref="Bitmap"/>.
        /// </summary>
        /// <returns></returns>
        protected override byte[] DoCreationWork(Graphics graphics, Bitmap img, MemoryStream stream, Bitmap overlayBitmap, Bitmap sourceBitmap)
        {
            graphics.Clear(Color.FromArgb(30, 30, 30));
            graphics.DrawImage(overlayBitmap, 0, 0, width, height);

            Rectangle rectangletxt = new Rectangle(18, 5, 134, 32);

            using (var privateFonts = new PrivateFontCollection())
            {
                privateFonts.AddFontFile(Path.Combine(Directory.GetCurrentDirectory(), "fonts", "logo.ttf"));
                using (var font = new Font(privateFonts.Families[0], _fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    TextRenderer.DrawText(graphics, _text, font, rectangletxt, Color.FromArgb(180, 180, 180), Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.GlyphOverhangPadding);

                    img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    return stream.ToArray();
                }
            }
        }
    }
}
