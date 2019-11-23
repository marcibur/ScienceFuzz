using ScienceFuzz.Initialization.Console.Config;
using ScienceFuzz.Initialization.Console.Logic;
using System.Threading.Tasks;

namespace ScienceFuzz.Initialization.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = await Configuration.LoadFromFileAsync(@"Config/configuration.json");
            await Storage.RecreateAsync(config);
            await Cache.RecreateAsync(config);
            await Seed.SeedAsync(config);
        }
    }
}
