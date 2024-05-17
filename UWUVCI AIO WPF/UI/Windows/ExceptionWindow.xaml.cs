using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace UWUVCI_AIO_WPF.UI.Windows
{
    public partial class ExceptionWindow : Window
    {
        private readonly MediaPlayer _player = new MediaPlayer();
        public ExceptionWindow(string title, string message)
        {
            InitializeComponent();            
            textBlockMessage.Text = message;    
            textBlockTitle.Text = title;
            this.Loaded += ExceptionWindow_Loaded;
            this.Unloaded += ExceptionWindow_Unloaded;
        }

        private void ExceptionWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            _player?.Stop();
        }

        private void ExceptionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _player.ScrubbingEnabled = false;
            _player.Open(new Uri("../../UI/Sounds/death.mp3", UriKind.RelativeOrAbsolute));
            _player.Volume = 0.6;
            _player.Play();

            Thickness margin = mario.Margin;          

            int up = 100;
            int down = 250;
            var bouncer = new ThicknessAnimation(new Thickness(10, margin.Top, 10, margin.Bottom), new Thickness(10, margin.Top - up, 10, margin.Bottom + up), new Duration(TimeSpan.FromSeconds(0.6)));
            bouncer.BeginTime = TimeSpan.FromSeconds(0.4);
            bouncer.EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut };
            var sb = new Storyboard();
            sb.Children.Add(bouncer);
            Storyboard.SetTarget(bouncer, this.mario);
            Storyboard.SetTargetProperty(bouncer, new PropertyPath("Margin"));
            var done = false;
            sb.Completed += (s, args) =>
            {
                if (!done)
                {
                    sb.Stop();
                    sb.Children.Clear();
                    bouncer = new ThicknessAnimation(new Thickness(10, margin.Top - up, 10, margin.Bottom + up), new Thickness(10, margin.Top - up + down, 10, margin.Bottom + up - down), new Duration(TimeSpan.FromSeconds(.5)));
                    sb.Children.Add(bouncer);
                    Storyboard.SetTarget(bouncer, this.mario);
                    Storyboard.SetTargetProperty(bouncer, new PropertyPath("Margin"));
                    sb.Begin();
                    done = true;
                }
            };
            sb.Begin();
        }

        private void CustomButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
