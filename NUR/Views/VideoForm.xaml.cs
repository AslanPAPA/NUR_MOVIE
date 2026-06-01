using FlyleafLib;
using FlyleafLib.MediaPlayer;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

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
            playerConfig.Player.SeekOffset = 5000;
            playerConfig.Player.SeekOffset2 = 10000;
            playerConfig.Player.SeekOffset3 = 15000;
            playerConfig.Player.SeekAccurate = true;

            player = new Player(playerConfig);

            InitializeComponent();
            mediaPlayerHost.Player = player;

            mediaPlayerHost.Player.BufferingStarted += Player_BufferingStarted;
            mediaPlayerHost.Player.BufferingCompleted += Player_BufferingCompleted;

            // Слушаем изменения свойств (Пауза, Звук)
            player.PropertyChanged += Player_PropertyChanged;

            // Клик по плееру возвращает фокус форме
            mediaPlayerHost.PreviewMouseLeftButtonDown += (s, e) => { this.Focus(); };

            this.Focusable = true;
            this.PreviewKeyDown += VideoForm_PreviewKeyDown;
        }

        // Автоматическое обновление кнопок при изменении состояния плеера
        private void Player_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.PropertyName == nameof(Player.IsPlaying))
                {
                    btnPlayPause.Content = player.IsPlaying ? "⏸" : "▶";
                }

                if (e.PropertyName == "Mute")
                {
                    btnMute.Content = player.Audio.Mute ? "🔇" : "🔊";
                }
            });
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
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
            if (player != null)
            {
                player.PropertyChanged -= Player_PropertyChanged;
                player.Dispose();
            }
        }

        private void VideoForm_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (player == null) return;

            switch (e.Key)
            {
                case Key.Space:
                    TogglePlayPause();
                    e.Handled = true;
                    break;

                case Key.Right:
                    player.SeekForward();
                    e.Handled = true;
                    break;

                case Key.Left:
                    player.SeekBackward();
                    e.Handled = true;
                    break;

                case Key.F:
                    ToggleFullScreenWithFocus(); // Вызываем хитрый метод с удержанием фокуса
                    e.Handled = true;
                    break;

                case Key.M:
                    ToggleMute();
                    e.Handled = true;
                    break;

                case Key.Escape:
                    if (mediaPlayerHost != null && mediaPlayerHost.IsFullScreen)
                    {
                        ToggleFullScreenWithFocus();
                        e.Handled = true;
                    }
                    break;
            }
        }

        // Хитрый метод: переключает экран и сразу возвращает фокус через микро-задержку диспетчера
        private void ToggleFullScreenWithFocus()
        {
            player.ToggleFullScreen();

            // Даем Windows долю миллисекунды развернуть окно, а затем нагло забираем фокус ввода обратно в форму
            Dispatcher.InvokeAsync(() =>
            {
                this.Focus();
                mediaPlayerHost.Focus();
            }, System.Windows.Threading.DispatcherPriority.Input);
        }

        private void TogglePlayPause()
        {
            if (player.IsPlaying)
                player.Pause();
            else
                player.Play();
        }

        private void ToggleMute()
        {
            player.Audio.Mute = !player.Audio.Mute;
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            TogglePlayPause();
            this.Focus();
        }

        private void btnMuteUnmute_Click(object sender, RoutedEventArgs e)
        {
            ToggleMute();
            this.Focus();
        }

        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullScreenWithFocus();
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

            this.Focus();
        }
    }
}