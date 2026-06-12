using NUR.Data;
using NUR.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NUR.Views
{
    public partial class FavoritesForm : UserControl
    {
        public ObservableCollection<Movie> FavoriteMovies { get; set; }
            = new ObservableCollection<Movie>();

        public FavoritesForm()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += FavoritesForm_Loaded;
        }

        private async void FavoritesForm_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFavorites();
        }

        // =========================
        // OFFLINE-FIRST SYNC METHOD
        // =========================
        public async Task LoadFavorites()
        {
            try
            {
                // 1. SYNC FROM API → LOCAL DB
                if (await InternetHelper.HasInternet())
                {
                    string json = await ApiClient.Instance.GetStringAsync(
                        "http://185.246.222.35:8080/api/favorites/");

                    var movies = JsonSerializer.Deserialize<List<Movie>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    if (movies != null)
                    {
                        DatabaseService.SaveFavorites(movies);
                    }
                }

                // 2. LOAD FROM LOCAL DB (SOURCE OF TRUTH)
                var localFavorites = DatabaseService.GetLocalFavorites();

                FavoriteMovies.Clear();

                foreach (var movie in localFavorites)
                {
                    DatabaseService.ApplyLocalPoster(movie);
                    FavoriteMovies.Add(movie);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // =========================
        // UI CLICK
        // =========================
        private void MovieCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Border border &&
                    border.DataContext is Movie selectedMovie)
                {
                    var mainWindow = Window.GetWindow(this) as MainWindow;

                    mainWindow?.OpenMovieDetail(selectedMovie);
                }
            }
        }
    }
}