using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScienceFuzz.Http.Client.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetJsonAsync<T>(this HttpClient http, string uri)
        {
            T result;
            using (var jsonStream = await http.GetStreamAsync(new Uri(uri)))
            {
                result = await JsonSerializer.DeserializeAsync<T>(jsonStream);
            }
            return result;
        }
    }
}
