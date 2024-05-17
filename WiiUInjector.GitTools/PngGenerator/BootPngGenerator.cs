using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;

namespace WiiUInjector.GitTools
{
    sealed class BootPngGenerator : PngGenerator
    {
        private readonly string _name1;
        private readonly string _name2;
        private readonly int _year;
        private readonly int _players;

        /// <summary>
        /// Create a new instance of the <see cref="BootPngGenerator"/> class.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="overlayPath"></param>
        /// <param name="name1"></param>
        /// <param name="name2"></param>
        /// <param name="year"></param>
        /// <param name="players"></param>
        public BootPngGenerator(string sourcePath, string overlayPath, string name1, string name2, int year, int players)
        {
            overlay = overlayPath;
            source = sourcePath;
            width = 1280;
            height = 720;
            _name1 = name1;
            _name2 = name2;
            _year = year;
            _players = players;
        }

        /// <summary>
        /// Create a boot image.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="img"></param>
        /// <param name="stream"></param>
        /// <param name="overlayBitmap"></param>
        /// <param name="sourceBitmap"></param>
        /// <returns></returns>
        protected override byte[] DoCreationWork(Graphics graphics, Bitmap img, MemoryStream stream, Bitmap overlayBitmap, Bitmap sourceBitmap)
        {
            graphics.Clear(Color.White);
            using (var font = new Font("Trebuchet MS", 10.0F, FontStyle.Bold, GraphicsUnit.Point))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(32, 32, 32)))
            using (Pen outline = new Pen(Color.FromArgb(222, 222, 222), 4.0F))
            using (Pen shadow = new Pen(Color.FromArgb(190, 190, 190), 6.0F))
            {
                StringFormat format = new StringFormat();
                if (sourceBitmap != null)
                {
                    if (overlayBitmap != null)
                    {
                        graphics.DrawImage(sourceBitmap, new Rectangle(131, 249, 400, 300));
                        DrawName(graphics, font, format, brush);
                        DrawYear(graphics, font, format, outline, shadow, brush);
                        DrawPlayers(graphics, font, format, outline, shadow, brush);
                    }
                    else graphics.DrawImage(sourceBitmap, new Rectangle(0, 0, width, height));
                }
                if (overlayBitmap != null)
                {
                    graphics.DrawImage(overlayBitmap, new Rectangle(0, 0, width, height));
                    DrawName(graphics, font, format, brush);
                    DrawYear(graphics, font, format, outline, shadow, brush);
                    DrawPlayers(graphics, font, format, outline, shadow, brush);
                }

                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Draw the game name
        /// </summary>
        /// <param name="g"></param>
        /// <param name="font"></param>
        /// <param name="format"></param>
        /// <param name="brush"></param>
        private void DrawName(Graphics g, Font font, StringFormat format, Brush brush)
        {
            if (!string.IsNullOrWhiteSpace(_name1))
            {
                using (Pen outlineBold = new Pen(Color.FromArgb(222, 222, 222), 5.0F))
                using (Pen shadowBold = new Pen(Color.FromArgb(190, 190, 190), 7.0F))
                {
                    Rectangle rect1 = !string.IsNullOrWhiteSpace(_name2) ? new Rectangle(578, 313, 640, 50) : new Rectangle(578, 340, 640, 50);
                    Rectangle rect2 = new Rectangle(578, 368, 640, 50);

                    Draw(_name1, g, font, format, outlineBold, shadowBold, brush, (int)FontStyle.Bold, 37.0F / 72.0F, rect1);
                    if (!string.IsNullOrWhiteSpace(_name2)) Draw(_name2, g, font, format, outlineBold, shadowBold, brush, (int)FontStyle.Bold, 37.0F / 72.0F, rect2);
                }
            }
        }

        /// <summary>
        /// Draw the release year text.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="font"></param>
        /// <param name="format"></param>
        /// <param name="outline"></param>
        /// <param name="shadow"></param>
        /// <param name="brush"></param>
        private void DrawYear(Graphics g, Font font, StringFormat format, Pen outline, Pen shadow, Brush brush)
        {
            if (_year > 0)
            {
                int year = Math.Min(_year, DateTime.Now.Year);
                var s = IsJapanese() ? year.ToString() + "年発売"  : "Released: " + year.ToString();

                Draw(s, g, font, format, outline, shadow, brush, (int)FontStyle.Regular, 25.0F / 72.0F, new Rectangle(586, 450, 600, 40));
            }
        }

        /// <summary>
        /// Draw the players text.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="font"></param>
        /// <param name="format"></param>
        /// <param name="outline"></param>
        /// <param name="shadow"></param>
        /// <param name="brush"></param>
        private void DrawPlayers(Graphics g, Font font, StringFormat format, Pen outline, Pen shadow, Brush brush)
        {
            if(_players >  0)
            {
                string s = "1" + (_players > 1 ? "-" + Math.Min(4, _players) : "");
                if (IsJapanese())
                    s = "プレイ人数　" + s + "人";
                else
                    s = "Players: " + s;

                Draw(s, g, font, format, outline, shadow, brush, (int)FontStyle.Regular, 25.0F / 72.0F, new Rectangle(586, 496, 600, 40));
            }
        }

        /// <summary>
        /// Checks to see if the game name is Japanese.
        /// </summary>
        /// <returns></returns>
        private bool IsJapanese()
        {   
            foreach (char c in (_name1 ?? string.Empty))
            {
                UnicodeCategory cat = char.GetUnicodeCategory(c);
                if (cat == UnicodeCategory.OtherLetter) // this covers Hiragana, Katakana, and Kanji
                    return true;
            }
            foreach (char c in (_name2 ?? string.Empty))
            {
                UnicodeCategory cat = char.GetUnicodeCategory(c);
                if (cat == UnicodeCategory.OtherLetter) // this covers Hiragana, Katakana, and Kanji
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Draws something on the bitmap.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="g"></param>
        /// <param name="font"></param>
        /// <param name="format"></param>
        /// <param name="outline"></param>
        /// <param name="shadow"></param>
        /// <param name="brush"></param>
        /// <param name="fontStyle"></param>
        /// <param name="dpiFact"></param>
        /// <param name="rect"></param>
        private void Draw(string s, Graphics g, Font font, StringFormat format, Pen outline, Pen shadow, Brush brush, int fontStyle, float dpiFact, Rectangle rect)
        {
            using (GraphicsPath p = new GraphicsPath())
            {
                p.AddString(s, font.FontFamily, fontStyle, g.DpiY * dpiFact, rect, format);
                g.DrawPath(shadow, p);
                g.DrawPath(outline, p);
                g.FillPath(brush, p);
            }
        }
    }
}
