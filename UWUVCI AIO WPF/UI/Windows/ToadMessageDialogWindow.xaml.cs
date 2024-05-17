using System.Threading;
using System;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace UWUVCI_AIO_WPF.UI.Windows
{
    /// <summary>
    /// Custom message box.
    /// </summary>
    public partial class ToadMessageDialogWindow : Window
    {
        public event RoutedEventHandler Click;
        private readonly string _text;
        private bool _skipIt = false;

        /// <summary>
        /// Creates a new instance of the <see cref="ToadMessageDialogWindow"/>.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public ToadMessageDialogWindow(string message)
        {
            InitializeComponent();
            _text = message;
            this.Loaded += ToadMessageDialogWindow_Loaded;
        }

        private void AddText()
        {
            Thread.Sleep(500);
            foreach (char c in _text)
            {
                if (!_skipIt)
                {
                    Thread.Sleep(30);
                    textBlock.Dispatcher.BeginInvoke(new Action(() => textBlock.Text += c));
                }
                else
                {
                    textBlock.Dispatcher.BeginInvoke(new Action(() => textBlock.Text = _text));
                    break;
                }
            }
            okButton.Dispatcher.BeginInvoke(new Action(() =>
            {
                okButton.Visibility = Visibility.Visible;
                okButton.InvalidateVisual();
            }));
        }
        private void ToadMessageDialogWindow_Loaded(object sender, RoutedEventArgs e)
        { 
            this.KeyDown += ToadMessageDialogWindow_KeyDown;
            this.MouseDown += ToadMessageDialogWindow_MouseDown;
            _ = Task.Run(() => AddText());
            ApplyMoveAnimation();
        }

        private void ToadMessageDialogWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _skipIt = true;
        }

        private void ToadMessageDialogWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            _skipIt = true;
        }

        private void ApplyMoveAnimation()
        {
            var move = new ThicknessAnimation()
            {
                From = new Thickness(-500, 100, 700, 100),
                To = new Thickness(100, 100, 100, 100),
                Duration = TimeSpan.FromSeconds(0.75)
            };
            Storyboard.SetTargetProperty(move, new PropertyPath(Grid.MarginProperty));
            Storyboard.SetTarget(move, grid);
            move.EasingFunction = new ElasticEase() { EasingMode = EasingMode.EaseOut, Springiness = .001, Oscillations = 1 };
            var sb = new Storyboard();
            sb.Children.Add(move);
            sb.Begin();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(sender, e);
            Close();
        }
    }
}
