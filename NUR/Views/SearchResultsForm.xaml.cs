using NUR.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NUR.Views
{
    public partial class SearchResultsForm : UserControl
    {
        public SearchResultsForm()
        {
            InitializeComponent();
        }

        public void SetMovies(List<Movie> movies, string query)
        {
            ResultsList.ItemsSource = movies;

            if (movies.Count == 0)
            {
                NoResultsText.Text =
                    $"Ничего не найдено по запросу:\n\"{query}\"";

                NoResultsText.Visibility = Visibility.Visible;
            }
            else
            {
                NoResultsText.Visibility = Visibility.Collapsed;
            }
        }

        private void SelectMovie_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border &&
                border.DataContext is Movie movie)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.OpenMovieDetail(movie);
            }
        }
    }
}