using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using UWUVCI_AIO_WPF.UI.Windows;
using UWUVCI_AIO_WPF.ViewModels;
using WiiUInjector.GitTools.Services;
using WiiUInjector.Repos;
using WiiUInjector.ViewModels;

namespace UWUVCI_AIO_WPF
{
    public partial class App : Application
    {
        private static readonly IEnvironmentService s_environmentService = new EnvironmentService("skip", "debug", "spacebypass");
        public static MainViewModel MainViewModel { get; private set; }

        readonly Timer t = new Timer(5000);
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Process[] pname = Process.GetProcessesByName("UWUVCI AIO");
            if (pname.Length > 1 && !s_environmentService.SkipInstanceCheck)
            {
                t.Elapsed += KillProg;
                t.Start();
                Custom_Message cm = new Custom_Message("Another Instance Running", " You already have an instance of UWUVCI AIO running. \n This instance will terminate in 5 seconds. ");
                cm.ShowDialog();
                KillProg(null, null);
            }
            else
            {
                MainViewModel = new MainViewModel(
                    s_environmentService,
                    new PageNavigationService(),
                    new CommonKeyService(),
                    new BaseRomService(new BaseRomRepo("https://github.com/Hotbrawl20/UWUVCI-VCB/raw/master/", "bases.")),
                    new MetadataService(),
                    new InjectionService(),
                    new PreviewImageDialogService(),
                    new ToadDialogService(),
                    new CommonKeyDialogService(),
                    new FileDialogService(),
                    new DirectoryDialogService(),
                    new ExceptionViewModel(new ExceptionDialogService()));
                MainWindow wnd = new MainWindow();

                double height = SystemParameters.PrimaryScreenHeight;
                double width = SystemParameters.PrimaryScreenWidth;

                if (width < 1150 || height < 700)
                {
                    t.Elapsed += KillProg;
                    t.Start();
                    Custom_Message cm = new Custom_Message("Resolution not supported", "Your screen resolution is not supported, please use a resolution of atleast 1152x864\nThis instance will terminate in 5 seconds.");
                    cm.ShowDialog();
                    KillProg(null, null);
                }

                if (!Environment.Is64BitOperatingSystem)
                {
                    //wnd.is32();
                    Custom_Message cm = new Custom_Message("Warning", "Some features may cause issues on a 32Bit System. Upgrading to 64Bit would be recommended.\nDue to an Issue with packing on 32Bit Systems, you need Java installed for packing. \nReport any issues in the UWUVCI Discord, or ping @NicoAICP or @ZestyTS in #wiiu-assistance in the Nintendo Homebrew discord. ");
                    cm.ShowDialog();
                }

                wnd.Show();
            }
        }

        private void KillProg(object sender, ElapsedEventArgs e)
        {
            t?.Stop();
            t?.Dispose();
            Environment.Exit(1);
        }
    }
}
