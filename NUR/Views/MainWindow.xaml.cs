using NUR.Data;
using NUR.Models;
using SharpGen.Runtime;
using System.IO;
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
        public List<Movie> AllMovies => _allMoviesFromApi;
        private DispatcherTimer _searchTimer;
        private bool _ignoreSearchTextChanged;
        private bool _isOffline = false;
        private string _currentCatalog = "Все";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            _searchTimer = new DispatcherTimer();
            _searchTimer.Interval = TimeSpan.FromMilliseconds(300);
            _searchTimer.Tick += SearchTimer_Tick;
        }

        private void SetOfflineMode(bool isOffline)
        {
            _isOffline = isOffline;

            OfflineBanner.Visibility =
                isOffline ? Visibility.Visible : Visibility.Collapsed;
        }

        private async Task CheckInternetAndUpdateUI()
        {
            bool hasInternet = await InternetHelper.HasInternet();

            SetOfflineMode(!hasInternet);
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

        private async void StartInternetWatcher()
        {
            while (true)
            {
                await CheckInternetAndUpdateUI();
                await Task.Delay(5000); // каждые 5 сек
            }
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckInternetAndUpdateUI();
            StartInternetWatcher();
            try
            {
                // Сначала загружаем фильмы из локальной базы
                _allMoviesFromApi = DatabaseService.LoadMovies();


                if (_allMoviesFromApi != null)
                {
                    foreach (var movie in _allMoviesFromApi)
                    {

                       
                        string localPoster =
                            PosterManager.GetLocalPoster(movie.Id);

                        if (File.Exists(localPoster))
                        {
                            movie.Poster = localPoster;
                        }
                    }

                    if (_allMoviesFromApi.Any())
                    {
                        UpdateHomeFormDisplay(_allMoviesFromApi);
                        FillFilters();
                    }
                }

                bool hasInternet = await InternetHelper.HasInternet();

                if (hasInternet)
                {
                    var freshMovies = await GetMoviesAsync();

                    if (freshMovies != null)
                    {
                        _allMoviesFromApi = freshMovies;

                        DatabaseService.SaveMovies(_allMoviesFromApi);

                        foreach (var movie in _allMoviesFromApi)
                        {
                            await PosterManager.DownloadPoster(movie);
                        }

                        foreach (var movie in _allMoviesFromApi)
                        {
                            string localPoster =
                                PosterManager.GetLocalPoster(movie.Id);

                            if (File.Exists(localPoster))
                            {
                                movie.Poster = localPoster;
                            }
                        }

                        UpdateHomeFormDisplay(_allMoviesFromApi);
                        FillFilters();
                    }
                }
                else
                {
                    if (_allMoviesFromApi == null || !_allMoviesFromApi.Any())
                    {
                        MessageBox.Show(
                            "Нет подключения к интернету, и локальная база пуста. Подключитесь к сети для первого запуска.");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void FillFilters()
        {
            GenreFilter.Items.Clear();
            ActorFilter.Items.Clear();
            YearFilter.Items.Clear();


            GenreFilter.Items.Add("Все жанры");

            foreach (var genre in _allMoviesFromApi
                .SelectMany(m => m.Genres ?? Enumerable.Empty<Genre>())
                .Select(g => g.Name?.Trim())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .GroupBy(name => name.ToLower())   
                .Select(g => g.First())           
                .OrderBy(name => name))
            {
                GenreFilter.Items.Add(genre);
            }

            GenreFilter.SelectedIndex = 0;


            ActorFilter.Items.Add("Все актёры");

            foreach (var actor in _allMoviesFromApi
                .SelectMany(m => m.Actors ?? Enumerable.Empty<Actor>())
                .Select(a => a.Name?.Trim())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .GroupBy(name => name.ToLower())   // убираем дубли
                .Select(g => g.First())
                .OrderBy(name => name))
            {
                ActorFilter.Items.Add(actor);
            }

            ActorFilter.SelectedIndex = 0;

            // =======================
            // ГОДЫ
            // =======================
            YearFilter.Items.Add("Все годы");

            foreach (var year in _allMoviesFromApi
                .Select(m => m.Year)
                .Where(y => y > 0)
                .Distinct()
                .OrderByDescending(y => y))
            {
                YearFilter.Items.Add(year);
            }

            YearFilter.SelectedIndex = 0;
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Movie> filtered = _allMoviesFromApi;

            // =======================
            // ЖАНР (стандарт: фильм может иметь несколько жанров)
            // =======================
            if (GenreFilter.SelectedIndex > 0)
            {
                string genre = GenreFilter.SelectedItem.ToString();

                filtered = filtered.Where(m =>
                    m.Genres != null &&
                    m.Genres.Any(g => g.Name == genre));
            }

            // =======================
            // АКТЁР (стандарт)
            // =======================
            if (ActorFilter.SelectedIndex > 0)
            {
                string actor = ActorFilter.SelectedItem.ToString();

                filtered = filtered.Where(m =>
                    m.Actors != null &&
                    m.Actors.Any(a => a.Name == actor));
            }

            // =======================
            // ГОД
            // =======================
            if (YearFilter.SelectedIndex > 0)
            {
                int year = Convert.ToInt32(YearFilter.SelectedItem);

                filtered = filtered.Where(m => m.Year == year);
            }

            var resultList = filtered.ToList();

            UpdateHomeFormDisplay(resultList);

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
            if (movies == null) return; 

            int currentYear = 2026;
            var freshMovies = movies.Where(m => m.Year == currentYear).ToList();

            var interestingGenres = new List<string> { "Боевик", "Комедия", "Фантастика", "Драма", "Приключения", "Триллер", "Криминал", "Фэнтези", "Детектив", "Мультфильмы" };
            var groupedData = interestingGenres
                .Select(name => new GenreGroup
                {
                    GenreName = name.ToUpper(),
                    Movies = movies.Where(m => m.Genres != null && m.Genres.Any(g => g.Name == name)).ToList()
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
                switch (clickedButton.Name)
                {
                    case "BtnHome":
                        ShowForm(homeForm);
                        break;

                    case "BtnSettings":
                        ShowForm(settingsForm);
                        break;

                    case "BtnProfile":
                        ShowForm(userProfile);
                        break;

                    case "BtnHistory":
                        ShowForm(historyForm);
                        break;

                    case "BtnFavorites":
                        favoritesForm.LoadFavorites();
                        ShowForm(favoritesForm);
                        break;
                }
            }
        }

        internal void ShowForm(UIElement selectedForm)
        {
            if (selectedForm != videoForm)
            {
                videoForm.player?.Pause();
            }

            homeForm.Visibility = Visibility.Collapsed;
            videoForm.Visibility = Visibility.Collapsed;
            settingsForm.Visibility = Visibility.Collapsed;
            movieDetailForm.Visibility = Visibility.Collapsed;
            userProfile.Visibility = Visibility.Collapsed;
            searchResultsForm.Visibility = Visibility.Collapsed;
            favoritesForm.Visibility = Visibility.Collapsed;
            historyForm.Visibility = Visibility.Collapsed;

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
            string path = videoUrl;
            if (!videoUrl.StartsWith("http"))
            {
                if (!Path.IsPathRooted(videoUrl))
                {
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, videoUrl);
                }
            }

            ShowForm(videoForm);
            videoForm.mediaStart(videoUrl);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ShowForm(settingsForm);
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

        public void RefreshFavorites()
{
        favoritesForm.LoadFavorites();
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

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            CatalogPopup.IsOpen = true;
        }

        private void UpdateCatalogMenu()
        {
            BtnAllCatalog.Content =
                _currentCatalog == "Все"
                    ? "✔ Все"
                    : "Все";

            BtnMoviesCatalog.Content =
                _currentCatalog == "Фильмы"
                    ? "✔ Фильмы"
                    : "Фильмы";

            BtnCartoonsCatalog.Content =
                _currentCatalog == "Мультфильмы"
                    ? "✔ Мультфильмы"
                    : "Мультфильмы";
        }

        private void MoviesCatalog_Click(object sender, RoutedEventArgs e)
        {
            _currentCatalog = "Фильмы";

            UpdateCatalogMenu();

            var movies = _allMoviesFromApi
                .Where(m => m.Genres == null ||
                       !m.Genres.Any(g => g.Name == "Мультфильмы"))
                .ToList();

            UpdateHomeFormDisplay(movies);

            CatalogPopup.IsOpen = false;
        }

        private void CartoonsCatalog_Click(object sender, RoutedEventArgs e)
        {
            _currentCatalog = "Мультфильмы";

            UpdateCatalogMenu();

            var cartoons = _allMoviesFromApi
                .Where(m => m.Genres != null &&
                       m.Genres.Any(g => g.Name == "Мультфильмы"))
                .ToList();

            UpdateHomeFormDisplay(cartoons);

            CatalogPopup.IsOpen = false;
        }

        private void AllCatalog_Click(object sender, RoutedEventArgs e)
        {
            _currentCatalog = "Все";

            UpdateCatalogMenu();

            UpdateHomeFormDisplay(_allMoviesFromApi);

            CatalogPopup.IsOpen = false;
        }
    }
   
}
