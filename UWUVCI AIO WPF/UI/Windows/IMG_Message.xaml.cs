using Pfim;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WiiUInjector;
using UWUVCI_AIO_WPF.UI.Frames.InjectFrames.Configurations;
using UWUVCI_AIO_WPF.ViewModels;
using Image = System.Windows.Controls.Image;
using Path = System.IO.Path;
using PixelFormat = System.Windows.Media.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace UWUVCI_AIO_WPF.UI.Windows
{
    /// <summary>
    /// Interaktionslogik für IMG_Message.xaml
    /// </summary>
    public partial class IMG_Message : Window
    {
        readonly string ic = "";
        readonly string tvs = "";
        public IMG_Message(string icon, string tv)
        {
            try
            {
                if (this.Owner != null)
                {
                    if (this.Owner?.GetType() != typeof(MainWindow))
                    {
                        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    }
                }
            }
            catch (Exception)
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            InitializeComponent();

            ic = icon;
            tvs = tv;
            if (ic.Contains("tga"))
            {
                icb.Visibility = Visibility.Hidden;
                icl.Visibility = Visibility.Hidden;
                //tgic.Visibility = Visibility.Visible;
            }
            if (tvs.Contains("tga"))
            {
                tvb.Visibility = Visibility.Hidden;
                tvl.Visibility = Visibility.Hidden;
                tgtv.Visibility = Visibility.Visible;
            }


            this.icon.Source = GetRepoImages(icon);
            this.tv.Source = GetRepoImages(tv);
        }

        private BitmapImage GetRepoImages(string imageURL)
        {
            BitmapImage bitmap = new BitmapImage();
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(new Uri(imageURL, UriKind.Absolute));
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                var webReponse = webRequest.GetResponse();
                var stream = webReponse.GetResponseStream();

                if (!imageURL.EndsWith(".tga"))
                {
                    var image = System.Drawing.Image.FromStream(stream);

                    using (var graphics = Graphics.FromImage(image))
                    {
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                    }

                    using (var memory = new MemoryStream())
                    {
                        image.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                        memory.Position = 0;

                        bitmap.BeginInit();
                        bitmap.StreamSource = memory;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                    }
                }
                else
                {
                    //This code doesn't work
                    /* 
                    var image = Pfim.Pfim.FromStream(stream);
                    foreach (var im in WpfImage(image))
                    {
                        var encoder = new PngBitmapEncoder();
                        using (var memory = new MemoryStream())
                        {
                            BitmapSource bitmapSource = (BitmapSource)im.Source;
                            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                            encoder.Save(memory);
                            memory.Position = 0;

                            bitmap.BeginInit();
                            bitmap.StreamSource = memory;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                        }
                    }
                    */

                    //using old method for .tga
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imageURL, UriKind.Absolute);
                    bitmap.EndInit();
                }
            }
            catch
            {
                //something broke, so yolo we go with the old method!
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageURL, UriKind.Absolute);
                bitmap.EndInit();
            }
            return bitmap;
        }

        private void Button_Click(object sender, RoutedEventArgs e) => Close();

        private void Canc_Click(object sender, RoutedEventArgs e) => Close();

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel mvm = DataContext as MainViewModel;
            var client = new WebClient();
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo")))
            {
                Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo"), true);
            }
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo"));
            client.DownloadFile(ic, Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo", $"iconTex.{ic.Split('.')[3]}"));
            if (mvm.test == GameConsole.GCN)
            {
                (mvm.Thing as GCConfig).icoIMG.Visibility = Visibility.Visible;
            }
            else if (mvm.GameConfiguration.Console == GameConsole.WII)
            {
                (mvm.Thing as WiiConfig).icoIMG.Visibility = Visibility.Visible;
            }
            client.DownloadFile(tvs, Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo", $"bootTvTex.{tvs.Split('.')[3]}"));
            if (mvm.test == GameConsole.GCN)
            {
                (mvm.Thing as GCConfig).tvIMG.Visibility = Visibility.Visible;
                (mvm.Thing as GCConfig).ImgPath(Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo", $"iconTex.{ic.Split('.')[3]}"), Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo", $"bootTvTex.{tvs.Split('.')[3]}"));

            }
            else if (mvm.GameConfiguration.Console == GameConsole.WII)
            {
                (mvm.Thing as WiiConfig).tvIMG.Visibility = Visibility.Visible;
                (mvm.Thing as WiiConfig).Imgpath(Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo", $"iconTex.{ic.Split('.')[3]}"), Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo", $"bootTvTex.{tvs.Split('.')[3]}"));

            }
            this.Close();
        }

        public void CheckForAdditionalFiles(GameConsole console, string repoid)
        {
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo"));
            }
            bool ini = false;
            bool btsnd = false;
            string inip = "";
            string btsndp = "";
            string exten = "";
            string linkbase = "https://raw.githubusercontent.com/UWUVCI-PRIME/UWUVCI-IMAGES/master/";
            if (console == GameConsole.N64)
            {
                if (RemoteFileExists(linkbase + repoid + "/game.ini"))
                {
                    ini = true;
                    inip = linkbase + repoid + "/game.ini";
                }
            }
            string[] ext = { "wav", "mp3", "btsnd" };
            foreach (var e in ext)
            {
                if (RemoteFileExists(linkbase + repoid + "/BootSound." + e))
                {
                    btsnd = true;
                    btsndp = linkbase + repoid + "/BootSound." + e;
                    exten = e;
                    break;
                }
            }
            if (ini || btsnd)
            {
                string extra = "There are more additional files found. Do you want to download those?";
                if (ini && !btsnd) { extra = "There is an additional INI file available for Dowload. Do you want to dowload it?"; }
                if (!ini && btsnd) { extra = "There is an additional BootSound file available for Dowload. Do you want to dowload it?"; }
                if (ini && btsnd) { extra = "There is an adittional INI and BootSound file available for Dowload. Do you want to download those?"; }
                MainViewModel mvm = DataContext as MainViewModel;
                Custom_Message cm = new Custom_Message("Found additional Files", extra);
                cm.ShowDialog();
                if (mvm.addi)
                {
                    var client = new WebClient();
                    if (ini)
                    {
                        client.DownloadFile(inip, Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo", "game.ini"));
                    }
                    if (btsnd)
                    {
                        client.DownloadFile(btsndp, Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo", $"bootSound.{exten}"));
                        switch (console)
                        {
                            case GameConsole.WII:
                                if (mvm.test == GameConsole.GCN)
                                {
                                    (mvm.Thing as GCConfig).sound.Text = Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo", $"bootSound.{exten}");
                                }
                                else
                                {
                                    (mvm.Thing as WiiConfig).sound.Text = Path.Combine(Directory.GetCurrentDirectory(), "bin", "repo", $"bootSound.{exten}");
                                }
                                break;

                        }
                    }
                    mvm.addi = false;
                }
            }
        }
        private bool RemoteFileExists(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }
    }
}
