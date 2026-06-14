using FlyleafLib;
using FlyleafLib.MediaPlayer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace NUR.Views
{
    public partial class VideoForm : UserControl
    {
        public Player player { get; set; }
        private Config playerConfig;
        private TimeSpan lastPosition = TimeSpan.Zero;

        public VideoForm()
        {
            InitializeComponent();
            this.Loaded += VideoForm_Loaded;
        }

        private void VideoForm_Loaded(object sender, RoutedEventArgs e)
        {
            if (mediaPlayerHost != null && player == null)
            {
                try
                {
                    playerConfig = new Config();
                    player = new Player(playerConfig);
                    mediaPlayerHost.Player = player;

                    mediaPlayerHost.Player.BufferingStarted += Player_BufferingStarted;
                    mediaPlayerHost.Player.BufferingCompleted += Player_BufferingCompleted;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка {ex.Message}");
                }   
            }
        }
        private void Player_BufferingStarted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (player != null && !player.IsPlaying) return;
                if (loadingOverlay != null) loadingOverlay.Visibility = Visibility.Visible;
            });
        }

        private void Player_BufferingCompleted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                loadingOverlay.Visibility = Visibility.Collapsed;
            });
        }

        public void mediaStart(string videoUrl)
        {
            if (!string.IsNullOrEmpty(videoUrl) && player != null)
            {
                player.OpenAsync(videoUrl);
                ApplySavedSpeed();
                navContainer.IsEnabled = true;
                this.Focus();
            }
        }

        private void ApplySavedSpeed()
        {
            if (player == null)
                return;

            string speed =
                Properties.Settings.Default.VideoSpeed;

            double rate = speed switch
            {
                "0.5x" => 0.5,
                "0.75x" => 0.75,
                "1x" => 1.0,
                "1.25x" => 1.25,
                "1.5x" => 1.5,
                "2x" => 2.0,
                _ => 1.0
            };

            player.Speed = rate;

            btnVideoSpeed.Content = speed;
        }

        private void VideoForm_Unloaded(object sender, RoutedEventArgs e)
        {
            player?.Pause();
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (player.IsPlaying)
            {
                player.Pause();
                btnPlayPause.Content = "▶";
            }
            else
            {
                player.Play();
                btnPlayPause.Content = "⏸";  
            }

        }

        private void btnMuteUnmute_Click(object sender, RoutedEventArgs e)
        {
            player.Audio.Mute = !player.Audio.Mute;
            btnMute.Content = player.Audio.Mute ? "🔇" : "🔊";
        }

        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            player.ToggleFullScreen();
        }

        private void VideoSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var slider = sender as Slider;
            if (slider == null) return;

            if (e.OriginalSource is DependencyObject originalSource)
            {
                DependencyObject parent = originalSource;
                while (parent != null && parent != slider)
                {
                    if (parent is Thumb) return;
                    parent = VisualTreeHelper.GetParent(parent);
                }
            }

            Point point = e.GetPosition(slider);
            double volumeSquare = point.X / slider.ActualWidth;
            double newValue = volumeSquare * slider.Maximum;

            if (newValue >= slider.Minimum && newValue <= slider.Maximum)
            {
                slider.Value = newValue;
            }
            
        }

        private void btnQuality_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (player != null)
            {
                lastPosition = TimeSpan.FromMilliseconds(player.CurTime);
                player.Pause();
            }
            
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.ShowForm(mainWindow.movieDetailForm);
        }

        private void btnVideoSpeed_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}