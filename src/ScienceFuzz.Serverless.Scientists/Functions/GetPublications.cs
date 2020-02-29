using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Table;
using ScienceFuzz.Data;
using ScienceFuzz.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScienceFuzz.Serverless.Scientists.Functions
{
    // TODO: Refactor
    // TODO: 404
    public static class GetPublications
    {
        [FunctionName(nameof(GetPublications))]
        public static async Task<IEnumerable<PublicationModel>> ExecuteAsync(
            [HttpTrigger(AuthorizationLevel.Function, HTTP.GET, Route = "Scientists/{scientistName}/Publications")] HttpRequest httpRequest,
            [Table(CONST.STORAGE_TABLE_NAMES.PUBLICATIONS, Connection = ENV.STORAGE_CONNECTION)] CloudTable publicationsTable,
            string scientistName)
        {
            var query = new TableQuery<Publication>().Where(
                  TableQuery.GenerateFilterCondition(nameof(Publication.PartitionKey), QueryComparisons.Equal, scientistName));

            var queryResult = await publicationsTable.ExecuteQuerySegmentedAsync(query, null);
            var publications = queryResult.Results.Select(x => new PublicationModel
            {
                Title = x.RowKey,
                Count = x.Count
            }).ToList();

            return publications;
        }

        public class AllPublicationsModel
        {
            public string Name { get; set; }
            public IEnumerable<PublicationModel> Publications { get; set; }
        }

        [FunctionName("GetAllPublications")]
        public static async Task<IActionResult> ExecuteItAsync(
            [HttpTrigger(AuthorizationLevel.Function, HTTP.GET, Route = "Scientists/Publications")] HttpRequest httpRequest,
            [Table(CONST.STORAGE_TABLE_NAMES.PUBLICATIONS, Connection = ENV.STORAGE_CONNECTION)] CloudTable publicationsTable)
        {
            var query = new TableQuery<Publication>();
            var queryResult = await publicationsTable.ExecuteQuerySegmentedAsync(query, null);

            var d = new Dictionary<string, List<PublicationModel>>();
            foreach (var item in queryResult)
            {
                if (!d.ContainsKey(item.PartitionKey))
                {
                    d[item.PartitionKey] = new List<PublicationModel>();
                }
                d[item.PartitionKey].Add(new PublicationModel
                {
                    Title = item.RowKey,
                    Count = item.Count
                });
            }

            return new OkObjectResult(d);
        }
    }
}
