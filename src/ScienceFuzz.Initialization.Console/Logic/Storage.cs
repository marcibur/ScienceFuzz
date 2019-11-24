using Microsoft.WindowsAzure.Storage;
using ScienceFuzz.Initialization.Console.Config;
using ScienceFuzz.Initialization.Console.Extensions;
using System.Threading.Tasks;

namespace ScienceFuzz.Initialization.Console.Logic
{
    public class Storage
    {
        public static async Task RecreateAsync(Configuration config)
        {
            var tableClient = CloudStorageAccount.Parse(config.StorageConnection).CreateCloudTableClient();
            await tableClient.RecreateTableAsync(CONST.STORAGE_TABLE_NAMES.SCIENTISTS);
            //await tableClient.RecreateTableAsync(CONST.STORAGE_TABLE_NAMES.PUBLICATIONS);
            //await tableClient.RecreateTableAsync(CONST.STORAGE_TABLE_NAMES.DISCIPLINE_CONTRIBUTIONS);
            //await tableClient.RecreateTableAsync(CONST.STORAGE_TABLE_NAMES.DOMAIN_CONTRIBUTIONS);

            //var blobClient = CloudStorageAccount.Parse(config.StorageConnection).CreateCloudBlobClient();
            //await blobClient.RecreateContainerAsync(CONST.STORAGE_CONTAINER_NAMES.DISCIPLINES);
        }
    }
}
