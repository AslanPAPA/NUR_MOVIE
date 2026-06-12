using System.Net.Http;
using System.Net.Http.Headers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace NUR.Data
{
    public class ApiClient
    {
        private static readonly HttpClient _client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(8)
        };

        public static string Token { get; set; }


        public static void SetToken(string token, string username) 
        {
            Token = token;
            UserSession.Username = username;
            NUR.Properties.Settings.Default.AuthToken = token;
            NUR.Properties.Settings.Default.Username = username; 
            NUR.Properties.Settings.Default.Save();
        }

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