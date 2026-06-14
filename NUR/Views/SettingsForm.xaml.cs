using System.Windows;
using System.Windows.Controls;
using NUR.Data;
using System.IO;

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

            // восстановление языка
            string savedLang = Properties.Settings.Default.Language;

            foreach (ComboBoxItem item in comboLang.Items)
            {
                if (item.Content.ToString() == savedLang)
                {
                    comboLang.SelectedItem = item;
                    break;
                }
            }

            SwitchLanguage(savedLang);

            // восстановление скорости (твоя логика)
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

        private void ClearCache_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string downloadsPath =
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Downloads");

                if (!Directory.Exists(downloadsPath))
                {
                    MessageBox.Show("Кэш уже пуст.");
                    return;
                }

                long totalBytes = 0;

                var files = Directory.GetFiles(
                    downloadsPath,
                    "*",
                    SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    FileInfo info = new FileInfo(file);
                    totalBytes += info.Length;
                }

                double totalGb =
                    totalBytes / 1024d / 1024d / 1024d;

                Directory.Delete(downloadsPath, true);

                MessageBox.Show(
                    $"Кэш очищен!\nУдалено: {totalGb:F2} GB");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка очистки: {ex.Message}");
            }
        }

        private void comboLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded)
                return;

            var item = comboLang.SelectedItem as ComboBoxItem;
            if (item == null)
                return;

            string lang = item.Content.ToString();

            Properties.Settings.Default.Language = lang;
            Properties.Settings.Default.Save();

            SwitchLanguage(lang);
        }

        private void SwitchLanguage(string lang)
        {
            string dictPath = lang switch
            {
                "Русский" => "Dictionary/Langs/Lang.ru.xaml",
                "Английский" => "Dictionary/Langs/Lang.en.xaml",
                _ => "Dictionary/Langs/Lang.ru.xaml"
            };

            var dict = new ResourceDictionary
            {
                Source = new Uri(dictPath, UriKind.Relative)
            };

            var oldDict = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null &&
                                     d.Source.OriginalString.Contains("Dictionary/Langs/"));

            if (oldDict != null)
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}