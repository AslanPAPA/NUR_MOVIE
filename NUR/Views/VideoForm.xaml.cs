using FlyleafLib;
using FlyleafLib.MediaPlayer;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            InitializeComponent();
            mediaPlayerHost.Player = player;
        }

        public void mediaStart(string videoUrl)
        {
            if (!string.IsNullOrEmpty(videoUrl) && player != null)
            { 
                player.OpenAsync(videoUrl);
                navContainer.IsEnabled = true;
            }

        }
        private void VideoForm_Unloaded(object sender, RoutedEventArgs e)
        {
            if (player != null)
            {
                player.Dispose();
            }
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (player.IsPlaying)
            {
                btnPlayPause.Content = "▶";
                player.Pause();
            }
            else
            {
                btnPlayPause.Content = "⏸";
                player.Play();
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

            // Получаем точку клика относительно слайдера
            Point point = e.GetPosition(slider);

            // Считаем пропорцию: где кликнули / вся длина слайдера
            double volumeSquare = point.X / slider.ActualWidth;

            // Вычисляем новое значение времени в тиках
            double newValue = volumeSquare * slider.Maximum;

            if (newValue >= slider.Minimum && newValue <= slider.Maximum)
            {
                slider.Value = newValue; // Слайдер сам обновит CurTime в плеере
            }
        }
    }
}