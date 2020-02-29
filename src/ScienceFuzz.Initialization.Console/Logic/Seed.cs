using CsvHelper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
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
            var publications = await SeedPublicationsAsync(tableClient);
            await SeedDisciplineContributionsAsync(tableClient, publications);
            await SeedDomainContributionsAsync(tableClient, publications);

            var blobClient = CloudStorageAccount.Parse(config.StorageConnection).CreateCloudBlobClient();
            await SeedDisciplineListAsync(blobClient);
        }

        static async Task SeedScientistsAsync(CloudTableClient tableClient)
        {
            System.Console.WriteLine("Seeding scientists...");
            var scientistsTable = tableClient.GetTableReference(CONST.STORAGE_TABLE_NAMES.SCIENTISTS);

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

        static async Task<IEnumerable<string>> SeedPublicationsAsync(CloudTableClient tableClient)
        {
            System.Console.WriteLine("Seeding publications...");
            var publicatonsTable = tableClient.GetTableReference(CONST.STORAGE_TABLE_NAMES.PUBLICATIONS);

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
            return publicationCsvModels.Select(x => x.Journal).Distinct().ToList();
        }

        static async Task SeedDisciplineContributionsAsync(CloudTableClient tableClient, IEnumerable<string> publications)
        {
            System.Console.WriteLine("Seeding disciplines...");
            var disciplineContributionsTable = tableClient.GetTableReference(CONST.STORAGE_TABLE_NAMES.DISCIPLINE_CONTRIBUTIONS);

            List<DisciplineContributionCsvModel> contributionsCsvModels;
            using (var reader = new StreamReader(@"Data\disciplines.csv"))
            using (var csv = new CsvReader(reader))
            {
                contributionsCsvModels = csv.GetRecords<DisciplineContributionCsvModel>().Where(x => publications.Contains(x.Journal)).ToList();
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

        static async Task SeedDomainContributionsAsync(CloudTableClient tableClient, IEnumerable<string> publications)
        {
            System.Console.WriteLine("Seeding domains...");
            var domainContributionsTable = tableClient.GetTableReference(CONST.STORAGE_TABLE_NAMES.DOMAIN_CONTRIBUTIONS);

            List<DomainContributionCsvModel> contributionsCsvModels;
            using (var reader = new StreamReader(@"Data\domains.csv"))
            using (var csv = new CsvReader(reader))
            {
                contributionsCsvModels = csv.GetRecords<DomainContributionCsvModel>().Where(x => publications.Contains(x.Journal)).ToList();
            }

            var operations = new List<TableOperation>();
            foreach (var contribution in contributionsCsvModels)
            {
                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = CONST.SCIENCE_DOMAIN_NAMES.HUMANITIES,
                    Count = contribution.Humanities
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = CONST.SCIENCE_DOMAIN_NAMES.TECHNOLOGY,
                    Count = contribution.Technology
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = CONST.SCIENCE_DOMAIN_NAMES.MEDICAL,
                    Count = contribution.Medical
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = CONST.SCIENCE_DOMAIN_NAMES.FARM,
                    Count = contribution.Farm
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = CONST.SCIENCE_DOMAIN_NAMES.SOCIAL,
                    Count = contribution.Social
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = CONST.SCIENCE_DOMAIN_NAMES.SCIENCE,
                    Count = contribution.Science
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = CONST.SCIENCE_DOMAIN_NAMES.RELIGION,
                    Count = contribution.Religion
                }));

                operations.Add(TableOperation.Insert(new DomainContribution
                {
                    PartitionKey = contribution.Journal,
                    RowKey = CONST.SCIENCE_DOMAIN_NAMES.ARTS,
                    Count = contribution.Arts
                }));
            }

            var count = 0;
            foreach (var operation in operations)
            {
                await domainContributionsTable.ExecuteAsync(operation);
                count++;
                System.Console.WriteLine($"Domains seeded: {count}");
            }

            System.Console.WriteLine("Domains seeded successfully.");
        }

        private static async Task SeedDisciplineListAsync(CloudBlobClient blobClient)
        {
            System.Console.WriteLine("Seeding discipline list...");
            var container = blobClient.GetContainerReference(CONST.STORAGE_CONTAINER_NAMES.DISCIPLINES);
            var blob = container.GetBlockBlobReference(CONST.FILE_NAMES.DISCIPLINE_LIST);
            await blob.UploadFromFileAsync(@$"Data\{CONST.FILE_NAMES.DISCIPLINE_LIST}");
            System.Console.WriteLine("Discipline list seeded successfully.");
        }
    }
}
