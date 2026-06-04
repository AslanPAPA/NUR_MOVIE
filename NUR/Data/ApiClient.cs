using System.Net.Http;
using System.Net.Http.Headers;

namespace NUR.Data
{
    public class ApiClient
    {
        private static readonly HttpClient _client =
            new HttpClient();

        public static string Token { get; set; }

        public static HttpClient Instance
        {
            get
            {
                _client.DefaultRequestHeaders.Authorization = null;

                if (!string.IsNullOrEmpty(Token))
                {
                    _client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Token",
                            Token
                        );
                }

                return _client;
            }
        }
    }
}