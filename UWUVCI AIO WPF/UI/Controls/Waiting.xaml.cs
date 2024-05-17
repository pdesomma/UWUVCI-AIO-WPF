using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Threading;

namespace UWUVCI_AIO_WPF.UI.Controls
{
    public partial class Waiting : UserControl
    {
        private readonly int _frameHeight = 447;
        private readonly int _frameWidth = 251;
        private bool _running = false;
        int index = 0;
        private readonly List<ImageBrush> _images = new List<ImageBrush>();

        public Waiting()
        {
            InitializeComponent();
            this.Loaded += WaitControl_Loaded;
        }

        private void WaitControl_Loaded(object sender, RoutedEventArgs e)
        {
            Int32Rect cropRect = new Int32Rect(0, 0, _frameWidth, _frameHeight);
            BitmapImage sheet = new BitmapImage(new Uri("pack://application:,,,/UI/Images/mario-running.png", UriKind.RelativeOrAbsolute));
            ImageSource frame;
            for (int col = 0; col < 3; col++)
            {
                int currentY = 0;
                int currentX = col * this._frameWidth;
                cropRect.X = currentX;
                cropRect.Y = currentY;
                frame = new CroppedBitmap(sheet, cropRect);
                _images.Add(new ImageBrush(frame));
            }

            _running = true;
            _ = Task.Run(() =>
            {
                while(_running)
                {
                    Thread.Sleep(100);
                    mario.Dispatcher.Invoke(() => mario.Fill = _images[index]);
                    index = (index + 1) % 3;
                }
            });
        }
    }
}
