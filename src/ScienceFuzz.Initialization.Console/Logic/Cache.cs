using Microsoft.WindowsAzure.Storage;
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
            await tableClient.RecreateTableAsync(CONST.CACHE_TABLE_NAMES.DISCIPLINE_CONTRIBUTIONS);
            await tableClient.RecreateTableAsync(CONST.CACHE_TABLE_NAMES.DOMAIN_CONTRIBUTIONS);
            await tableClient.RecreateTableAsync(CONST.CACHE_TABLE_NAMES.TSNE);
            await tableClient.RecreateTableAsync(CONST.CACHE_TABLE_NAMES.KMEANS);
        }
    }
}
