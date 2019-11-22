
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
            await SeedScientistsAsync(tableClient);
            await SeedPublicationsAsync(tableClient);
            await SeedDisciplineContributionsAsync(tableClient);
            //await SeedDomainContributionsAsync(tableClient);
        }

        static async Task SeedScientistsAsync(CloudTableClient tableClient)
        {
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

        static async Task SeedPublicationsAsync(CloudTableClient tableClient)
        {
            var publicatonsTable = tableClient.GetTableReference(Constants.StorageTableNames.Publications);

            List<PublicationCsvModel> publicationCsvModels;
            using (var reader = new StreamReader(@"Data\publications.csv"))
            using (var csv = new CsvReader(reader))
            {
                publicationCsvModels = csv.GetRecords<PublicationCsvModel>().ToList();
            }

            var publicationGroups = publicationCsvModels.Select(x => new Publication
            {
                PartitionKey = x.Author,
                RowKey = x.Journal,
                Count = x.Count
            }).GroupBy(x => x.PartitionKey);

            foreach (var group in publicationGroups)
            {
                var batch = new TableBatchOperation();
                foreach (var scientist in group)
                {
                    batch.Add(TableOperation.Insert(scientist));
                }
                await publicatonsTable.ExecuteBatchAsync(batch);
            }
        }

        static async Task SeedDisciplineContributionsAsync(CloudTableClient tableClient)
        {
            var disciplineContributionsTable = tableClient.GetTableReference(Constants.StorageTableNames.DisciplineContributions);

            List<DisciplineContributionCsvModel> contributionsCsvModels;
            using (var reader = new StreamReader(@"Data\disciplineContributions.csv"))
            using (var csv = new CsvReader(reader))
            {
                contributionsCsvModels = csv.GetRecords<DisciplineContributionCsvModel>().ToList();
            }

            //var d = new Dictionary<string, int>();
            //var err = contributionsCsvModels.Select(x => x.Journal);
            //foreach (var e in err)
            //{
            //    if (!d.ContainsKey(e))
            //    {
            //        d[e] = 1;
            //    }
            //    else
            //    {
            //        d[e] = d[e] + 1;
            //    }
            //}

            //var elo = d.Where(x => x.Value > 1).ToList();

            //var list = new List<DisciplineContributionCsvModel>();
            //foreach (var contrib in contributionsCsvModels)
            //{
            //    var dic = new Dictionary<string, int>();
            //    foreach (var disc in contrib.Disciplines)
            //    {
            //        if (!dic.ContainsKey(disc))
            //        {
            //            dic[disc] = 1;
            //        }
            //        else
            //        {
            //            dic[disc] = dic[disc] + 1;
            //        }
            //    }
            //    if (dic.Any(x => x.Value > 1))
            //    {
            //        list.Add(contrib);
            //    }
            //}

            //var d = list;
            var batches = new List<TableBatchOperation>();
            foreach (var contribution in contributionsCsvModels)
            {
                var batch = new TableBatchOperation();
                foreach (var discipline in contribution.Disciplines)
                {
                    batch.Add(TableOperation.Insert(new DisciplineContribution
                    {
                        PartitionKey = contribution.Journal,
                        RowKey = discipline
                    }));
                }
                batches.Add(batch);
            }

            System.Console.WriteLine("Uploading discipline batches...");
            var count = 0;
            foreach (var batch in batches)
            {
                try
                {
                    await disciplineContributionsTable.ExecuteBatchAsync(batch);
                    count += batch.Count;
                    System.Console.WriteLine($"UPLOADED BATCHES: {count}");
                }
                catch (System.Exception e)
                {
                    throw;
                }
            }

            //Parallel.ForEach(batches, async x => await disciplineContributionsTable.ExecuteBatchAsync(x));

            //foreach (var operation in operations)
            //{
            //    try
            //    {
            //        await disciplineContributionsTable.ExecuteAsync(operation);
            //        i++;
            //    }
            //    catch (System.Exception e)
            //    {
            //        var t = e;
            //        throw;
            //    }

            //    System.Console.WriteLine($"INSERTED [{i}] records so far...");
            //}
        }

        static async Task SeedDomainContributionsAsync(CloudTableClient tableClient)
        {
            var domainContributionsTable = tableClient.GetTableReference(Constants.StorageTableNames.DomainContributions);

            List<DomainContributionCsvModel> contributionsCsvModels;
            using (var reader = new StreamReader(@"Data\domainContributions.csv"))
            using (var csv = new CsvReader(reader))
            {
                contributionsCsvModels = csv.GetRecords<DomainContributionCsvModel>().ToList();
            }

            var operations = new List<TableOperation>();
            foreach (var contribution in contributionsCsvModels)
            {
                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = Constants.ScienceDomainNames.Humanities,
                    Count = contribution.Humanities
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = Constants.ScienceDomainNames.Technology,
                    Count = contribution.Technology
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = Constants.ScienceDomainNames.Medical,
                    Count = contribution.Medical
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = Constants.ScienceDomainNames.Farm,
                    Count = contribution.Farm
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = Constants.ScienceDomainNames.Social,
                    Count = contribution.Social
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = Constants.ScienceDomainNames.Science,
                    Count = contribution.Science
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = Constants.ScienceDomainNames.Religion,
                    Count = contribution.Religion
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = Constants.ScienceDomainNames.Arts,
                    Count = contribution.Arts
                }));
            }

            var groups = operations.GroupBy(x => x.Entity.PartitionKey);
            foreach (var group in groups)
            {
                var batch = new TableBatchOperation();
                foreach (var operation in group)
                {
                    batch.Add(operation);
                }
                await domainContributionsTable.ExecuteBatchAsync(batch);
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
