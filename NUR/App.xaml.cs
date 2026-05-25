using System;
using System.IO;
using System.Windows;
using FlyleafLib;

namespace NUR
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FFmpeg");
            Engine.Start(new EngineConfig()
            {
                FFmpegPath = ffmpegPath,
                UIRefresh = true,
                UIRefreshInterval = 250
            });
        }
    }
}