using System;
using System.IO;

namespace NUR.Data
{
    public static class DownloadManager
    {
        private static string DownloadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloads");

        public static string GetLocalPath(int movieId)
        {
            if (!Directory.Exists(DownloadFolder)) Directory.CreateDirectory(DownloadFolder);
            return Path.Combine(DownloadFolder, $"{movieId}.mp4");
        }

        public static bool IsDownloaded(int movieId)
        {
            return File.Exists(GetLocalPath(movieId));
        }
    }
}