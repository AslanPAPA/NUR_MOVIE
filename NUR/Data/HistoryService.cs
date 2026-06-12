using System.Net.Http.Json;
using NUR.Models;

namespace NUR.Data
{
    public static class HistoryService
    {
        public static async Task AddToHistory(int movieId)
        {
            var data = new { movie_id = movieId };
            await ApiClient.Instance.PostAsJsonAsync(
                "http://185.246.222.35:8080/api/history/add/",
                data
            );
        }

        public static async Task<List<Movie>> GetHistory()
        {
            var response =
                await ApiClient.Instance.GetFromJsonAsync<List<Movie>>(
                    "http://185.246.222.35:8080/api/history/"
                );

            return response ?? new List<Movie>();
        }
    }
}