
using CsvHelper;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;
using ScienceFuzz.Data;
using ScienceFuzz.Initialization.Console.Config;
using ScienceFuzz.Initialization.Console.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScienceFuzz.Initialization.Console
{
    class Program
    {
        private static Configuration _config;

        static async Task Main(string[] args)
        {
            await LoadConfigurationAsync();
            await DropCreateStorageAsync();
            await DropCreateCacheAsync();
            await SeedDataAsync();
        }

        static async Task LoadConfigurationAsync()
        {
            using (var stream = File.OpenRead(@"Config/configuration.json"))
            {
                _config = await JsonSerializer.DeserializeAsync<Configuration>(stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }

        static async Task DropCreateStorageAsync()
        {
            var tableClient = CloudStorageAccount.Parse(_config.StorageConnection).CreateCloudTableClient();
            await RecreateTableAsync(tableClient, Constants.StorageTableNames.Scientists);
            await RecreateTableAsync(tableClient, Constants.StorageTableNames.Publications);
            await RecreateTableAsync(tableClient, Constants.StorageTableNames.DisciplineContributions);
            await RecreateTableAsync(tableClient, Constants.StorageTableNames.DomainContributions);

            var blobClient = Microsoft.Azure.Storage.CloudStorageAccount.Parse(_config.StorageConnection).CreateCloudBlobClient();
            await RecreateContainerAsync(blobClient, Constants.StorageContainerNames.Disciplines);
            await RecreateContainerAsync(blobClient, Constants.StorageContainerNames.Domains);
        }

        static async Task DropCreateCacheAsync()
        {
            var tableClient = CloudStorageAccount.Parse(_config.CacheConnection).CreateCloudTableClient();
            await RecreateTableAsync(tableClient, Constants.CacheTableNames.DiscipilneContributions);
            await RecreateTableAsync(tableClient, Constants.CacheTableNames.DomainContributions);
            await RecreateTableAsync(tableClient, Constants.CacheTableNames.Tsne);
            await RecreateTableAsync(tableClient, Constants.CacheTableNames.Kmeans);
        }

        static async Task SeedDataAsync()
        {
            var tableClient = CloudStorageAccount.Parse(_config.StorageConnection).CreateCloudTableClient();
            var scientistsTable = tableClient.GetTableReference(Constants.StorageTableNames.Scientists);

            List<ScientistCsvModel> scientistCsvModels;
            using (var reader = new StreamReader(@"Data\scientists.csv"))
            using (var csv = new CsvReader(reader))
            {
                scientistCsvModels = csv.GetRecords<ScientistCsvModel>().ToList();
            }

            var scientistGroups = scientistCsvModels.Select(x => new Scientist
            {
                PartitionKey = x.Unit,
                RowKey = x.Name
            }).GroupBy(x => x.PartitionKey);

            var batches = new List<TableBatchOperation>();
            foreach (var group in scientistGroups)
            {
                var batch = new TableBatchOperation();
                foreach (var scientist in group)
                {
                    batch.Add(TableOperation.Insert(scientist));
                }
                await scientistsTable.ExecuteBatchAsync(batch);
            }
        }

        static async Task RecreateTableAsync(CloudTableClient tableClient, string tableName)
        {
            var table = tableClient.GetTableReference(tableName);
            await table.DeleteIfExistsAsync();
            await table.CreateAsync();
        }

        static async Task RecreateContainerAsync(CloudBlobClient blobClient, string containerName)
        {
            var container = blobClient.GetContainerReference(containerName);
            await container.DeleteIfExistsAsync();
            await container.CreateAsync();
        }
    }
}
