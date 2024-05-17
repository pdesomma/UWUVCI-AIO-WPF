using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace UWUVCI_AIO_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static byte[] StreamToBytes(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        internal void Is32()
        {
            //Wii.DataContext = this;
            //Wii.IsEnabled = false;
            //GC.DataContext = this;
            //GC.IsEnabled = false;
        }

        private void VwiiMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var p = new Process();
                var fileName = Application.ResourceAssembly.Location;
                foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.exe"))
                if (Path.GetFileName(file).ToLower().Contains("vwii"))
                {
                    fileName = file;
                    break;
                }

                p.StartInfo.FileName = fileName;
                p.Start();
                Environment.Exit(0);
            }
            catch { /* left empty on purpose */ }
        }

        private void BegMacButton_Click(object sender, RoutedEventArgs e) => Process.Start("https://ko-fi.com/uwuvci");

        private void BegUwuvciButton_Click(object sender, RoutedEventArgs e) => Process.Start("https://ko-fi.com/zestyts");

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1 && e.ChangedButton == MouseButton.Left) this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
            else if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void WindowsCloseButton_MouseDown(object sender, MouseButtonEventArgs e) => Close();

        private void WindowsMaxButton_MouseDown(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Maximized;

        private void WindowsMinButton_MouseDown(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;

        private void WIndowsRestoreButton_MouseDown(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Normal;

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                winRestoreButton.Visibility = Visibility.Collapsed;
                winMaxButton.Visibility = Visibility.Visible;
            }
            else if (WindowState == WindowState.Maximized)
            {
                winRestoreButton.Visibility = Visibility.Visible;
                winMaxButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}