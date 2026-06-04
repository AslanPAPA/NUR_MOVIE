using NUR.Data;
using NUR.Models;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace NUR.Views
{

    public partial class MovieDetailForm : UserControl
    {
        public MovieDetailForm()
        {
            InitializeComponent();
            this.Loaded += MovieDetailForm_Loaded;
        }

        private void MovieDetailForm_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDownloadButton();
        }

        private void UpdateDownloadButton()
        {
            var movie = this.DataContext as Movie;
            if (movie == null) return;

            bool isDownloaded = DownloadManager.IsDownloaded(movie.Id);

            BtnDownload.IsEnabled = !isDownloaded; // Кнопка перестает нажиматься, если файл есть

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

            // ВАЖНО: Проверяем, скачан ли файл, ПЕРЕД тем как открывать окно
            if (DownloadManager.IsDownloaded(movie.Id))
            {
                // Если уже скачан, вообще не открываем окно!
                return;
            }

            // Если не скачан — открываем окно загрузки
            DownloadWindow downloadWin = new DownloadWindow(movie.VideoUrl, movie.Id);
            downloadWin.ShowDialog();

            // Обновляем состояние кнопки после закрытия окна
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

            // Проверяем: если скачано - берем локальный путь, если нет - онлайн ссылку
            if (DownloadManager.IsDownloaded(movie.Id))
            {
                string localPath = DownloadManager.GetLocalPath(movie.Id);
                // Дополнительная проверка на случай, если файл удалили вручную из папки
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
            }
        }
    }
}