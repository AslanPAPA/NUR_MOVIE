using System.Net.Http;

namespace NUR.Services
{
    public static class ApiClient
    {
        private static readonly HttpClient _instance = new HttpClient();

        public static HttpClient Instance => _instance;
    }
}