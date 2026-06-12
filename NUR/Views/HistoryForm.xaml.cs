using NUR.Data;
using NUR.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace NUR.Views
{
    public partial class HistoryForm : UserControl
    {
        public ObservableCollection<Movie> History { get; set; } = new();

        public HistoryForm()
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += HistoryForm_Loaded;
        }

        private async void HistoryForm_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadHistory();
        }

        private async Task LoadHistory()
        {
            try
            {
                List<Movie> list;

                if (await InternetHelper.HasInternet())
                {
                    list = await HistoryService.GetHistory();
                }
                else
                {
                    list = DatabaseService.GetLocalHistory();
                }

                History.Clear();

                foreach (var item in list)
                {
                    DatabaseService.ApplyLocalPoster(item);
                    History.Add(item);

                }

                emptyHistoryTxt.Visibility =
                        History.Count == 0
                            ? Visibility.Visible
                            : Visibility.Collapsed;
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки истории");
            }
        }

        private void WatchRightNowBtn_Click(object sender, RoutedEventArgs e)
        {
            var movie = (sender as FrameworkElement)?.DataContext as Movie;

            if (movie == null)
                return;

            (Window.GetWindow(this) as MainWindow)?.OpenMovieDetail(movie);
        }
    }
}