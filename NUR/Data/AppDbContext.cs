using Microsoft.EntityFrameworkCore;
using NUR.Models;

namespace NUR.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<AppUser> Users { get; set; }

       
    }
}