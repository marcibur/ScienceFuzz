using Microsoft.Azure.Cosmos.Table;
using ScienceFuzz.Initialization.Console.Config;
using ScienceFuzz.Initialization.Console.Extensions;
using System.Threading.Tasks;

namespace ScienceFuzz.Initialization.Console.Logic
{
    public static class Cache
    {
        public static async Task RecreateAsync(Configuration config)
        {
            var tableClient = CloudStorageAccount.Parse(config.CacheConnection).CreateCloudTableClient();
            await tableClient.RecreateTableAsync(Constants.CacheTableNames.DiscipilneContributions);
            await tableClient.RecreateTableAsync(Constants.CacheTableNames.DomainContributions);
            await tableClient.RecreateTableAsync(Constants.CacheTableNames.Tsne);
            await tableClient.RecreateTableAsync(Constants.CacheTableNames.Kmeans);
        }
    }
}
