using NUR.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            if (movie != null && !string.IsNullOrEmpty(movie.Video_File))
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    string fullUrl = movie.Video_File.StartsWith("http")
                        ? movie.Video_File
                        : $"http://127.0.0.1:8000{movie.Video_File}";

                    mainWindow.StartPlayer(fullUrl);
                }
            }
            else
            {
                MessageBox.Show("Файл видео не найден или еще не загружен на сервер.", "Упс!");
            }
        }
    }
}
