using ScienceFuzz.Http.Client.Extensions;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScienceFuzz.Http.Client
{
    public class ScienceFuzzClient
    {
        private readonly HttpClient _http;
        private readonly string _uriBase;

        public ScienceFuzzClient(HttpClient http, string uriBase)
        {
            _http = http;
            _uriBase = uriBase;
        }

        public async Task<string[]> GetScientistNamesAsync()
        {
            var scientistNames = await _http.GetJsonAsync<string[]>($"{_uriBase}/Scientists/Names");
            return scientistNames;
        }
    }
}
