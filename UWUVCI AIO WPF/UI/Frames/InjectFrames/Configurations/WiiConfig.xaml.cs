using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WiiUInjector;
using UWUVCI_AIO_WPF.Properties;
using UWUVCI_AIO_WPF.UI.Windows;
using UWUVCI_AIO_WPF.ViewModels;
using WiiUDownloaderLibrary;

namespace UWUVCI_AIO_WPF.UI.Frames.InjectFrames.Configurations
{
    /// <summary>
    /// Interaktionslogik für WiiConfig.xaml
    /// </summary>
    public partial class WiiConfig : Page, IDisposable
    {
        readonly MainViewModel mvm;
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
        public void Imgpath(string icon, string tv)
        {
            ic.Text = icon;
            this.tv.Text = tv;
        }
        public WiiConfig()
        {
            InitializeComponent();
            mvm = DataContext as MainViewModel;
            mvm.SetThing(this);
            Injection.ToolTip = "Changing the extension of a ROM may result in a faulty inject.\nWe will not give any support in such cases";
            List<string> gpEmu = new List<string>
            {
                "Do not use. WiiMotes only",
                "Classic Controller",
                "Horizontal WiiMote",
                "Vertical WiiMote",
                "Force Classic Controller",
                "Force No Classic Controller"
            };
            gamepad.ItemsSource = gpEmu;
            gamepad.SelectedIndex = 0;
            mvm.test = GameConsole.WII;
            List<string> selection = new List<string>
            {
                "Video Patches",
                "Region Patches",
                "Extras"
            };
            selectionDB.ItemsSource = selection;
            selectionDB.SelectedIndex = 0;
        }
        public WiiConfig(GameConfig c)
        {
            InitializeComponent();
            mvm = DataContext as MainViewModel;
            GetInfoFromConfig();
            mvm.GameConfiguration = c.Clone();
            mvm.SetThing(this);
            Injection.ToolTip = "Changing the extension of a ROM may result in a faulty inject.\nWe will not give any support in such cases";
            List<string> gpEmu = new List<string>
            {
                "Do not use. WiiMotes only",
                "Classic Controller",
                "Horizontal WiiMote",
                "Vertical WiiMote",
                "Force Classic Controller",
                "Force No Classic Controller"
            };
            gamepad.ItemsSource = gpEmu;
            gamepad.SelectedIndex = 0;
            mvm.test = GameConsole.WII;
            List<string> selection = new List<string>
            {
                "Video Patches",
                "Region Patches",
                "Extras"
            };
            selectionDB.ItemsSource = selection;
            selectionDB.SelectedIndex = 0;
        }
        public void Dispose()
        {

        }

        private async void Set_Rom_Path(object sender, RoutedEventArgs e)
        {
            string path = mvm.GetFilePath(true, false);
            ancast_Button.IsEnabled = false;
            ancastKey.Text = "";
            ancastKey.IsEnabled = false;
            if (!CheckIfNull(path))

            {
                int TitleIDInt;
                bool isok = false;
                if (path.ToLower().Contains(".gcz") || path.ToLower().Contains(".dol") || path.ToLower().Contains(".wad"))
                {
                    isok = true;
                }
                else
                {
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
                }


                if (isok)
                {
                    motepass.IsEnabled = false;
                    motepass.IsChecked = false;
                    gamepad.IsEnabled = true;
                    mvm.NKITFLAG = false;
                    trimn.IsEnabled = false;
                    trimn.IsChecked = false;
                    vmcsmoll.IsEnabled = true;
                    pal.IsEnabled = true;
                    ntsc.IsEnabled = true;
                    mvm.donttrim = false;
                    jppatch.IsEnabled = true;
                    motepass.IsEnabled = false;
                    List<string> gpEmu = new List<string>
                    {
                        "Do not use. WiiMotes only",
                        "Classic Controller",
                        "Horizontal WiiMote",
                        "Vertical WiiMote",
                        "Force Classic Controller",
                        "Force No Classic Controller"
                    };
                    gamepad.ItemsSource = gpEmu;
                    gamepad.ItemsSource = gpEmu;
                    mvm.RomPath = path;
                    mvm.RomSet = true;

                    if (!path.ToLower().Contains(".gcz") && !path.ToLower().Contains(".dol") && !path.ToLower().Contains(".wad"))
                    {
                        string rom = await mvm.GetInternalWIIGCNName(mvm.RomPath, false);
                        Regex reg = new Regex("[*'\",_&#^@:;?!<>|µ~#°²³´`éⓇ©™]");
                        gn.Text = reg.Replace(rom, string.Empty);
                        mvm.GameConfiguration.GameName = reg.Replace(rom, string.Empty);

                        if (System.IO.Path.GetExtension(path).ToLower() == "iso")
                        {
                            trimn.IsEnabled = true;
                            mvm.IsIsoNkit();
                        }
                    }
                    else if (path.ToLower().Contains(".dol"))
                    {
                        ancastKey.IsEnabled = true;
                        ancast_Button.IsEnabled = true;
                        mvm.NKITFLAG = false;
                        trimn.IsEnabled = false;
                        trimn.IsChecked = false;
                        vmcsmoll.IsEnabled = false;
                        pal.IsEnabled = false;
                        ntsc.IsEnabled = false;
                        RF_n.IsEnabled = false;
                        RF_tj.IsEnabled = false;
                        RF_tn.IsEnabled = false;
                        RF_tp.IsEnabled = false;
                        jppatch.IsEnabled = false;
                        motepass.IsChecked = false;
                        motepass.IsEnabled = true;
                        mvm.donttrim = false;
                        gamepad.IsEnabled = false;
                        LR.IsEnabled = false;
                    }
                    else if (path.ToLower().Contains(".wad"))
                    {
                        mvm.NKITFLAG = false;
                        trimn.IsEnabled = false;
                        trimn.IsChecked = false;
                        vmcsmoll.IsEnabled = false;
                        pal.IsEnabled = false;
                        ntsc.IsEnabled = false;
                        RF_n.IsEnabled = false;
                        RF_tj.IsEnabled = false;
                        RF_tn.IsEnabled = false;
                        RF_tp.IsEnabled = false;
                        jppatch.IsEnabled = false;
                        mvm.donttrim = false;

                    }
                    else
                    {
                        motepass.IsChecked = false;
                        motepass.IsEnabled = false;

                        trimn.IsEnabled = true;
                    }


                }
                else
                {
                    Custom_Message cm = new Custom_Message("Wrong ROM", "The chosen ROM is not a supported WII Game");
                    cm.ShowDialog();
                }
            }
        }
        public string ReadAncastFromOtp()
        {
            var ret = "";
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "OTP.bin | otp.bin";
                var res = dialog.ShowDialog();
                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    var filepath = dialog.FileName;
                    var test = new byte[16];

                    using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        fs.Seek(0x90, SeekOrigin.Begin);
                        fs.Read(test, 0, 16);
                    }
                    foreach (var b in test)
                        ret += string.Format("{0:X2}", b);
                }
            }
            return ret;
        }

        private void InjectGame(object sender, RoutedEventArgs e)
        {
            mvm.Index = gamepad.SelectedIndex;
            if (LR.IsChecked == true)
            {
                mvm.LR = true;
            }
            else
            {
                mvm.LR = false;
            }
            mvm.GameConfiguration.GameName = gn.Text;

            if (!string.IsNullOrEmpty(ancastKey.Text))
            {
                ancastKey.Text = ancastKey.Text.ToUpper();

                var toolsPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "bin", "Tools");
                var tempPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "bin", "temp");
                var downloadPath = System.IO.Path.Combine(tempPath, "download");
                var c2wPath = System.IO.Path.Combine(tempPath, "C2W");
                var imgFileCode = System.IO.Path.Combine(c2wPath, "code", "c2w.img");
                var imgFile = System.IO.Path.Combine(c2wPath, "c2w.img");
                var c2wFile = System.IO.Path.Combine(c2wPath, "c2w_patcher.exe");

                var sourceData = ancastKey.Text;
                var tempSource = Encoding.ASCII.GetBytes(sourceData);
                var tmpHash = MD5.Create().ComputeHash(tempSource);
                var hash = BitConverter.ToString(tmpHash);

                if (hash == "31-8D-1F-9D-98-FB-08-E7-7C-7F-E1-77-AA-49-05-43")
                {
                    Settings.Default.Ancast = ancastKey.Text;
                    string[] ancastKeyCopy = { ancastKey.Text };

                    Task.Run(() =>
                    {
                        Directory.CreateDirectory(tempPath + "\\C2W");

                        var titleIds = new List<string>()
                        {
                            "0005001010004000",
                            "0005001010004001"
                        };

                        foreach (var titleId in titleIds)
                        {
                            Task.Run(() => Downloader.DownloadAsync(titleId, downloadPath)).GetAwaiter().GetResult();
                        }

                        foreach (var titleId in titleIds)
                        {
                            CSharpDecrypt.CSharpDecrypt.Decrypt(new string[] { Settings.Default.Ckey, System.IO.Path.Combine(downloadPath, titleId), c2wPath });
                        }

                        File.WriteAllLines(c2wPath + "\\starbuck_key.txt", ancastKeyCopy);

                        File.Copy(System.IO.Path.Combine(toolsPath, "c2w_patcher.exe"), c2wFile, true);

                        File.Copy(imgFileCode, imgFile, true);

                        var currentDir = Directory.GetCurrentDirectory();
                        Directory.SetCurrentDirectory(c2wPath);
                        using (Process c2w = new Process())
                        {
                            c2w.StartInfo.FileName = "c2w_patcher.exe";
                            c2w.StartInfo.Arguments = $"-nc";
                            c2w.Start();
                            c2w.WaitForExit();
                        }
                        Directory.SetCurrentDirectory(currentDir);

                        File.Copy(System.IO.Path.Combine(c2wPath, "c2p.img"), imgFileCode, true);
                    }).GetAwaiter();
                }
                else
                {
                    var cm = new Custom_Message("C2W Error", "Ancast code is incorrect.\nNot continuing with inject.");
                    cm.ShowDialog();
                    return;
                }

                var message = new DownloadWait("Setting Up C2W - Please Wait", mvm);
                message.ShowDialog();
                File.Delete(imgFileCode);
                try
                {
                    Directory.Delete(downloadPath, true);
                }
                catch { }
                File.Delete(c2wFile);
                File.Delete(c2wPath + "\\starbuck_key.txt");
                File.Delete(System.IO.Path.Combine(c2wPath, "c2p.img"));
                File.Delete(imgFileCode);
                try
                {
                    Directory.Delete(System.IO.Path.Combine(c2wPath, "code"), true);
                }
                catch { }
            }

            //mvm.Inject(false);
        }

        public void GetInfoFromConfig()
        {
            rp.Text = "";
            mvm.RomPath = "";
            mvm.RomSet = false;
            mvm.GC2Rom = "";
            if (tv.Text.Length > 0)
            {
                tvIMG.Visibility = Visibility.Visible;
            }
            if (ic.Text.Length > 0)
            {
                icoIMG.Visibility = Visibility.Visible;
            }
            if (drc.Text.Length > 0)
            {
                drcIMG.Visibility = Visibility.Visible;
            }
            if (log.Text.Length > 0)
            {
                logIMG.Visibility = Visibility.Visible;
            }
            gn.Text = mvm.GameConfiguration.GameName;
            mvm.Index = mvm.GameConfiguration.Index;
            gamepad.SelectedIndex = mvm.GameConfiguration.Index;
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
            LR.IsChecked = mvm.LR;
            if (mvm.GameConfiguration.donttrim)
            {
                trimn.IsChecked = true;
            }
            else
            {
                trimn.IsChecked = false;
            }
            jppatch.IsChecked = mvm.jppatch;
            motepass.IsChecked = mvm.passtrough;
            if (mvm.Patch)
            {
                if (mvm.toPal)
                {
                    vmcsmoll.IsChecked = false;
                    pal.IsChecked = false;
                    ntsc.IsChecked = true;
                }
                else
                {
                    vmcsmoll.IsChecked = false;
                    ntsc.IsChecked = false;
                    pal.IsChecked = true;
                }
            }
            else
            {
                vmcsmoll.IsChecked = true;
                pal.IsChecked = false;
                ntsc.IsChecked = false;
            }

            if (mvm.regionfrii)
            {
                if (mvm.regionfriijp)
                {
                    RF_n.IsChecked = false;
                    RF_tj.IsChecked = true;
                    RF_tn.IsChecked = false;
                    RF_tp.IsChecked = false;
                }
                else if (mvm.regionfriius)
                {
                    RF_n.IsChecked = false;
                    RF_tj.IsChecked = false;
                    RF_tn.IsChecked = true;
                    RF_tp.IsChecked = false;
                }
                else
                {
                    RF_n.IsChecked = false;
                    RF_tj.IsChecked = false;
                    RF_tn.IsChecked = false;
                    RF_tp.IsChecked = true;
                }
            }
            else
            {
                RF_n.IsChecked = true;
                RF_tj.IsChecked = false;
                RF_tn.IsChecked = false;
                RF_tp.IsChecked = false;
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

        private void Gn_KeyUp_1(object sender, KeyEventArgs e)
        {

        }

        private void Gamepad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mvm.Index = gamepad.SelectedIndex;
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
        public void Reset()
        {
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

        private void Gn_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Up) || Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Left) || Keyboard.IsKeyDown(Key.Right))
            {
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { }

        private void SoundImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                TitleKeys webbrowser = new TitleKeys("wii", "Wii Inject Guide");
                webbrowser.Show();
            }
            catch (Exception)
            {
                Custom_Message cm = new Custom_Message("Not Implemented", "The Helppage for Wii is not implemented yet");
                cm.Show();
            }
        }

        private void Trimn_Click(object sender, RoutedEventArgs e)
        {
            if (!mvm.donttrim)
            {
                mvm.toPal = false;
                mvm.Patch = false;
                vmcsmoll.IsChecked = true;
                vmcsmoll.IsEnabled = false;
                pal.IsEnabled = false;
                ntsc.IsEnabled = false;
                mvm.donttrim = true;
                mvm.jppatch = false;
                int last = gamepad.SelectedIndex;
                List<string> gpEmu = new List<string>
                {
                    "Do not use. WiiMotes only",
                    "Classic Controller",
                    "Horizontal WiiMote",
                    "Vertical WiiMote",
                    "[NEEDS TRIMMING] Force Classic Controller",
                    "Force No Classic Controller"
                };
                gamepad.ItemsSource = gpEmu;
                gamepad.SelectedIndex = last;
                jppatch.IsEnabled = false;
            }
            else
            {
                int last = gamepad.SelectedIndex;
                vmcsmoll.IsEnabled = true;
                pal.IsEnabled = true;
                ntsc.IsEnabled = true;
                mvm.donttrim = false;
                jppatch.IsEnabled = true;
                List<string> gpEmu = new List<string>
                {
                    "Do not use. WiiMotes only",
                    "Classic Controller",
                    "Horizontal WiiMote",
                    "Vertical WiiMote",
                    "Force Classic Controller",
                    "Force No Classic Controller"
                };
                gamepad.ItemsSource = gpEmu;
                gamepad.ItemsSource = gpEmu;
                gamepad.SelectedIndex = last;
            }
        }

        private void Jppatch_Click(object sender, RoutedEventArgs e)
        {
            if (mvm.jppatch)
            {
                mvm.jppatch = false;
            }
            else
            {
                mvm.jppatch = true;
            }
        }

        private void SelectionDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (selectionDB.SelectedIndex)
            {
                case 0:
                    VideoMode.Visibility = Visibility.Visible;
                    RegionFrii.Visibility = Visibility.Hidden;
                    Extra.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    VideoMode.Visibility = Visibility.Hidden;
                    RegionFrii.Visibility = Visibility.Visible;
                    Extra.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    VideoMode.Visibility = Visibility.Hidden;
                    RegionFrii.Visibility = Visibility.Hidden;
                    Extra.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Motepass_Checked(object sender, RoutedEventArgs e) => mvm.passtrough = false;

        private void Motepass_Unchecked(object sender, RoutedEventArgs e) => mvm.passtrough = true;

        private void RF_tp_Click(object sender, RoutedEventArgs e)
        {
            if (RF_tp.IsChecked == true)
            {
                mvm.regionfrii = true;
                mvm.regionfriijp = false;
                mvm.regionfriius = false;
            }
            if (RF_tj.IsChecked == true)
            {
                mvm.regionfrii = true;
                mvm.regionfriijp = true;
                mvm.regionfriius = false;
            }
            if (RF_tn.IsChecked == true)
            {
                mvm.regionfrii = true;
                mvm.regionfriijp = false;
                mvm.regionfriius = true;
            }
            if (RF_n.IsChecked == true)
            {
                mvm.regionfrii = false;
                mvm.regionfriijp = false;
                mvm.regionfriius = false;
            }
        }

        private void Log_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                mvm.GameConfiguration.GameName = gn.Text;
            }
            catch { }
        }
        private void Ancast_OTP(object sender, RoutedEventArgs e)
        {
            ancastKey.Text = ReadAncastFromOtp();
        }
    }
}