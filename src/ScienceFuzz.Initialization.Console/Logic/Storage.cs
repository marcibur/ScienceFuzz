using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;
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
            await tableClient.RecreateTableAsync(Constants.StorageTableNames.Scientists);
            await tableClient.RecreateTableAsync(Constants.StorageTableNames.Publications);
            await tableClient.RecreateTableAsync(Constants.StorageTableNames.DisciplineContributions);
            await tableClient.RecreateTableAsync(Constants.StorageTableNames.DomainContributions);

            var blobClient = Microsoft.Azure.Storage.CloudStorageAccount.Parse(config.StorageConnection).CreateCloudBlobClient();
            await blobClient.RecreateContainerAsync(Constants.StorageContainerNames.Disciplines);
        }
    }
}
