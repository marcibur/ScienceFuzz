using CsvHelper;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;
using ScienceFuzz.Data;
using ScienceFuzz.Initialization.Console.Config;
using ScienceFuzz.Initialization.Console.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ScienceFuzz.Initialization.Console.Logic
{
    public static class Seed
    {
        public static async Task SeedAsync(Configuration config)
        {
            var tableClient = CloudStorageAccount.Parse(config.StorageConnection).CreateCloudTableClient();
            await SeedScientistsAsync(tableClient);
            await SeedPublicationsAsync(tableClient);
            await SeedDisciplineContributionsAsync(tableClient);
            await SeedDomainContributionsAsync(tableClient);

            var blobClient = Microsoft.Azure.Storage.CloudStorageAccount.Parse(config.StorageConnection).CreateCloudBlobClient();
            await SeedDisciplineListAsync(blobClient);
        }

        static async Task SeedScientistsAsync(CloudTableClient tableClient)
        {
            System.Console.WriteLine("Seeding scientists...");
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

            System.Console.WriteLine("Scientists seeded successfully.");
        }

        static async Task SeedPublicationsAsync(CloudTableClient tableClient)
        {
            System.Console.WriteLine("Seeding publications...");
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

            System.Console.WriteLine("Publications seeded successfully.");
        }

        static async Task SeedDisciplineContributionsAsync(CloudTableClient tableClient)
        {
            System.Console.WriteLine("Seeding disciplines...");
            var disciplineContributionsTable = tableClient.GetTableReference(Constants.StorageTableNames.DisciplineContributions);

            List<DisciplineContributionCsvModel> contributionsCsvModels;
            using (var reader = new StreamReader(@"Data\disciplines.csv"))
            using (var csv = new CsvReader(reader))
            {
                contributionsCsvModels = csv.GetRecords<DisciplineContributionCsvModel>().ToList();
            }

            var count = 0;
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
                await disciplineContributionsTable.ExecuteBatchAsync(batch);
                count += batch.Count;
                System.Console.WriteLine($"Disciplines seeded: {count}");
            }

            System.Console.WriteLine("Disciplines seeded successfully.");
        }

        static async Task SeedDomainContributionsAsync(CloudTableClient tableClient)
        {
            System.Console.WriteLine("Seeding domains...");
            var domainContributionsTable = tableClient.GetTableReference(Constants.StorageTableNames.DomainContributions);

            List<DomainContributionCsvModel> contributionsCsvModels;
            using (var reader = new StreamReader(@"Data\domains.csv"))
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

            var temp = groups.Where(x => x.Count() > 8).Select(x => x.Key).ToList();

            var count = 0;
            foreach (var group in groups)
            {
                var batch = new TableBatchOperation();
                foreach (var operation in group)
                {
                    batch.Add(operation);
                }
                await domainContributionsTable.ExecuteBatchAsync(batch);
                count += batch.Count;
                System.Console.WriteLine($"Domains seeded: {count}");
            }

            System.Console.WriteLine("Domains seeded successfully.");
        }

        private static async Task SeedDisciplineListAsync(CloudBlobClient blobClient)
        {
            System.Console.WriteLine("Seeding discipline list...");
            var container = blobClient.GetContainerReference(Constants.StorageContainerNames.Disciplines);
            var blob = container.GetBlockBlobReference(Constants.FileNames.DisciplineList);
            await blob.UploadFromFileAsync(@$"Data\{Constants.FileNames.DisciplineList}");
            System.Console.WriteLine("Discipline list seeded successfully.");
        }
    }
}
