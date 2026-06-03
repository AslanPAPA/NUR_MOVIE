using NUR.Models;
using NUR.Services;
using SharpGen.Runtime;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace NUR.Views
{
    public partial class MainWindow : Window
    {
        private List<Movie> _allMoviesFromApi;
        private DispatcherTimer _searchTimer;
        private bool _ignoreSearchTextChanged;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            _searchTimer = new DispatcherTimer();
            _searchTimer.Interval = TimeSpan.FromMilliseconds(300);
            _searchTimer.Tick += SearchTimer_Tick;
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
                _allMoviesFromApi = await GetMoviesAsync();
                FillFilters();
                if (_allMoviesFromApi != null)
                {
                    UpdateHomeFormDisplay(_allMoviesFromApi);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void FillFilters()
        {
            GenreFilter.Items.Add("Все жанры");

            foreach (var genre in _allMoviesFromApi
                .SelectMany(m => m.Genres)
                .Select(g => g.Name)
                .Distinct()
                .OrderBy(x => x))
            {
                GenreFilter.Items.Add(genre);
            }

            GenreFilter.SelectedIndex = 0;

            ActorFilter.Items.Add("Все актёры");

            foreach (var actor in _allMoviesFromApi
                .SelectMany(m => m.Actors)
                .Select(a => a.Name)
                .Distinct()
                .OrderBy(x => x))
            {
                ActorFilter.Items.Add(actor);
            }

            ActorFilter.SelectedIndex = 0;

            YearFilter.Items.Add("Все годы");

            foreach (var year in _allMoviesFromApi
                .Select(m => m.Year)
                .Distinct()
                .OrderByDescending(x => x))
            {
                YearFilter.Items.Add(year);
            }

            YearFilter.SelectedIndex = 0;
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Movie> filtered = _allMoviesFromApi;

            if (GenreFilter.SelectedIndex > 0)
            {
                string genre = GenreFilter.SelectedItem.ToString();

                filtered = filtered.Where(m =>
                    m.Genres.Any(g => g.Name == genre));
            }

            if (ActorFilter.SelectedIndex > 0)
            {
                string actor = ActorFilter.SelectedItem.ToString();

                filtered = filtered.Where(m =>
                    m.Actors.Any(a => a.Name == actor));
            }

            if (YearFilter.SelectedIndex > 0)
            {
                int year = Convert.ToInt32(YearFilter.SelectedItem);

                filtered = filtered.Where(m => m.Year == year);
            }

            UpdateHomeFormDisplay(filtered.ToList());

            FilterPopup.IsOpen = false;
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            GenreFilter.SelectedIndex = 0;
            ActorFilter.SelectedIndex = 0;
            YearFilter.SelectedIndex = 0;

            UpdateHomeFormDisplay(_allMoviesFromApi);
        }
        private void UpdateHomeFormDisplay(List<Movie> movies)
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
                .Where(g => g.Movies.Any()).ToList();

            homeForm.DataContext = new { FreshMovies = freshMovies, MovieGenres = groupedData };
        }


        private async Task<List<Movie>> GetMoviesAsync()
        {
                string url = "http://185.246.222.35:8080/api/movies/";

                var response = await ApiClient.Instance.GetStringAsync(url);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Movie>>(response, options);
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
                    case "Профиль":
                        ShowForm(userProfile);
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
            userProfile.Visibility = Visibility.Collapsed;
            searchResultsForm.Visibility = Visibility.Collapsed;

            selectedForm.Visibility = Visibility.Visible;
        }


        public void OpenMovieDetail(Movie movie)
        {
            _ignoreSearchTextChanged = true;
            SearchBox.Text = "";
            _ignoreSearchTextChanged = false;

            movieDetailForm.DataContext = movie;
            ShowForm(movieDetailForm);
        }

        public void StartPlayer(string videoUrl)
        {
            ShowForm(videoForm);
            videoForm.mediaStart(videoUrl);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ShowForm(settingsForm);
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterPopup.IsOpen = true;
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_ignoreSearchTextChanged)
                return;

            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private void SearchTimer_Tick(object? sender, EventArgs e)
        {
            _searchTimer.Stop();

            string query = SearchBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(query))
            {
                ShowForm(homeForm);
                return;
            }

            var results = _allMoviesFromApi
                    .Where(m =>

                            // Название фильма
                            (m.Title != null &&
                             m.Title.Contains(query, StringComparison.OrdinalIgnoreCase))

                            ||

                            // Жанры
                            (m.Genres != null &&
                             m.Genres.Any(g =>
                                 g.Name.Contains(query, StringComparison.OrdinalIgnoreCase)))

                            ||

                            // Актёры
                            (m.Actors != null &&
                             m.Actors.Any(a =>
                                 a.Name.Contains(query, StringComparison.OrdinalIgnoreCase)))

                        )
                        .ToList();

            searchResultsForm.SetMovies(results, query);

            ShowForm(searchResultsForm);
        }

    }
   
}
