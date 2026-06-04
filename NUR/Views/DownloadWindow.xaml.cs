using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using NUR.Data;



namespace NUR.Views
{
    public partial class DownloadWindow : Window
    {
        public DownloadWindow(string hlsUrl, int movieId)
        {
            InitializeComponent();
            StartDownload(hlsUrl, movieId);
        }

        private void StartDownload(string hlsUrl, int movieId)
        {
            string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FFmpeg/ffmpeg_prog", "ffmpeg.exe");
            string outputPath = DownloadManager.GetLocalPath(movieId);
            string arguments = $"-i \"{hlsUrl}\" -c copy -bsf:a aac_adtstoasc \"{outputPath}\"";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                Process proc = Process.Start(psi);
                proc.EnableRaisingEvents = true;
                proc.Exited += (s, e) => {
                    Dispatcher.Invoke(() => {
                        DownloadPanel.Visibility = Visibility.Collapsed;
                        FinishPanel.Visibility = Visibility.Visible;
                    });
                };
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка: " + ex.Message);
                this.Close();
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}