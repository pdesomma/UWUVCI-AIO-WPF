
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using UWUVCI_AIO_WPF.Classes;
using UWUVCI_AIO_WPF.Properties;
using UWUVCI_AIO_WPF.UI.Frames.InjectFrames.Configurations;
using UWUVCI_AIO_WPF.UI.Windows;
using WiiUInjector;
using WiiUInjector.Configs;
using WiiUInjector.Services;
using WiiUInjector.Messaging;
using WiiUInjector.ViewModels;
using WiiUInjector.ViewModels.Configs;
using WiiUInjector.ViewModels.Notifications;
using WiiUInjector.ViewModels.Services;

namespace UWUVCI_AIO_WPF.ViewModels
{
    public class MainViewModel : ViewModel
    { 
        private BackgroundViewModel _backgroundViewModel;
        private BasesViewModel _basesViewModel;
        private IConfigViewModel _configViewModel;
        private InjectionViewModel _injectionViewModel;
        private readonly IMetadataService _metadataService;
        private readonly IBaseRomService _baseRomService;
        private readonly ICommonKeyService _commonKeyService;
        private readonly IDialogService _fileDialogService;
        private readonly IDialogService _directoryDialogService;
        private readonly IDialogService _showImagePreviewDialogService;
        private readonly IEnvironmentService _environmentService;
        private readonly IInjectionService _injectionService;

        public bool AllowSpaceBypass => _environmentService.AllowSpaceBypass;
        public bool DebugMode => _environmentService.Debug;
        public ICommonKeyViewModel CommonKeyViewModel { get; private set; }
        public ConsolesViewModel ConsolesViewModel { get; private set; }
        public NavigationViewModel NavigationViewModel { get; private set; }
        public DialogViewModel ToadViewModel { get; private set; }

        public IConfigViewModel Config
        {
            get => _configViewModel;
            set => SetProperty(ref _configViewModel, value);
        }

        public BackgroundViewModel BackgroundTaskViewModel
        {
            get => _backgroundViewModel;
            set => SetProperty(ref _backgroundViewModel, value);
        }

        public BasesViewModel GameBases
        {
            get => _basesViewModel;
            set => SetProperty(ref _basesViewModel, value);
        }

        public InjectionViewModel InjectionViewModel
        {
            get => _injectionViewModel;
            set => SetProperty(ref _injectionViewModel, value);
        }

        public string GreetingText { get; private set; }

        public bool addi = false;
        public bool cd = false;
        public bool choosefolder = false;
        public System.Windows.Controls.ListViewItem curr = null;
        public string doing = "";
        public bool donttrim = false;
        public bool failed = false;
        public string foldername = "";
        public bool GC = false;
        public int Index = -1;
        public bool jppatch = false;
        public bool LR = false;
        public bool passtrough = true;
        public bool Patch = false;
        public string ProdCode = "";
        public bool regionfrii = false;
        public bool regionfriijp = false;
        public bool regionfriius = false;
        public GameConfig saveconf = null;
        public bool SaveWorkAround = false;
        public GameConsole test;
        public bool toPal = false;
        internal bool backupenableWii = true;
        internal bool enableWii = true;
        private string _baseStore;
        private bool _canInject = false;
        private string _cBasePath;
        private GameConfig _gameConfig = new GameConfig();
        private string _gc2Rom = "";
        private bool _injected = false;
        private bool _injected2 = false;
        private string _injectStore;
        private List<BaseRom> _lBases = new List<BaseRom>();
        private List<string> _lGameBasesString = new List<string>();
        private string _msg;
        private bool _pathsSet = false;
        private string _romPath;

        public MainViewModel(
            IEnvironmentService environmentService,
            INavigationService navigationService,
            ICommonKeyService commonKeyService,
            IBaseRomService baseRomService,
            IMetadataService metadataService,
            IInjectionService injectionService,
            IDialogService showimagePreviewDialogService,
            IDialogService toadDialogService,
            IDialogService enterCommonKeyDialogService,
            IDialogService fileDialogService,
            IDialogService directoryDialogService,
            ExceptionViewModel exceptionViewModel) : base(exceptionViewModel)
        {
            _baseRomService = baseRomService;
            _environmentService = environmentService;
            _commonKeyService = commonKeyService;
            _fileDialogService = fileDialogService;
            _directoryDialogService = directoryDialogService;
            _injectionService = injectionService;
            _showImagePreviewDialogService = showimagePreviewDialogService;
            _metadataService = metadataService;


            NavigationViewModel = new NavigationViewModel(navigationService, ExceptionViewModel);
            BackgroundTaskViewModel = new BackgroundViewModel(ExceptionViewModel);
            CommonKeyViewModel = new CommonKeyViewModel(_commonKeyService, enterCommonKeyDialogService, ExceptionViewModel);
            ConsolesViewModel = new ConsolesViewModel(NavigationViewModel, ExceptionViewModel);
            ToadViewModel = new ToadViewModel(toadDialogService, ExceptionViewModel);

            GreetingText = CommonKeyViewModel.CommonKey is null ? CannedText.Greeting : CannedText.WelcomeBack;

            // listen for selections
            Messenger.Register<BaseSelectedNotification>((n) => InjectionViewModel = new InjectionViewModel(Config.Config, n.Selection.BaseRom, _injectionService, CommonKeyViewModel, BackgroundTaskViewModel, ExceptionViewModel));
            Messenger.Register<ConsoleSelectedNotification>((n) => HandleConsoleSelectedNotification(n.Selection));
        }

        /// <summary>
        /// User chose a console, prep the view models.
        /// </summary>
        /// <param name="console"></param>
        private void HandleConsoleSelectedNotification(GameConsole console)
        {
            _ = Task.Run(() =>
            {
                GameBases = new BasesViewModel(console, _baseRomService, CommonKeyViewModel, BackgroundTaskViewModel, ExceptionViewModel);                
            });
            _ = Task.Run(() =>
            {
                switch (console)
                {
                    case GameConsole.N64:
                        Config = new N64ConfigViewModel(new N64Config(), _metadataService, _fileDialogService, _fileDialogService, _showImagePreviewDialogService, ExceptionViewModel);
                        break;
                    case GameConsole.MSX:
                        Config = new MsxConfigViewModel(new MsxConfig(), _metadataService, _fileDialogService, _showImagePreviewDialogService, ExceptionViewModel);
                        break;
                    case GameConsole.TG16:
                        Config = new Tg16ConfigViewModel(new Tg16Config(), _metadataService, _fileDialogService, _directoryDialogService, _showImagePreviewDialogService, ExceptionViewModel);
                        break;
                    case GameConsole.NDS:
                        Config = new NdsConfigViewModel(new NdsConfig(), _metadataService, _fileDialogService, _showImagePreviewDialogService, ExceptionViewModel);
                        break;
                    case GameConsole.GBA:
                        Config = new GbaConfigViewModel(new GbaConfig(), _metadataService, _fileDialogService, _showImagePreviewDialogService, ExceptionViewModel);
                        break;
                    case GameConsole.SNES:
                        Config = new SnesConfigViewModel(new SnesConfig(), _metadataService, _fileDialogService, _showImagePreviewDialogService, ExceptionViewModel);
                        break;
                    case GameConsole.NES:
                        Config = new NesConfigViewModel(new NesConfig(), _metadataService, _fileDialogService, _showImagePreviewDialogService, ExceptionViewModel);
                        break;
                    default:
                        Config = null;
                        break;
                }
            });
        }

        public string BaseStore
        {
            get => _baseStore;
            set => SetProperty(ref _baseStore, value);
        }
        
        public bool CanInject
        {
            get => _canInject;
            set => SetProperty(ref _canInject, value);
        }

        public string CBasePath
        {
            get => _cBasePath;
            set => SetProperty(ref _cBasePath, value);
        }

        public GameConfig GameConfiguration
        {
            get => _gameConfig;
            set => SetProperty(ref _gameConfig, value);
        }

        public BaseRom GbTemp { get; set; }

        public string GC2Rom
        {
            get => _gc2Rom;
            set => SetProperty(ref _gc2Rom, value);
        }

        public bool Injected
        {
            get => _injected;
            set => SetProperty(ref _injected, value);
        }

        public bool Injected2
        {
            get => _injected2;
            set => SetProperty(ref _injected2, value);
        }

        public string InjectStore
        {
            get => _injectStore;
            set => SetProperty(ref _injectStore, value);
        }

        public List<BaseRom> LBases
        {
            get => _lBases;
            set => SetProperty(ref _lBases, value);
        }

        public List<string> LGameBasesString
        {
            get => _lGameBasesString;
            set => SetProperty(ref _lGameBasesString, value);
        }

        public string Msg
        {
            get => _msg;
            set => SetProperty(ref _msg, value);
        }

        public bool NKITFLAG { get; set; } = false;
        public int OldIndex { get; set; }

        public bool PathsSet
        {
            get => _pathsSet;
            set => SetProperty(ref _pathsSet, value);
        }

        public string RomPath
        {
            get => _romPath;
            set => SetProperty(ref _romPath, value);
        }

        public bool RomSet { get; set; }
        public string SelectedBaseAsString { get; set; }
        public Page Thing { get; set; }

        public static int GetDeterministicHashCode(string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }
                return hash1 + (hash2 * 1566083941);
            }
        }

        public bool CBaseConvertInfo()
        {
            bool ret = false;
            Custom_Message cm = new Custom_Message("NUS Custom Base", " You seem to have added a NUS format Custom Base. \n Do you want it to be converted to be used with the Injector?");
            cm.ShowDialog();

            if (choosefolder)
            {
                ret = true;
                choosefolder = false;
            }
            return ret;
        }

        public bool CheckSysKey(string key)
        {
            if (key.GetHashCode() == -589797700)
            {
                Settings.Default.SysKey = key;
                Settings.Default.Save();
                return true;
            }
            return false;
        }

        public bool CheckSysKey1(string key)
        {
            if (key.GetHashCode() == -1230232583)
            {
                Settings.Default.SysKey1 = key;
                Settings.Default.Save();
                return true;
            }
            return false;
        }

        public bool ConfirmRiffWave(string path)
        {
            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                reader.BaseStream.Position = 0x00;
                long WAVHeader1 = reader.ReadInt32();
                reader.BaseStream.Position = 0x08;
                long WAVHeader2 = reader.ReadInt32();
                return WAVHeader1 == 1179011410 & WAVHeader2 == 1163280727;
            }
        }

        public bool DirectoryIsEmpty(string path)
        {
            var entries = Directory.EnumerateFileSystemEntries(path);
            using (var enumerator = entries.GetEnumerator())
            {
                return !enumerator.MoveNext();
            }
        }

        public async Task DownloadAsync()
        {
            if (await CheckForInternetConnectionAsync())
            {
                DownloadWait dw;

                if (GameConfiguration.Console == GameConsole.WII || GameConfiguration.Console == GameConsole.GCN)
                {
                    // Start the actual download
                    _ = Task.Run(() => { Injection.Download(this); });

                    // Display the waiting dialog with the estimated time
                    dw = new DownloadWait("Downloading Base - Please Wait", this);
                }
                else
                {
                    _ = Task.Run(() => { Injection.Download(this); });
                    dw = new DownloadWait("Downloading Base - Please Wait", this);
                }
                dw.ShowDialog();
            }
        }

        public void ExportFile()
        {
            GameConfiguration.lr = LR;
            GameConfiguration.tgcd = cd;
            GameConfiguration.donttrim = donttrim;
            GameConfiguration.motepass = passtrough;
            GameConfiguration.jppatch = jppatch;
            GameConfiguration.vm = Patch;
            GameConfiguration.vmtopal = toPal;
            GameConfiguration.rf = regionfrii;
            GameConfiguration.rfjp = regionfriijp;
            GameConfiguration.rfus = regionfriius;
            GameConfiguration.disgamepad = Index == -1;
            GameConfiguration.fourbythree = cd;
            if (GameConfiguration.Console == GameConsole.N64)
            {
                //ReadIniIntoConfig();
            }
            GameConfig backup = GameConfiguration;
            if (test == GameConsole.GCN) backup.Console = GameConsole.GCN;
            if (GameConfiguration.GameName == "" || GameConfiguration.GameName == null) backup.GameName = "NoName";
            GameConfiguration.Index = Index;
            CheckAndFixConfigFolder();
            var sanitizedGameName = backup.GameName;
            Array.ForEach(Path.GetInvalidFileNameChars(),
                  c => sanitizedGameName = sanitizedGameName.Replace(c.ToString(), string.Empty));
            string outputPath = $@"configs\[{backup.Console}]{sanitizedGameName}.uwuvci";
            int i = 1;
            while (File.Exists(outputPath))
            {
                outputPath = $@"configs\[{backup.Console}]{sanitizedGameName}_{i}.uwuvci";
                i++;
            }

            Stream createConfigStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            GZipStream compressedStream = new GZipStream(createConfigStream, CompressionMode.Compress);
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(compressedStream, backup);
            compressedStream.Close();
            createConfigStream.Close();
            Custom_Message cm = new Custom_Message("Export success", " The Config was successfully exported.\n Click the Open Folder Button to open the Location where the Config is stored. ", Path.Combine(Directory.GetCurrentDirectory(), outputPath));
            cm.ShowDialog();
        }

        public string GetFilePath(bool ROM, bool INI)
        {
            Custom_Message cm;
            string ret = string.Empty;
            if (ROM && !INI)
            {
                switch (GameConfiguration.Console)
                {
                    case GameConsole.NDS:
                        cm = new Custom_Message("Information", " You can only inject NDS ROMs that are not DSi Enhanced (example for not working: Pokémon Black & White) \n\n If attempting to inject a DSi Enhanced ROM, we will not give you any support with fixing said injection. ");
                        if (!Properties.Settings.Default.ndsw)
                        {
                            cm.ShowDialog();
                        }

                        break;

                    case GameConsole.SNES:
                        cm = new Custom_Message("Information", " You can only inject SNES ROMs that are not using any Co-Processors (example for not working: Star Fox) \n\n If attempting to inject a ROM in need of a Co-Processor, we will not give you any support with fixing said injection. ");
                        if (!Properties.Settings.Default.snesw)
                        {
                            cm.ShowDialog();
                        }
                        break;
                }
            }

            using (var dialog = new OpenFileDialog())
            {
                if (ROM)
                {
                    if (INI)
                    {
                        dialog.Filter = "BootSound Files (*.mp3; *.wav; *.btsnd) | *.mp3;*.wav;*.btsnd";
                    }
                    else if (GC)
                    {
                        dialog.Filter = "GCN ROM (*.iso; *.gcm) | *.iso; *.gcm";
                    }
                    else
                    {
                        switch (GameConfiguration.Console)
                        {
                            case GameConsole.N64:
                                dialog.Filter = "Nintendo 64 ROM (*.n64; *.v64; *.z64) | *.n64;*.v64;*.z64";
                                break;
                            case GameConsole.MSX:
                                dialog.Filter = "MSX/MSX2 ROM (*.ROM) | *.ROM";
                                break;
                            case GameConsole.WII:
                                if (test == GameConsole.GCN)
                                {
                                    dialog.Filter = "GC ROM (*.iso; *.gcm; *.nkit.iso; *.nkit.gcz) | *.iso; *.gcm; *.nkit.iso; *.nkit.gcz";
                                }
                                else
                                {
                                    dialog.Filter = "All Supported Types (*.*) | *.iso; *.wbfs; *.nkit.gcz; *.nkit.iso; *.dol; *.wad|Wii ROM (*.iso; *.wbfs; *.nkit.gcz; *.nkit.iso) | *.iso; *.wbfs; *.nkit.gcz; *.nkit.iso|Wii Homebrew (*.dol) | *.dol|Wii Channel (*.wad) | *.wad";
                                }
                                break;
                            case GameConsole.GCN:
                                dialog.Filter = "GC ROM (*.iso; *.gcm; *.nkit.iso; *.nkit.gcz) | *.iso; *.gcm; *.nkit.iso; *.nkit.gcz";
                                break;
                        }
                    }
                }
                else if (!INI)
                {
                    dialog.Filter = "Images (*.png; *.jpg; *.bmp; *.tga; *jpeg) | *.png;*.jpg;*.bmp;*.tga;*jpeg";
                }
                else if (INI)
                {
                    dialog.Filter = "N64 VC Configuration (*.ini) | *.ini";
                }
                if (Directory.Exists("SourceFiles"))
                {
                    dialog.InitialDirectory = "SourceFiles";
                }

                DialogResult res = dialog.ShowDialog();
                if (res == DialogResult.OK)
                {
                    if (dialog.FileName.ToLower().Contains(".gcz"))
                    {
                        Custom_Message cm1 = new Custom_Message("Information", " Using a GameCube GCZ Nkit for a Wii Inject or vice versa will break things. \n You will not be able to grab the BootImages or GameName using this type of ROM. ");
                        if (!Properties.Settings.Default.gczw)
                        {
                            cm1.ShowDialog();
                        }
                    }
                    ret = dialog.FileName;
                }
                else
                {
                    if (dialog.Filter.Contains("BootImages") || dialog.Filter.Contains("BootSound"))
                    {
                        ret = "";
                    }
                }
            }
            return ret;
        }

        public async Task<string> GetInternalWIIGCNName(string OpenGame, bool gc)
        {
            string ret = "";
            try
            {
                using (var reader = new BinaryReader(File.OpenRead(OpenGame)))
                {
                    string TempString = "";
                    string SystemType = "wii/";
                    if (gc)
                    {
                        SystemType = "gcn/";
                    }
                    var repoid = "";
                    reader.BaseStream.Position = 0x00;
                    char TempChar;
                    //WBFS Check
                    List<string> repoids = new List<string>();
                    if (new FileInfo(OpenGame).Extension.Contains("wbfs")) //Performs actions if the header indicates a WBFS file
                    {
                        reader.BaseStream.Position = 0x200;

                        reader.BaseStream.Position = 0x218;

                        reader.BaseStream.Position = 0x220;
                        while ((TempChar = reader.ReadChar()) != 0) ret += TempChar;
                        reader.BaseStream.Position = 0x200;
                        while ((TempChar = reader.ReadChar()) != 0) TempString += TempChar;
                        repoid = TempString;
                    }
                    else
                    {
                        reader.BaseStream.Position = 0x18;

                        reader.BaseStream.Position = 0x20;
                        while ((TempChar = reader.ReadChar()) != 0) ret += TempChar;
                        reader.BaseStream.Position = 0x00;
                        while ((TempChar = reader.ReadChar()) != 0) TempString += TempChar;
                        repoid = TempString;
                    }

                    if (await Util.IsConnectedToInternetAsync())
                    {
                        repoids.Add(SystemType + repoid);
                        repoids.Add(SystemType + repoid.Substring(0, 3) + "E" + repoid.Substring(4, 2));
                        repoids.Add(SystemType + repoid.Substring(0, 3) + "P" + repoid.Substring(4, 2));
                        repoids.Add(SystemType + repoid.Substring(0, 3) + "J" + repoid.Substring(4, 2));
                    }
                }
            }
            catch (Exception)
            {
                Custom_Message cm = new Custom_Message("Unknown ROM", " It seems that you inserted an unknown ROM as a Wii or GameCube game. \n It is not recommended continuing with said ROM!");
                cm.ShowDialog();
            }

            return ret;
        }

        public void ImageWarning()
        {
            Custom_Message cm = new Custom_Message("Image Warning", " Images need to either be in a Bit Depth of 32bit or 24bit. \n If using Tools like paint.net do not choose the Auto function.");
            cm.ShowDialog();
        }

        public void ImportConfig(string configPath)
        {
            FileInfo fn = new FileInfo(configPath);
            if (Directory.Exists(@"bin\cfgBoot"))
            {
                Directory.Delete(@"bin\cfgBoot", true);
            }
            if (fn.Extension.Contains("uwuvci"))
            {
                FileStream inputConfigStream = new FileStream(configPath, FileMode.Open, FileAccess.Read);
                GZipStream decompressedConfigStream = new GZipStream(inputConfigStream, CompressionMode.Decompress);
                IFormatter formatter = new BinaryFormatter();
                GameConfiguration = (GameConfig)formatter.Deserialize(decompressedConfigStream);
                LR = GameConfiguration.lr;
                cd = GameConfiguration.tgcd;
                passtrough = GameConfiguration.motepass;
                jppatch = GameConfiguration.jppatch;
                Patch = GameConfiguration.vm;
                toPal = GameConfiguration.vmtopal;
                regionfrii = GameConfiguration.rf;
                regionfriijp = GameConfiguration.rfjp;
                regionfriius = GameConfiguration.rfus;
            }
            if (_gameConfig.Console == GameConsole.WII && test != GameConsole.GCN)
            {
                (Thing as WiiConfig).GetInfoFromConfig();
            }
            else if (test == GameConsole.GCN)
            {
                (Thing as GCConfig).getInfoFromConfig();
            }
        }

        public void IsIsoNkit()
        {
            using (var fs = new FileStream(RomPath, FileMode.Open, FileAccess.Read))
            {
                byte[] procode = new byte[4];
                fs.Seek(0x200, SeekOrigin.Begin);
                fs.Read(procode, 0, 4);
                var s = ByteArrayToString(procode);

                fs.Close();
                if (s.ToLower().Contains("nkit"))
                {
                    NKITFLAG = true;
                }
                else
                {
                    NKITFLAG = false;
                }
            }
        }

        public void OpenDialog(string title, string msg)
        {
            Custom_Message cm = new Custom_Message(title, msg);
            cm.ShowDialog();
        }

        public void Pack(bool loadiine)
        {
            if (loadiine)
            {
                Injection.Loadiine(GameConfiguration.GameName);
            }
            else
            {
                if (_gameConfig.GameName != null)
                {
                    Regex reg = new Regex("[^a-zA-Z0-9 é -]");
                    _gameConfig.GameName = _gameConfig.GameName.Replace("|", " ");
                    _gameConfig.GameName = reg.Replace(_gameConfig.GameName, "");
                }

                Task.Run(() => { Injection.Packing(GameConfiguration.GameName, this); });

                DownloadWait dw = new DownloadWait("Packing Inject - Please Wait", this);
                dw.ShowDialog();

                string extra = "";
                string names = "Copy to SD";
                if (GameConfiguration.Console == GameConsole.WII) extra = "\n Some games cannot reboot into the WiiU Menu. Shut down via the GamePad. \n If Stuck in a BlackScreen, you need to unplug your WiiU.";
                if (GameConfiguration.Console == GameConsole.WII && _romPath.ToLower().Contains(".wad")) extra += "\n Make sure that the chosen WAD is installed in your vWii!";
                if (GC)
                {
                    extra = "\n Make sure to have Nintendon't + config on your SD.\n You can add them by pressing the \"SD Setup\" button or using the \"Start Nintendont Config Tool\" button under Settings. ";
                    names = "SD Setup";
                }
                GC2Rom = "";

                Custom_Message cm = new Custom_Message("Injection Complete", $" You need CFW (example: haxchi or mocha) to run and install this inject! \n It's recommended to install onto USB to avoid brick risks.{extra}\n To Open the Location of the Inject press Open Folder.\n If you want the inject to be put on your SD now, press {names}. ", Settings.Default.OutPath);
                cm.ShowDialog();
            }
            LGameBasesString.Clear();
            CanInject = false;
            RomSet = false;
            RomPath = null;
            Injected = false;
            GameConfiguration.CBasePath = null;
            GC = false;
            NKITFLAG = false;
            CBasePath = null;
            ProdCode = "";
            foldername = "";
            //mw.ListView_Click(mw.listCONS, null);
        }

        public string ReadCkeyFromOtp()
        {
            string ret = "";
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "OTP.bin | otp.bin";
                DialogResult res = dialog.ShowDialog();
                if (res == DialogResult.OK)
                {
                    var filepath = dialog.FileName;
                    using (var fs = new FileStream(filepath,
                                 FileMode.Open,
                                 FileAccess.Read))
                    {
                        byte[] test = new byte[16];
                        fs.Seek(0xE0, SeekOrigin.Begin);

                        fs.Read(test, 0, 16);
                        fs.Close();
                        foreach (var b in test)
                        {
                            ret += string.Format("{0:X2}", b);
                        }
                    }
                }
            }
            return ret;
        }

        public void ResetTitleKeys()
        {
            File.Delete("bin/keys/gba.vck");
            File.Delete("bin/keys/nds.vck");
            File.Delete("bin/keys/nes.vck");
            File.Delete("bin/keys/n64.vck");
            File.Delete("bin/keys/msx.vck");
            File.Delete("bin/keys/tg16.vck");
            File.Delete("bin/keys/snes.vck");
            File.Delete("bin/keys/wii.vck");
            Custom_Message cm = new Custom_Message("Reset Successful", " The TitleKeys are now reset. \n The Program will now restart.");
            cm.ShowDialog();
            Process p = new Process();
            p.StartInfo.FileName = System.Windows.Application.ResourceAssembly.Location;

            p.StartInfo.Arguments = "--skip" + (DebugMode ? " --skip" : "");
            p.Start();
            Environment.Exit(0);
        }

        public void ResetTKQuest()
        {
            Custom_Message cm = new Custom_Message("Resetting TitleKeys", " This Option will reset all entered TitleKeys meaning you will need to reenter them again! \n Do you still wish to continue?");
            cm.ShowDialog();
            cm.Close();
        }

        public void RestartIntoBypass()
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = System.Windows.Application.ResourceAssembly.Location;
                if (DebugMode)
                {
                    if (SaveWorkAround)
                    {
                        p.StartInfo.Arguments = "--debug --skip --spacebypass";
                    }
                    else
                    {
                        p.StartInfo.Arguments = "--debug --skip";
                    }
                }
                else
                {
                    if (SaveWorkAround)
                    {
                        p.StartInfo.Arguments = "--skip --spacebypass";
                    }
                    else
                    {
                        p.StartInfo.Arguments = "--skip";
                    }
                }
                p.Start();
                Environment.Exit(0);
            }
        }

        public void SetThing(Page T)
        {
            Thing = T;
        }

        public bool SysKey1set()
        {
            return CheckSysKey1(Properties.Settings.Default.SysKey1);
        }

        public bool SysKeyset()
        {
            return CheckSysKey(Properties.Settings.Default.SysKey);
        }

        public void UpdatePathSet()
        {
            PathsSet = Settings.Default.PathsSet;
            if (BaseStore != Settings.Default.BasePath)
            {
                BaseStore = Settings.Default.BasePath;
            }
            if (InjectStore != Settings.Default.BasePath)
            {
                InjectStore = Settings.Default.OutPath;
            }
        }

        public async Task UpdateToolsAsync()
        {
            if (await CheckForInternetConnectionAsync())
            {
                string[] bases = ToolCheck.ToolNames;
                _ = Task.Run(() =>
                {
                    double l = 100 / bases.Length;
                    foreach (string s in bases)
                    {
                        //DeleteTool(s);
                        // DownloadTool(s, this);
                    }
                });

                DownloadWait dw = new DownloadWait("Updating Tools - Please Wait", this);
                dw.ShowDialog();
                //await toolCheckAsync();
                Custom_Message cm = new Custom_Message("Finished Update", " Finished Updating Tools! Restarting UWUVCI AIO ");
                cm.ShowDialog();
                Process p = new Process();
                p.StartInfo.FileName = System.Windows.Application.ResourceAssembly.Location;
                if (DebugMode)
                {
                    if (SaveWorkAround)
                    {
                        p.StartInfo.Arguments = "--debug --skip --spacebypass";
                    }
                    else
                    {
                        p.StartInfo.Arguments = "--debug --skip";
                    }
                }
                else
                {
                    if (SaveWorkAround)
                    {
                        p.StartInfo.Arguments = "--skip --spacebypass";
                    }
                    else
                    {
                        p.StartInfo.Arguments = "--skip";
                    }
                }
                p.Start();
                Environment.Exit(0);
            }
        }

        private static void CheckAndFixConfigFolder()
        {
            if (!Directory.Exists(@"configs"))
            {
                Directory.CreateDirectory(@"configs");
            }
        }
        
        private string ByteArrayToString(byte[] arr)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(arr);
        }

        private async Task<bool> CheckForInternetConnectionAsync()
        {
            if (!await Util.IsConnectedToInternetAsync())
            {
                Custom_Message cm = new Custom_Message("No Internet Connection", "To Download Tools, Bases or required Files you need to be connected to the Internet. The Program will now terminate. ");
                cm.ShowDialog();
                Environment.Exit(1);
                return false;
            }
            return true;
        }
    }
}