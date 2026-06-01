using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization; // Обязательно добавляем для атрибутов

namespace NUR.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [JsonPropertyName("release_year")] // Маппим release_year из JSON в Year
        public int Year { get; set; }

        public string Poster { get; set; }

        [JsonPropertyName("video_file")] // На всякий случай маппим точное имя с сервера
        public string VideoFile { get; set; }

        [JsonPropertyName("video_url")]
        public string VideoUrl { get; set; }

        public string Country { get; set; }

        [JsonPropertyName("age_rating")] // Маппим age_rating из JSON в Age_Limit
        public string Age_Limit { get; set; }



        public string Director { get; set; } // В Django пока нет этого поля, вернет null - это ок

        public List<Genre> Genres { get; set; }
        public List<Actor> Actors { get; set; }

        public string ActorsString => Actors != null && Actors.Any()
            ? string.Join(", ", Actors.Select(a => a.Name))
            : "Не указаны";
    }
}