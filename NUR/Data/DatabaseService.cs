using Microsoft.Data.Sqlite;
using NUR.Models;
using System.Text.Json;

namespace NUR.Data
{
    public static class DatabaseService
    {
        private static readonly string DbPath = "nur.db";

        public static void Initialize()
        {
            using var connection =
                new SqliteConnection($"Data Source={DbPath}");

            connection.Open();

            string sql = @"
CREATE TABLE IF NOT EXISTS Movies
(
    Id INTEGER PRIMARY KEY,
    Title TEXT,
    Description TEXT,
    Year INTEGER,
    PosterUrl TEXT,
    VideoUrl TEXT,
    Genres TEXT,
    Actors TEXT
);";

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        public static void SaveMovies(List<Movie> movies)
        {
            using var connection =
                new SqliteConnection($"Data Source={DbPath}");

            connection.Open();

            foreach (var movie in movies)
            {
                string sql = @"
INSERT OR REPLACE INTO Movies
(
    Id,
    Title,
    Description,
    Year,
    PosterUrl,
    VideoUrl,
    Genres,
    Actors
)
VALUES
(
    @Id,
    @Title,
    @Description,
    @Year,
    @PosterUrl,
    @VideoUrl,
    @Genres,
    @Actors
)";

                using var cmd = new SqliteCommand(sql, connection);

                cmd.Parameters.AddWithValue("@Id", movie.Id);
                cmd.Parameters.AddWithValue("@Title", movie.Title ?? "");
                cmd.Parameters.AddWithValue("@Description", movie.Description ?? "");
                cmd.Parameters.AddWithValue("@Year", movie.Year);
                cmd.Parameters.AddWithValue("@PosterUrl", movie.Poster ?? "");
                cmd.Parameters.AddWithValue("@VideoUrl", movie.VideoUrl ?? "");
                cmd.Parameters.AddWithValue(
    "@Genres",
    JsonSerializer.Serialize(movie.Genres ?? new List<Genre>()));

                cmd.Parameters.AddWithValue(
                    "@Actors",
                    JsonSerializer.Serialize(movie.Actors ?? new List<Actor>()));

                cmd.ExecuteNonQuery();
            }
        }


public static List<Movie> LoadMovies()

        {

            List<Movie> movies = new();



            using var connection =

                new SqliteConnection($"Data Source={DbPath}");



            connection.Open();



            string sql = "SELECT * FROM Movies";



            using var cmd = new SqliteCommand(sql, connection);

            using var reader = cmd.ExecuteReader();



            while (reader.Read())

            {

                movies.Add(new Movie
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    Year = reader.GetInt32(3),
                    Poster = reader.GetString(4),
                    VideoUrl = reader.GetString(5),

                    Genres = !reader.IsDBNull(6)
        ? JsonSerializer.Deserialize<List<Genre>>(reader.GetString(6)) ?? new()
        : new(),

                    Actors = !reader.IsDBNull(7)
        ? JsonSerializer.Deserialize<List<Actor>>(reader.GetString(7)) ?? new()
        : new()
                });

            }



            return movies;

        }
    }
}