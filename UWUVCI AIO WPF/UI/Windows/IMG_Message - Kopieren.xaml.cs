using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WiiUInjector;
using UWUVCI_AIO_WPF.UI.Frames.InjectFrames.Configurations;
using UWUVCI_AIO_WPF.ViewModels;
using Path = System.IO.Path;

namespace UWUVCI_AIO_WPF.UI.Windows
{
    /// <summary>
    /// Interaktionslogik für IMG_Message.xaml
    /// </summary>
    public partial class TDRSHOW : Window, IDisposable
    {
        private static readonly string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "temp");
        private static readonly string toolsPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "Tools");
        readonly string copy = "";
        readonly string pat = "";
        readonly bool drc = false;
        readonly BitmapImage bitmap = new BitmapImage();
        public TDRSHOW(string path, bool drc)
        {
            this.drc = drc;
            try
            {
                if (this.Owner?.GetType() != typeof(MainWindow))
                {
                    this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }
            catch (Exception)
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            pat = String.Copy(path);
            InitializeComponent();
            if (Directory.Exists(Path.Combine(tempPath, "image"))) Directory.Delete(Path.Combine(tempPath, "image"), true);
            Directory.CreateDirectory(Path.Combine(tempPath, "image"));
            if (pat == "Added via Config")
            {
            }
            if (new FileInfo(pat).Extension.Contains("tga"))
            {
                using (Process conv = new Process())
                {

                    conv.StartInfo.UseShellExecute = false;
                    conv.StartInfo.CreateNoWindow = true;


                    conv.StartInfo.FileName = Path.Combine(toolsPath, "tga2png.exe");
                    conv.StartInfo.Arguments = $"-i \"{pat}\" -o \"{Path.Combine(tempPath, "image")}\"";

                    conv.Start();
                    conv.WaitForExit();

                    foreach (string sFile in Directory.GetFiles(Path.Combine(tempPath, "image"), "*.png"))
                    {
                        copy = sFile;
                    }
                }
            }
            else
            {
                copy = pat;
            }




            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(copy, UriKind.Absolute);
            image.EndInit();
            image.Freeze();
            img.Source = image;

            if (path == "Added via Config")
            {
                File.Delete(pat);
            }
        }

        public void Dispose()
        {

        }

        private void Canc_Click(object sender, RoutedEventArgs e)
        {
            bitmap.UriSource = null;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
