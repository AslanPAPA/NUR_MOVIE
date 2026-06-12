using NUR.Data;
using NUR.Models;
using NUR.Services;
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

            HistorySyncService.HistoryChanged += async () =>
            {
                await Dispatcher.InvokeAsync(LoadHistory);
            };
        }

        private async void HistoryForm_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadHistory();
        }

        private async Task LoadHistory()
        {
            try
            {
                List<Movie> list = new();

                // =========================
                // ONLINE → SYNC FIRST
                // =========================
                if (await InternetHelper.HasInternet())
                {
                    var serverList = await HistoryService.GetHistory();

                    foreach (var movie in serverList)
                    {
                        DatabaseService.SaveHistory(movie);
                    }
                }

                // =========================
                // ALWAYS LOAD LOCAL (SOURCE OF TRUTH)
                // =========================
                list = DatabaseService.GetLocalHistory();

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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