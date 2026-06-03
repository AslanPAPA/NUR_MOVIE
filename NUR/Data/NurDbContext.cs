using Microsoft.EntityFrameworkCore;
using NUR.Models;

namespace NUR.Data
{
    public class NurDbContext : DbContext
    {
        // Это и есть твоя таблица в базе данных
        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // ЗАМЕНИ НА СВОЮ СТРОКУ ПОДКЛЮЧЕНИЯ
            // Скопируй её из свойств твоего подключения в SQL Server Management Studio
            string connectionString = @"Data Source=185.246.222.35,1433;Initial Catalog=NurDB;User ID=sa;Password=Aslan_2006_06;TrustServerCertificate=True;Connect Timeout=30;";
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Указываем, что модель AppUser соответствует таблице app_users в базе
            modelBuilder.Entity<AppUser>().ToTable("app_users");
        }
    }
}