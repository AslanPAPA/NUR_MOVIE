using System.Net.Http;
using System.Threading.Tasks;

public static class InternetHelper
{
    private static readonly HttpClient client = new()
    {
        Timeout = TimeSpan.FromSeconds(8)
    };

    private static readonly string[] Urls =
    {
        "https://clients3.google.com/generate_204",
        "https://www.msftconnecttest.com/connecttest.txt",
        "https://1.1.1.1"
    };

    public static async Task<bool> HasInternet()
    {
        foreach (var url in Urls)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Head, url);

                using var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                    return true;
            }
            catch
            {
            }
        }

        return false;
    }
}