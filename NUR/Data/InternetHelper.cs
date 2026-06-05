using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NUR.Data
{
    public static class InternetHelper
    {


        public static async Task<bool> HasInternet()
        {
            try
            {
                using HttpClient client = new();

                client.Timeout = TimeSpan.FromSeconds(3);

                await client.GetAsync(
                    "http://185.246.222.35:8080/api/movies/");

                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
