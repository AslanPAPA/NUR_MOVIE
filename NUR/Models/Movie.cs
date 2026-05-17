using System.Linq;

namespace NUR.Models
{
    public class Genre { public string Name { get; set; } }
    public class Actor { public string Name { get; set; } }

    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public string Poster { get; set; }  
        public string Video_File { get; set; }  
        public string Country { get; set; }
        public string Age_Limit { get; set; }
        public string Director { get; set; }

        public List<Genre> Genres { get; set; }
        public List<Actor> Actors { get; set; }
        public string ActorsString => Actors != null ? string.Join(", ", Actors.Select(a => a.Name)) : "Не указаны";
    }
}