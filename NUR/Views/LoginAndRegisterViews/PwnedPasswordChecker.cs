using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace NUR.Views.LoginAndRegisterViews
{
    public static class PwnedPasswordChecker
    {
        private static readonly HttpClient _http = new HttpClient();

        public static async Task<bool> IsPasswordPwned(string password)
        {
            string sha1 = ComputeSha1(password);
            string prefix = sha1.Substring(0, 5);
            string suffix = sha1.Substring(5).ToUpper();

            var url = $"https://api.pwnedpasswords.com/range/{prefix}";

            _http.DefaultRequestHeaders.UserAgent.ParseAdd("MyAppPasswordCheck");

            var response = await _http.GetStringAsync(url);

            var lines = response.Split('\n');

            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length != 2) continue;

                string hashSuffix = parts[0].Trim();

                if (hashSuffix == suffix)
                {
                    return true; // пароль найден в утечках
                }
            }

            return false;
        }

        private static string ComputeSha1(string input)
        {
            using var sha1 = SHA1.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha1.ComputeHash(bytes);

            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}
