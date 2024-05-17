using System;
using System.Windows;
using System.Windows.Controls;
using UWUVCI_AIO_WPF.ViewModels;
using WiiUInjector;

namespace UWUVCI_AIO_WPF.UI.Frames
{
    public partial class INJECTFRAME : Page
    {
        readonly MainViewModel mvm;
        public INJECTFRAME(GameConsole console)
        {
            mvm = DataContext as MainViewModel;
            mvm.GameConfiguration.Console = console;

            InitializeComponent();
            if (console == GameConsole.WII)
            {
                fLoadConfig.Content = new InjectFrames.Configurations.WiiConfig();

            }
            else if (console == GameConsole.GCN)
            {
                fLoadConfig.Content = new InjectFrames.Configurations.GCConfig();
            }
            mvm.Injected2 = false;
            Console.WriteLine("GameConfig : " + mvm.GameConfiguration.Console.ToString());
        }
        public INJECTFRAME(GameConsole console, GameConfig c)
        {
            mvm = DataContext as MainViewModel;
            mvm.GameConfiguration.Console = console;

            InitializeComponent();
            if (console == GameConsole.WII)
            {
                fLoadConfig.Content = new InjectFrames.Configurations.WiiConfig(c);
            }
            else if (console == GameConsole.GCN)
            {
                fLoadConfig.Content = new InjectFrames.Configurations.GCConfig(c);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Export config
            mvm.ExportFile();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            mvm.Pack(true);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            mvm.Pack(false);
        }
    }
}
