using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Table;
using ScienceFuzz.Data;
using ScienceFuzz.Models.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScienceFuzz.Serverless.Scientists.Functions
{
    // TODO: Refactor
    public static class GetPublications
    {
        [FunctionName(nameof(GetPublications))]
        public static async Task<IActionResult> ExecuteAsync(
            [HttpTrigger(AuthorizationLevel.Function, HTTP.GET, Route = "Scientists/{scientistName}/Publications")] HttpRequest httpRequest,
            [Table(CONST.STORAGE_TABLE_NAMES.PUBLICATIONS, Connection = ENV.STORAGE_CONNECTION)] CloudTable publicationsTable,
            string scientistName)
        {
            var query = new TableQuery<Publication>().Where(
                  TableQuery.GenerateFilterCondition(nameof(Publication.PartitionKey), QueryComparisons.Equal, scientistName));

            var publications = new List<PublicationModel>();
            foreach (Publication publication in await publicationsTable.ExecuteQuerySegmentedAsync(query, null))
            {
                publications.Add(new PublicationModel
                {
                    Title = publication.RowKey,
                    Count = publication.Count
                });
            }

            return new OkObjectResult(publications);
        }
    }
}
