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

    public partial class HomeForm : UserControl
    {
        public HomeForm()
        {
            InitializeComponent();
        }

        private void ScrollLeft_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewer(CarouselList);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - 335);
            }
        }

        private void ScrollRight_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewer(CarouselList);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + 335);
            }
        }
        public static ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer scrollViewer) return scrollViewer;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }

        private void SelectMovie_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is TextBlock tb && tb.DataContext is Movie selectedMovie)
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
