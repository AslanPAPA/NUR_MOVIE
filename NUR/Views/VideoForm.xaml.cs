using FlyleafLib;
using FlyleafLib.MediaPlayer;
using System.Diagnostics;
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
        private bool isDragging = false;

        public VideoForm()
        {
            playerConfig = new Config();


            player = new Player(playerConfig);

            this.DataContext = this;

            InitializeComponent();
            mediaPlayerHost.Player = player;

            

            mediaPlayerHost.Player.BufferingStarted += Player_BufferingStarted;
            mediaPlayerHost.Player.BufferingCompleted += Player_BufferingCompleted;

   
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
                navContainer.IsEnabled = true;
                this.Focus();
            }
        }

        private void VideoForm_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("VideoForm_Unloaded");

            if (player != null)
            {
                player.Stop();
                player.Dispose();
            }
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
    }
}