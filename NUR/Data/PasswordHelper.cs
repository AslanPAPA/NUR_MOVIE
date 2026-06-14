using Isopoh.Cryptography.Argon2;

namespace NUR.Data
{
    public static class PasswordHelper
    {
        // HASH
        public static string HashPassword(string password)
        {
            return Argon2.Hash(password);
        }

        // VERIFY
        public static bool VerifyPassword(
            string password,
            string hash)
        {
            return Argon2.Verify(hash, password);
        }
    }
}