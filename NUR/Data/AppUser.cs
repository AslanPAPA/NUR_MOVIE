using System.ComponentModel.DataAnnotations;

namespace NUR.Data
{
    public class AppUser
    {
        [Key] // Указываем, что id - это первичный ключ
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Тут будет лежать хэш
        public string? Email { get; set; }
    }
}