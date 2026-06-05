using NUR.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NUR.Data
{
    public static class PosterManager
    {
        public static async Task DownloadPoster(Movie movie)
        {
            try
            {
                string folder = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Posters");

                Directory.CreateDirectory(folder);

                string localPath =
                    Path.Combine(folder, $"{movie.Id}.jpg");

                if (File.Exists(localPath))
                    return;

                using HttpClient client = new();

                byte[] bytes =
                    await client.GetByteArrayAsync(movie.Poster);

                await File.WriteAllBytesAsync(localPath, bytes);
            }
            catch
            {
            }
        }

        public static string GetLocalPoster(int movieId)
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Posters",
                $"{movieId}.jpg");
        }
    }
}