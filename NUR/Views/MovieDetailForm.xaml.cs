using NUR.Models;
using System.Windows;
using System.Windows.Controls;

namespace NUR.Views
{

    public partial class MovieDetailForm : UserControl
    {
        public MovieDetailForm()
        {
            InitializeComponent();
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

            if (movie != null && !string.IsNullOrEmpty(movie.VideoUrl))
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.StartPlayer(movie.VideoUrl);
                }
            }
            else
            {
                MessageBox.Show("Файл видео не найден или еще не загружен на сервер.", "Упс!");
            }
        }   
    }
}
