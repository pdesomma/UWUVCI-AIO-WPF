using System;
using System.Windows;
using System.Windows.Threading;
using UWUVCI_AIO_WPF.ViewModels;

namespace UWUVCI_AIO_WPF.UI.Windows
{
    /// <summary>
    /// Interaktionslogik für DownloadWait.xaml
    /// </summary>

    partial class DownloadWait : Window
    {
        readonly MainViewModel mvm;
        readonly DispatcherTimer timer = new DispatcherTimer();

        public DownloadWait(string doing, MainViewModel mvm)
        {
            try
            {
                if (Owner?.GetType() == typeof(MainWindow))
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
            }
            catch (Exception)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            this.mvm = mvm;
            InitializeComponent();
            Key.Text = doing;

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Window_Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            msgT.Text = mvm.Msg;
        }

        public void ChangeOwner(MainWindow ow)
        {
            Owner = ow;
            try
            {
                if (Owner?.GetType() == typeof(MainWindow))
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    ShowInTaskbar = false;
                }
            }
            catch (Exception)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }
    }
}
