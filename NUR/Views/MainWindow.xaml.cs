using NUR.Models;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NUR.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены что хотите выйти ?", "Оповещение", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void videoPlayer_Click(object sender, RoutedEventArgs e)
        {
            videoForm.Visibility = Visibility.Visible;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var movies = await GetMoviesAsync();

                if (movies != null)
                {
                    int currentYear = 2026;

                    var freshMovies = movies.Where(m => m.Year == currentYear).ToList();

                    var interestingGenres = new List<string> { "Боевик", "Комедия", "Фантастика", "Драма" };
                    var groupedData = interestingGenres
                        .Select(name => new GenreGroup
                        {
                            GenreName = name.ToUpper(),
                            Movies = movies.Where(m => m.Genres.Any(g => g.Name == name)).ToList()
                        })
                        .Where(g => g.Movies.Any())
                        .ToList();

                    homeForm.DataContext = new
                    {
                        FreshMovies = freshMovies,
                        MovieGenres = groupedData
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }



        private async Task<List<Movie>> GetMoviesAsync()
        {
            using (HttpClient client = new HttpClient())
            {

                string url = "http://185.246.222.35:8080/api/movies/";

                var response = await client.GetStringAsync(url);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<List<Movie>>(response, options);
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button clickedButton)
            {
                string buttonText = clickedButton.Content.ToString();

                switch (buttonText)
                {
                    case "Главная":
                        ShowForm(homeForm);
                        break;
                    case "Видеоплеер":
                        ShowForm(videoForm);
                        break;
                    case "Настройки":
                        ShowForm(settingsForm);
                        break;
                    case "Избранное":
                        MessageBox.Show("Раздел 'Избранное' в разработке");
                        break;
                }
            }
        }

        internal void ShowForm(UIElement selectedForm)
        {
            homeForm.Visibility = Visibility.Collapsed;
            videoForm.Visibility = Visibility.Collapsed;
            settingsForm.Visibility = Visibility.Collapsed;
            movieDetailForm.Visibility = Visibility.Collapsed;
            selectedForm.Visibility = Visibility.Visible;
        }


        public void OpenMovieDetail(Movie movie)
        {
            movieDetailForm.DataContext = movie;
            ShowForm(movieDetailForm);
        }

        public void StartPlayer(string videoUrl)
        {
            ShowForm(videoForm);
            videoForm.mediaStart(videoUrl);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ShowForm(settingsForm);
        }
    }
   
}
