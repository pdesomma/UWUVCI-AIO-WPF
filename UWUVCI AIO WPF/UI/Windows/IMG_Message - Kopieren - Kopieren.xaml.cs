﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UWUVCI_AIO_WPF.UI.Frames.InjectFrames.Configurations;

namespace UWUVCI_AIO_WPF.UI.Windows
{
    /// <summary>
    /// Interaktionslogik für IMG_Message.xaml
    /// </summary>
    public partial class ICOSHOW : Window, IDisposable
    {
        private static readonly string tempPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "bin", "temp");
        private static readonly string toolsPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "bin", "Tools");
        string copy = "";
        string pat = "";
        BitmapImage bitmap = new BitmapImage();
        public ICOSHOW(string path)
        {

            pat = String.Copy(path);

            InitializeComponent();
            if (Directory.Exists(System.IO.Path.Combine(tempPath, "image"))) Directory.Delete(System.IO.Path.Combine(tempPath, "image"), true);
            Directory.CreateDirectory(System.IO.Path.Combine(tempPath, "image"));
            if(path == "Added via Config")
            {
                File.WriteAllBytes(System.IO.Path.Combine(tempPath, "image", "ico." + (FindResource("mvm") as MainViewModel).GameConfiguration.TGAIco.extension), (FindResource("mvm") as MainViewModel).GameConfiguration.TGAIco.ImgBin);
                pat = System.IO.Path.Combine(tempPath, "image", "ico." + (FindResource("mvm") as MainViewModel).GameConfiguration.TGAIco.extension);
            }
            if (new FileInfo(pat).Extension.Contains("tga"))
            {
                using (Process conv = new Process())
                {

                    conv.StartInfo.UseShellExecute = false;
                    conv.StartInfo.CreateNoWindow = true;


                    conv.StartInfo.FileName = System.IO.Path.Combine(toolsPath, "tga2png.exe");
                    conv.StartInfo.Arguments = $"-i \"{pat}\" -o \"{System.IO.Path.Combine(tempPath, "image")}\"";

                    conv.Start();
                    conv.WaitForExit();

                    foreach (string sFile in Directory.GetFiles(System.IO.Path.Combine(tempPath, "image"), "*.png"))
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



    }
}