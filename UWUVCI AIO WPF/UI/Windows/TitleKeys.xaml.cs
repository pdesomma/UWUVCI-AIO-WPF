using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UWUVCI_AIO_WPF.ViewModels;

namespace UWUVCI_AIO_WPF.UI.Windows
{
    public partial class TitleKeys : Window
    {
        public TitleKeys(string url)
        {
            MainViewModel mvm = DataContext as MainViewModel;
            try
            {
                if (Owner?.GetType() != typeof(MainWindow))
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }
            catch (Exception)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            string fullurl = "";

            wb.Source = new Uri(fullurl, UriKind.Absolute);
            wb.Refresh(true);

            clsWebbrowser_Errors.SuppressscriptErrors(wb, true);
            InitializeComponent();
        }
        public TitleKeys(string url, string title)
        {
            InitializeComponent();
            wb.Visibility = Visibility.Hidden;
            Thread t = new Thread(() => DoStuff(url));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            tbTitleBar.Text = title.Replace(" Inject ", " ");
        }
        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    Owner.PointToScreen(new Point(Left, Top));
                    DragMove();
                    // this.Owner.PointFromScreen(new Point(this.Left, this.Top));
                    //(FindResource("mvm") as MainViewModel).mw.PointFromScreen(new Point(this.Left, this.Top));
                }

                //PointFromScreen(new Point(this.Left, this.Top));

            }
            catch (Exception)
            {
                // left empty on purpose
            }
        }

        public void DoStuff(string url)
        {
            MainViewModel mvm = DataContext as MainViewModel;
            string fullurl = "";

            Dispatcher.Invoke(() =>
            {
                wb.Source = new Uri(fullurl, UriKind.Absolute);
                clsWebbrowser_Errors.SuppressscriptErrors(wb, true);
            });
        }

        private void Window_Close(object sender, RoutedEventArgs e)
        {
            Owner.Left = Left;
            Owner.Top = Top;
            Owner.Show();
            Close();
        }

        private void Window_Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void wb_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            load.Visibility = Visibility.Hidden;
            wb.Visibility = Visibility.Visible;
        }



        private void wb_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (!e.Uri.ToString().Contains("UWUVCI-PRIME"))
            {
                e.Cancel = true;

                var startInfo = new ProcessStartInfo
                {
                    FileName = e.Uri.ToString()
                };

                Process.Start(startInfo);
            }

            // cancel navigation to the clicked link in the webBrowser control
        }
    }
    public static class clsWebbrowser_Errors
    {
        //*set wpf webbrowser Control to silent

        //*code source: https://social.msdn.microsoft.com/Forums/vstudio/en-US/4f686de1-8884-4a8d-8ec5-ae4eff8ce6db


        public static void SuppressscriptErrors(this WebBrowser webBrowser, bool hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);

            if (fiComWebBrowser == null)
                return;

            object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);

            if (objComWebBrowser == null)
                return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }
    }
}
