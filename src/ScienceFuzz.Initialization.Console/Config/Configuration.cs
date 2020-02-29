using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScienceFuzz.Initialization.Console.Config
{
    public class Configuration
    {
        public string StorageConnection { get; set; }

        public static async Task<Configuration> LoadFromFileAsync(string filePath)
        {
            Configuration config;
            using (var stream = File.OpenRead(@"Config/configuration.json"))
            {
                config = await JsonSerializer.DeserializeAsync<Configuration>(stream,
                   new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return config;
        }
    }
}
