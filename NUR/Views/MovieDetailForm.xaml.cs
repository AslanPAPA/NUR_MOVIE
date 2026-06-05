using NUR.Data;
using NUR.Models;
using System.IO;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace NUR.Views
{

    public partial class MovieDetailForm : UserControl
    {
        public MovieDetailForm()
        {
            InitializeComponent();
            this.Loaded += MovieDetailForm_Loaded;
        }

        private async void MovieDetailForm_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDownloadButton();
            await UpdateFavoriteButton();
        }

        private void UpdateDownloadButton()
        {
            var movie = this.DataContext as Movie;
            if (movie == null) return;

            bool isDownloaded = DownloadManager.IsDownloaded(movie.Id);

            BtnDownload.IsEnabled = !isDownloaded;

            if (isDownloaded)
            {
                BtnDownload.Content = "ФИЛЬМ СКАЧАН";
                BtnDownload.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#28a745"));
                BtnDownload.Foreground = System.Windows.Media.Brushes.White;
            }
            else
            {
                BtnDownload.Content = "СМОТРЕТЬ ОФФЛАЙН";
                BtnDownload.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFD700"));
                BtnDownload.Foreground = System.Windows.Media.Brushes.Black;
            }
        }



        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            var movie = this.DataContext as Movie;
            if (movie == null) return;

            if (DownloadManager.IsDownloaded(movie.Id))
            {
                return;
            }

            DownloadWindow downloadWin = new DownloadWindow(movie.VideoUrl, movie.Id);
            downloadWin.ShowDialog();

            UpdateDownloadButton();
        }


        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ShowForm(mainWindow.homeForm);
            }
        }

        private void BtnWatch_Click(object sender, RoutedEventArgs e)
        {
            var movie = this.DataContext as Movie;
            if (movie == null) return;

            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow == null) return;

            if (DownloadManager.IsDownloaded(movie.Id))
            {
                string localPath = DownloadManager.GetLocalPath(movie.Id);
                if (File.Exists(localPath))
                {
                    mainWindow.StartPlayer(localPath);
                }
                else
                {
                    UpdateDownloadButton();
                    MessageBox.Show("Файл не найден. Снова доступно для скачивания.");
                }
            }
            else if (!string.IsNullOrEmpty(movie.VideoUrl))
            {
                mainWindow.StartPlayer(movie.VideoUrl);
            }
            else
            {
                MessageBox.Show("Ошибка: Ссылка на видео отсутствует.");
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                UpdateDownloadButton();
                UpdateFavoriteButton();
            }
        }

        private async Task UpdateFavoriteButton()
        {
            var movie = DataContext as Movie;

            if (movie == null)
                return;

            string json =
                await ApiClient.Instance.GetStringAsync(
                    $"http://185.246.222.35:8080/api/favorites/{movie.Id}/");

            bool isFavorite =
                json.Contains("\"is_favorite\":true");

            FavIcon.Source = new BitmapImage(
                new Uri(
                    isFavorite
                        ? "/Assets/images/full_heart.png"
                        : "/Assets/images/no_full_heart.png",
                    UriKind.Relative));
        }

        private void UpdateFavoriteIcon(bool isFavorite)
        {
            FavIcon.Source = new BitmapImage(
                new Uri(
                    isFavorite
                        ? "/Assets/images/full_heart.png"
                        : "/Assets/images/no_full_heart.png",
                    UriKind.Relative));
        }

        private async void BtnFavorite_Click(object sender, RoutedEventArgs e)
        {
            var movie = DataContext as Movie;
            if (movie == null) return;

            var client = ApiClient.Instance;

            var response = await client.PostAsJsonAsync(
                "http://185.246.222.35:8080/api/favorites/toggle/",
                new { movie_id = movie.Id }
            );

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Ошибка сервера");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();

            bool isFavorite = json.Contains("true");

            UpdateFavoriteIcon(isFavorite);

            var main = Application.Current.MainWindow as MainWindow;
            main?.RefreshFavorites();
        }
    }
}