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

        private async void FavoritesForm_Loaded(
                  object sender,
                  System.Windows.RoutedEventArgs e)
        {
            await LoadFavoritesFromApi();
        }

        public async void LoadFavorites()
        {
            await LoadFavoritesFromApi();
        }

        private async Task LoadFavoritesFromApi()
        {
            FavoriteMovies.Clear();

            try
            {
                string json =
                    await ApiClient.Instance.GetStringAsync(
                        "http://185.246.222.35:8080/api/favorites/");

                var movies = JsonSerializer.Deserialize<List<Movie>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                foreach (var movie in movies)
                {
                    FavoriteMovies.Add(movie);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Favorites error: " + ex.Message);
            }
        }

        private void MovieCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Border border &&
                    border.DataContext is Movie selectedMovie)
                {
                    var mainWindow = Window.GetWindow(this) as MainWindow;

                    if (mainWindow != null)
                    {
                        mainWindow.OpenMovieDetail(selectedMovie);
                    }
                }
            }
        }
    }
}