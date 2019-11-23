using Microsoft.AspNetCore.Http;
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
    }
}
