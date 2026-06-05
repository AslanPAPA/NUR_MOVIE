using FlyleafLib;
using NUR.Data;
using NUR.Views;
using System.IO;
using System.Windows;

namespace NUR
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DatabaseService.Initialize();

            try
            {
                string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FFmpeg");

                Engine.Start(new EngineConfig()
                {
                    FFmpegPath = ffmpegPath,
                    UIRefresh = true,
                    UIRefreshInterval = 250
                });

                string savedToken = NUR.Properties.Settings.Default.AuthToken;
                string savedUsername = NUR.Properties.Settings.Default.Username;

                if (!string.IsNullOrEmpty(savedToken))
                {
                    ApiClient.Token = savedToken;
                    UserSession.Username = savedUsername;

                    MainWindow mainWin = new MainWindow();
                    this.MainWindow = mainWin;
                    mainWin.Show();
                }
                else
                {
                    LoginReg loginWin = new LoginReg();
                    this.MainWindow = loginWin;
                    loginWin.Show();
                }

                

                string downloadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloads");
                if (!Directory.Exists(downloadPath))
                {
                    Directory.CreateDirectory(downloadPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка старта приложения: {ex.Message}");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            NUR.Properties.Settings.Default.Save();
            base.OnExit(e);
        }
    }
}