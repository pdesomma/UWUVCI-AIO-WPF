using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WiiUInjector;
using UWUVCI_AIO_WPF.UI.Windows;
using UWUVCI_AIO_WPF.ViewModels;

namespace UWUVCI_AIO_WPF.UI.Frames.InjectFrames.Configurations
{
    /// <summary>
    /// Interaktionslogik für OtherConfigs.xaml
    /// </summary>
    public partial class GCConfig : Page, IDisposable
    {
        readonly MainViewModel mvm;
        bool cd = false;
        public GCConfig()
        {
            InitializeComponent();
            mvm = DataContext as MainViewModel;
            mvm.SetThing(this);
            Injection.ToolTip = "Changing the extension of a ROM may result in a faulty inject.\nWe will not give any support in such cases";
            mvm.test = GameConsole.GCN;
            mvm.Index = 1;
        }
        public void ClearImages(int i)
        {

            switch (i)
            {
                case 0:
                    icoIMG.Visibility = Visibility.Hidden;
                    ic.Text = null;
                    break;
                case 1:
                    tvIMG.Visibility = Visibility.Hidden;
                    tv.Text = null;
                    break;
                case 2:
                    drcIMG.Visibility = Visibility.Hidden;
                    drc.Text = null;
                    break;
                case 3:
                    logIMG.Visibility = Visibility.Hidden;
                    log.Text = null;
                    break;
            }
        }
        private void SoundImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { }

        public GCConfig(GameConfig c)
        {
            InitializeComponent();
            mvm = DataContext as MainViewModel;
            mvm.GameConfiguration = c.Clone(); 
            GetInfoFromConfig();
            mvm.SetThing(this);
            Injection.ToolTip = "Changing the extension of a ROM may result in a faulty inject.\nWe will not give any support in such cases";
            mvm.test = GameConsole.GCN;
            mvm.Index = 1;
        }
        public void Dispose()
        {

        }
        public void ImgPath(string icon, string tv)
        {
            ic.Text = icon;
            this.tv.Text = tv;
        }
        private async void Set_Rom_Path(object sender, RoutedEventArgs e)
        {
            string path = mvm.GetFilePath(true, false);            
            bool isok = false;
            if (!CheckIfNull(path))
            {
                using (var reader = new BinaryReader(File.OpenRead(path)))
                {
                    if (path.ToLower().Contains(".gcz"))
                    {
                        isok = true;
                    }
                    else
                    {
                        reader.BaseStream.Position = 0x00;
                        var TitleIDInt = reader.ReadInt32();
                        if (TitleIDInt != 65536 && TitleIDInt != 1397113431)
                        {
                            reader.BaseStream.Position = 0x18;
                            long GameType = reader.ReadInt64();
                            if (GameType == 4440324665927270400)
                            {
                                isok = true;
                            }
                        }
                        reader.Close();
                    }

                }
                if (isok)
                {
                    trimn.IsEnabled = true;
                    if (path.Contains("nkit.iso"))
                    {
                        trimn.IsEnabled = false;
                        trimn.IsChecked = false;
                        Trimn_Click(null, null);
                    }
                    mvm.RomPath = path;
                    mvm.RomSet = true;
                    if (!path.ToLower().Contains(".gcz"))
                    {
                        trimn.IsChecked = false;
                        Trimn_Click(null, null);
                        string rom = await mvm.GetInternalWIIGCNName(mvm.RomPath, true);
                        Regex reg = new Regex("[*'\",_&#^@:;?!<>|µ~#°²³´`éⓇ©™]");
                        gn.Text = reg.Replace(rom, string.Empty);
                        mvm.GameConfiguration.GameName = reg.Replace(rom, string.Empty);
                        mvm.GC2Rom = "";
                    }

                }
                else
                {
                    Custom_Message cm = new Custom_Message("Wrong ROM", "The chosen ROM is not a supported GameCube Game");
                    cm.ShowDialog();
                }

            }


        }

        private void InjectGame(object sender, RoutedEventArgs e)
        {
            mvm.GameConfiguration.GameName = gn.Text;
            mvm.GC = true;
            //mvm.Inject(cd);
            mvm.Index = 1;
            gp.IsChecked = false;
        }

        private void Set_TvTex(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "bin", "createdIMG", "bootTvTex.png");
            if (File.Exists(path))
            {
                tv.Text = path;
                tvIMG.Visibility = Visibility.Visible;
            }
        }

        private void Set_DrcTex(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "bin", "createdIMG", "bootDrcTex.png");
            if (File.Exists(path))
            {
                drc.Text = path;
                drcIMG.Visibility = Visibility.Visible;
            }
        }

        private void Set_IconTex(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "bin", "createdIMG", "iconTex.png");
            if (File.Exists(path))
            {
                this.ic.Text = path;
                icoIMG.Visibility = Visibility.Visible;
            }
        }

        private void Set_LogoTex(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "bin", "createdIMG", "bootLogoTex.png");
            if (File.Exists(path))
            {
                this.log.Text = path;
                logIMG.Visibility = Visibility.Visible;
            }
        }
        public void GetInfoFromConfig()
        {
            rp.Text = "";
            mvm.RomPath = "";
            mvm.RomSet = false;
            mvm.GC2Rom = "";
            gc2.Text = "";
            gn.Text = mvm.GameConfiguration.GameName;
            if (mvm.GameConfiguration.extension != "" && mvm.GameConfiguration.bootsound != null)
            {
                if (!Directory.Exists(@"bin\cfgBoot"))
                {
                    Directory.CreateDirectory(@"bin\cfgBoot");
                }
                if (File.Exists($@"bin\cfgBoot\bootSound.{mvm.GameConfiguration.extension}"))
                {
                    File.Delete($@"bin\cfgBoot\bootSound.{mvm.GameConfiguration.extension}");
                }
                File.WriteAllBytes($@"bin\cfgBoot\bootSound.{mvm.GameConfiguration.extension}", mvm.GameConfiguration.bootsound);
                sound.Text = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "bin", "cfgBoot", $"bootSound.{mvm.GameConfiguration.extension}");
            }
            if (mvm.GameConfiguration.disgamepad)
            {
                mvm.Index = -1;
                gp.IsChecked = true;
            }
            else
            {
                mvm.Index = 1;
                gp.IsChecked = false;
            }
            cd = mvm.GameConfiguration.fourbythree;
            mvm.cd = cd;
            fbt.IsChecked = cd;
            if (mvm.GameConfiguration.donttrim)
            {
                trimn.IsChecked = true;
            }
            else
            {
                trimn.IsChecked = false;
            }
        }
        private bool CheckIfNull(string s)
        {
            if (s == null || s.Equals(string.Empty))
            {
                return true;
            }
            return false;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {

            cd = !cd;
            mvm.cd = cd;
        }

        private void Gn_KeyUp(object sender, KeyEventArgs e)
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

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            if (mvm.Index == -1)
            {
                mvm.Index = 1;
            }
            else
            {
                mvm.Index = -1;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = mvm.GetFilePath(true, false);


            if (!CheckIfNull(path))
            {
                mvm.GC2Rom = path;
            }
        }
        public void Reset()
        {
            gc2.Text = "";
            tv.Text = "";
            drc.Text = "";
            gn.Text = "";
            ic.Text = "";
            log.Text = "";
        }
        private void IcoIMG_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { }

        private void TvIMG_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TDRSHOW t = new TDRSHOW(tv.Text, false);
            t.ShowDialog();
        }

        private void DrcIMG_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TDRSHOW t = new TDRSHOW(drc.Text, true);
            t.ShowDialog();

        }

        private void LogIMG_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LOGSHOW t = new LOGSHOW(log.Text);
            t.ShowDialog();
        }

        private void Ic_TextChanged(object sender, TextChangedEventArgs e)
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

        private void Drc_TextChanged(object sender, TextChangedEventArgs e)
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

        private void Tv_TextChanged(object sender, TextChangedEventArgs e)
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

        private void Log_TextChanged(object sender, TextChangedEventArgs e)
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string path = mvm.GetFilePath(true, true);
            if (!CheckIfNull(path))
            {
                if (new FileInfo(path).Extension.Contains("wav"))
                {
                    if (!mvm.ConfirmRiffWave(path))
                    {
                        Custom_Message cm = new Custom_Message("Incompatible WAV file", "Your WAV file needs to be a RIFF WAVE file which is 16 bit stereo and also 48000khz");
                        cm.ShowDialog();
                    }
                }
            }
            else if(path == "")
            {
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            /*
            try
            {

                TitleKeys webbrowser = new TitleKeys("gcn", "UWUVCI AIO - GCN Help");
                try
                {
                    webbrowser.Owner = mvm.mw;
                }
                catch (Exception)
                {

                }
                mvm.mw.Hide();
                webbrowser.ShowDialog();
                
            }
            catch (Exception)
            {
                Custom_Message cm = new Custom_Message("Not Implemented", "The Helppage for GCN is not implemented yet");
                try
                {
                    cm.Owner = mvm.mw;
                }
                catch (Exception)
                {

                }
                cm.Show();
            }
            */
            // 
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                TitleKeys webbrowser = new TitleKeys("gcn", "GameCube Inject Guide");
                webbrowser.Show();
            }
            catch (Exception)
            {
                Custom_Message cm = new Custom_Message("Not Implemented", "The Helppage for GameCube is not implemented yet");
                cm.Show();
            }
        }

        private void Trimn_Click(object sender, RoutedEventArgs e) => mvm.donttrim = trimn.IsChecked ?? false;

        private void Gn_TextChanged(object sender, TextChangedEventArgs e) => mvm.GameConfiguration.GameName = gn?.Text;
    }
}
