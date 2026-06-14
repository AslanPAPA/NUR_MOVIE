using System.Windows;
using System.Windows.Controls;

namespace NUR.Views
{
    public partial class SettingsForm : UserControl
    {
        private bool _isLoaded = false;

        public SettingsForm()
        {
            InitializeComponent();
            Loaded += settingsFrom_Loaded;
        }

        private void settingsFrom_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = false;

            foreach (ComboBoxItem item in videoSPeedBox.Items)
            {
                if (item.Content.ToString() == Properties.Settings.Default.VideoSpeed)
                {
                    videoSPeedBox.SelectedItem = item;
                    break;
                }
            }

            _isLoaded = true;
        }

        private void videoSPeedBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded)
                return;

            var item = videoSPeedBox.SelectedItem as ComboBoxItem;
            if (item == null)
                return;

            Properties.Settings.Default.VideoSpeed = item.Content.ToString();
            Properties.Settings.Default.Save();
        }
    }
}