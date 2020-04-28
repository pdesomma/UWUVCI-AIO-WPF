﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UWUVCI_AIO_WPF.Properties;
using UWUVCI_AIO_WPF.UI.Windows;

namespace UWUVCI_AIO_WPF.UI.Frames.InjectFrames.Configurations
{
    /// <summary>
    /// Interaktionslogik für WiiConfig.xaml
    /// </summary>
    public partial class WiiConfig : Page, IDisposable
    {
        MainViewModel mvm;
        bool dont = true;
        public WiiConfig()
        {
            InitializeComponent();
            mvm = FindResource("mvm") as MainViewModel;
            mvm.setThing(this);
            Injection.ToolTip = "Changing the extension of a ROM may result in a faulty inject.\nWe will not give any support in such cases";
            List<string> gpEmu = new List<string>();
            gpEmu.Add("None");
            gpEmu.Add("Classic Controller");
            gpEmu.Add("Horizontal WiiMote");
            gpEmu.Add("Vertical WiiMote");
            gpEmu.Add("Force Classic Controller");
            gpEmu.Add("Force No Classic Controller");
            gamepad.ItemsSource = gpEmu;
            gamepad.SelectedIndex = 0;
            mvm.test = GameBaseClassLibrary.GameConsoles.WII;
        }
        public WiiConfig(GameConfig c)
        {
            InitializeComponent();
            mvm = FindResource("mvm") as MainViewModel;
            getInfoFromConfig();
            mvm.GameConfiguration = c.Clone();
            mvm.setThing(this);
            Injection.ToolTip = "Changing the extension of a ROM may result in a faulty inject.\nWe will not give any support in such cases";
            List<string> gpEmu = new List<string>();
            gpEmu.Add("None");
            gpEmu.Add("Classic Controller");
            gpEmu.Add("Horizontal WiiMote");
            gpEmu.Add("Vertical WiiMote");
            gpEmu.Add("Force Classic Controller");
            gpEmu.Add("Force No Classic Controller");
            gamepad.ItemsSource = gpEmu;
            gamepad.SelectedIndex = 0;
            mvm.test = GameBaseClassLibrary.GameConsoles.WII;
        }
        public void Dispose()
        {

        }

        private void Set_Rom_Path(object sender, RoutedEventArgs e)
        {
            string path = mvm.GetFilePath(true, false);
            if (!CheckIfNull(path))

            {
                int TitleIDInt = 0;
                bool isok = false;
                using (var reader = new BinaryReader(File.OpenRead(path)))
                {
                    reader.BaseStream.Position = 0x00;
                    TitleIDInt = reader.ReadInt32();
                    if (TitleIDInt == 1397113431) //Performs actions if the header indicates a WBFS file
                    { isok = true; }
                    else if (TitleIDInt != 65536)
                    {
                        long GameType = 0;
                        reader.BaseStream.Position = 0x18;
                        GameType = reader.ReadInt64();
                        if (GameType == 2745048157)
                        {
                            isok = true;
                        }

                    }
                    reader.Close();
                }
                if (isok)
                {
                    mvm.RomPath = path;
                    mvm.RomSet = true;
                    if (mvm.BaseDownloaded)
                    {
                        mvm.CanInject = true;

                    }
                    string rom = mvm.getInternalWIIGCNName(mvm.RomPath, false);
                    Regex reg = new Regex("[*'\",_&#^@:;?!<>|µ~#°²³´`éⓇ©™]");
                    gn.Text = reg.Replace(rom, string.Empty);
                    mvm.GameConfiguration.GameName = reg.Replace(rom, string.Empty);
                    if (mvm.GameConfiguration.TGAIco.ImgPath != "" || mvm.GameConfiguration.TGAIco.ImgPath != null)
                    {
                        ic.Text = mvm.GameConfiguration.TGAIco.ImgPath;
                    }
                    if (mvm.GameConfiguration.TGATv.ImgPath != "" || mvm.GameConfiguration.TGATv.ImgPath != null)
                    {
                        tv.Text = mvm.GameConfiguration.TGATv.ImgPath;
                    }
                }
                else
                {
                    Custom_Message cm = new Custom_Message("Wrong ROM", "The chosen ROM is not a supported WII Game");
                    try
                    {
                        cm.Owner = mvm.mw;
                    }
                    catch (Exception)
                    {

                    }
                    cm.ShowDialog();
                }

            }


        }

        private void InjectGame(object sender, RoutedEventArgs e)
        {
            if (File.Exists(tv.Text))
            {
                mvm.GameConfiguration.TGATv.ImgPath = tv.Text;
            }
            else if (!tv.Text.Equals("Added via Config") && !tv.Text.Equals("Downloaded from Cucholix Repo"))
            {
                mvm.GameConfiguration.TGATv.ImgPath = null;
            }
            if (File.Exists(ic.Text))
            {
                mvm.GameConfiguration.TGAIco.ImgPath = ic.Text;
            }
            else if (!ic.Text.Equals("Added via Config") && !ic.Text.Equals("Downloaded from Cucholix Repo"))
            {
                mvm.GameConfiguration.TGAIco.ImgPath = null;

            }
            if (File.Exists(log.Text))
            {
                mvm.GameConfiguration.TGALog.ImgPath = log.Text;
            }
            else if (!log.Text.Equals("Added via Config") && !log.Text.Equals("Downloaded from Cucholix Repo"))
            {
                mvm.GameConfiguration.TGALog.ImgPath = null;
            }
            if (File.Exists(drc.Text))
            {
                mvm.GameConfiguration.TGADrc.ImgPath = drc.Text;
            }
            else if (!drc.Text.Equals("Added via Config") && !drc.Text.Equals("Downloaded from Cucholix Repo"))
            {
                mvm.GameConfiguration.TGADrc.ImgPath = null;
            }
            mvm.Index = gamepad.SelectedIndex;
            if (LR.IsChecked == true)
            {
                mvm.LR = true;
            }
            else
            {
                mvm.LR = false;
            }
            mvm.Inject(false);
        }

        private void Set_TvTex(object sender, RoutedEventArgs e)
        {
            if (!Settings.Default.dont)
            {
                mvm.ImageWarning();
            }
            string path = mvm.GetFilePath(false, false);
            if (!CheckIfNull(path))
            {
                mvm.GameConfiguration.TGATv.ImgPath = path;
                mvm.GameConfiguration.TGATv.extension = new FileInfo(path).Extension;
                tv.Text = path;
                tvIMG.Visibility = Visibility.Visible;
            }


        }

        private void Set_DrcTex(object sender, RoutedEventArgs e)
        {
            if (!Settings.Default.dont)
            {
                mvm.ImageWarning();
            }
            string path = mvm.GetFilePath(false, false);
            if (!CheckIfNull(path))
            {
                mvm.GameConfiguration.TGADrc.ImgPath = path;
                mvm.GameConfiguration.TGADrc.extension = new FileInfo(path).Extension;
                drc.Text = path;
                drcIMG.Visibility = Visibility.Visible;
            }


        }

        private void Set_IconTex(object sender, RoutedEventArgs e)
        {
            if (!Settings.Default.dont)
            {
                mvm.ImageWarning();
            }
            string path = mvm.GetFilePath(false, false);
            if (!CheckIfNull(path))
            {
                mvm.GameConfiguration.TGAIco.ImgPath = path;
                mvm.GameConfiguration.TGAIco.extension = new FileInfo(path).Extension;
                ic.Text = path;
                icoIMG.Visibility = Visibility.Visible;
            }

        }

        private void Set_LogoTex(object sender, RoutedEventArgs e)
        {
            if (!Settings.Default.dont)
            {
                mvm.ImageWarning();
            }
            string path = mvm.GetFilePath(false, false);
            if (!CheckIfNull(path))
            {
                mvm.GameConfiguration.TGALog.ImgPath = path;
                mvm.GameConfiguration.TGALog.extension = new FileInfo(path).Extension;
                log.Text = path;
                logIMG.Visibility = Visibility.Visible;
            }

        }
        public void getInfoFromConfig()
        {
            rp.Text = "";
            mvm.RomPath = "";
            mvm.RomSet = false;
            mvm.gc2rom = "";
            tv.Text = mvm.GameConfiguration.TGATv.ImgPath;
            if (tv.Text.Length > 0)
            {
                tvIMG.Visibility = Visibility.Visible;
            }
            ic.Text = mvm.GameConfiguration.TGAIco.ImgPath;
            if (ic.Text.Length > 0)
            {
                icoIMG.Visibility = Visibility.Visible;
            }
            drc.Text = mvm.GameConfiguration.TGADrc.ImgPath;
            if (drc.Text.Length > 0)
            {
                drcIMG.Visibility = Visibility.Visible;
            }
            log.Text = mvm.GameConfiguration.TGALog.ImgPath;
            if (log.Text.Length > 0)
            {
                logIMG.Visibility = Visibility.Visible;
            }
            gn.Text = mvm.GameConfiguration.GameName;

        }
        private bool CheckIfNull(string s)
        {
            if (s == null || s.Equals(string.Empty))
            {
                return true;
            }
            return false;
        }

        private void gn_KeyUp(object sender, KeyEventArgs e)
        {

            /*Regex reg = new Regex("[^a-zA-Z0-9 é -]");
       string backup = string.Copy(gn.Text);
       gn.Text = reg.Replace(gn.Text, string.Empty);
       gn.CaretIndex = gn.Text.Length;
       if (gn.Text != backup)
       {
           gn.ScrollToHorizontalOffset(double.MaxValue);
       }*/




        }

        private void gn_KeyUp_1(object sender, KeyEventArgs e)
        {

        }

        private void gamepad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gamepad.SelectedIndex == 1 || gamepad.SelectedIndex == 4)
            {
                LR.IsEnabled = true;
            }
            else
            {
                LR.IsChecked = false;
                LR.IsEnabled = false;
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            mvm.toPal = true;
            mvm.Patch = true;
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            mvm.toPal = false;
            mvm.Patch = true;
        }

        private void RadioButton_Click_2(object sender, RoutedEventArgs e)
        {
            mvm.toPal = false;
            mvm.Patch = false;
        }
        public void reset()
        {
            tv.Text = "";
            drc.Text = "";
            gn.Text = "";
            ic.Text = "";
            log.Text = "";


        }
        private void icoIMG_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new ICOSHOW(ic.Text).ShowDialog();
        }

        private void tvIMG_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new TDRSHOW(tv.Text,false).ShowDialog();
        }

        private void drcIMG_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TDRSHOW t = new TDRSHOW(drc.Text,true);
            t.ShowDialog();

        }

        private void logIMG_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new LOGSHOW(log.Text).ShowDialog();
        }

        private void ic_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ic.Text.Length > 0)
            {
                icoIMG.Visibility = Visibility.Visible;
            }
            else
            {
                icoIMG.Visibility = Visibility.Hidden;
            }
        }

        private void drc_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (drc.Text.Length > 0)
            {
                drcIMG.Visibility = Visibility.Visible;
            }
            else
            {

                drcIMG.Visibility = Visibility.Hidden;
            }
        }

        private void tv_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tv.Text.Length > 0)
            {
                tvIMG.Visibility = Visibility.Visible;
            }
            else
            {
                tvIMG.Visibility = Visibility.Hidden;
            }
        }

        private void log_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (log.Text.Length > 0)
            {
                logIMG.Visibility = Visibility.Visible;
            }
            else
            {
                logIMG.Visibility = Visibility.Hidden;
            }
        }

        private void gn_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Up) || Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Left) || Keyboard.IsKeyDown(Key.Right))
            {
                dont = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = mvm.GetFilePath(true, true);
            if (!CheckIfNull(path))
            {
                if (new FileInfo(path).Extension.Contains("wav"))
                {
                    if (mvm.ConfirmRiffWave(path))
                    {
                        mvm.BootSound = path;
                    }
                    else
                    {
                        Custom_Message cm = new Custom_Message("Incompatible WAV file", "Your WAV file needs to be a RIFF WAVE file which is 16 bit stereo and also 48000khz");
                        try
                        {
                            cm.Owner = mvm.mw;
                        }
                        catch (Exception)
                        {

                        }
                        cm.ShowDialog();
                    }
                }
                else
                {

                    mvm.BootSound = path;
                }
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string url = mvm.GetURL("wii");
                if (url == null || url == "") throw new Exception();
                TitleKeys webbrowser = new TitleKeys(url, "UWUVCI AIO - Wii Help");
                try
                {
                    webbrowser.Owner = mvm.mw;
                }
                catch (Exception)
                {

                }
                webbrowser.ShowDialog();
            }
            catch (Exception)
            {
                Custom_Message cm = new Custom_Message("Not Implemented", "The Helppage for Wii is not implemented yet");
                try
                {
                    cm.Owner = mvm.mw;
                }
                catch (Exception)
                {

                }
                cm.Show();
            }
        }
    }
}