using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using NUR.Data;

namespace NUR.Views
{
    public partial class DownloadWindow : Window
    {
        private Process _ffmpegProcess;

        private string _tempPath;
        private string _finalPath;

        public DownloadWindow(string hlsUrl, int movieId)
        {
            InitializeComponent();
            StartDownload(hlsUrl, movieId);
        }

        private void StartDownload(string hlsUrl, int movieId)
        {
            string ffmpegPath =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "FFmpeg",
                    "ffmpeg_prog",
                    "ffmpeg.exe");

            _finalPath = DownloadManager.GetLocalPath(movieId);

            // например:
            // 12.mp4 -> 12.tmp.mp4
            _tempPath =
                Path.Combine(
                    Path.GetDirectoryName(_finalPath)!,
                    Path.GetFileNameWithoutExtension(_finalPath) +
                    ".tmp.mp4");

            try
            {
                if (!File.Exists(ffmpegPath))
                {
                    MessageBox.Show(
                        $"FFmpeg не найден:\n{ffmpegPath}");
                    Close();
                    return;
                }

                if (File.Exists(_tempPath))
                    File.Delete(_tempPath);

                string arguments =
                    $"-i \"{hlsUrl}\" -c copy -bsf:a aac_adtstoasc \"{_tempPath}\"";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                _ffmpegProcess = Process.Start(psi);

                if (_ffmpegProcess == null)
                {
                    MessageBox.Show(
                        "Не удалось запустить FFmpeg.");
                    Close();
                    return;
                }

                _ffmpegProcess.EnableRaisingEvents = true;

                _ffmpegProcess.Exited += (s, e) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            if (_ffmpegProcess.ExitCode == 0)
                            {
                                if (File.Exists(_finalPath))
                                    File.Delete(_finalPath);

                                if (File.Exists(_tempPath))
                                {
                                    File.Move(
                                        _tempPath,
                                        _finalPath);
                                }

                                DownloadPanel.Visibility =
                                    Visibility.Collapsed;

                                FinishPanel.Visibility =
                                    Visibility.Visible;
                            }
                            else
                            {
                                if (File.Exists(_tempPath))
                                    File.Delete(_tempPath);

                                MessageBox.Show(
                                    "Ошибка загрузки фильма.");

                                Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                $"Ошибка:\n{ex.Message}");
                        }
                    });
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка запуска:\n{ex.Message}");

                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                if (_ffmpegProcess != null &&
                    !_ffmpegProcess.HasExited)
                {
                    _ffmpegProcess.Kill();
                    _ffmpegProcess.WaitForExit();
                }

                if (!string.IsNullOrEmpty(_tempPath) &&
                    File.Exists(_tempPath))
                {
                    File.Delete(_tempPath);
                }
            }
            catch
            {
            }

            base.OnClosed(e);
        }

        private void BtnOk_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}