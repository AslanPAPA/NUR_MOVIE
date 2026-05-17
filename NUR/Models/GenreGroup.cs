using System;
using System.Collections.Generic;
using System.Text;

namespace NUR.Models
{
    public class GenreGroup
    {
        public string GenreName { get; set; }
        public List<Movie> Movies { get; set; }
    }
}
