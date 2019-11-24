using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Table;
using ScienceFuzz.Data;
using ScienceFuzz.Models.Shared;
using ScienceFuzz.Serverless.Domains.Logic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScienceFuzz.Serverless.Disciplines.Functions
{
    // TODO: Refactor
    // TODO: 404
    public static class CalculateAll
    {
        [StorageAccount(ENV.STORAGE_CONNECTION)]
        [FunctionName(nameof(CalculateAll))]
        public static async Task<IEnumerable<ContributionsModel>> ExecuteAsync(
            [HttpTrigger(AuthorizationLevel.Function, HTTP.GET, Route = "Scientists/Domains/Contributions")] HttpRequest httpRequest,
            [Table(CONST.STORAGE_TABLE_NAMES.SCIENTISTS)] CloudTable scientistsTable,
            [Table(CONST.STORAGE_TABLE_NAMES.PUBLICATIONS)] CloudTable publicationsTable,
            [Table(CONST.STORAGE_TABLE_NAMES.DOMAIN_CONTRIBUTIONS)] CloudTable domainsTable)
        {
            var query = new TableQuery<Scientist>().Select(new string[] { nameof(Scientist.RowKey) });
            var queryResult = await scientistsTable.ExecuteQuerySegmentedAsync(query, null);
            var scientistNames = queryResult.Results.Select(x => x.RowKey).OrderBy(x => int.Parse(x.Split('_')[1])).ToList();

            var result = new List<ContributionsModel>();
            foreach (var scientistName in scientistNames)
            {
                var contributions = await new Calculation(scientistName, publicationsTable, domainsTable).Execute();
                result.Add(new ContributionsModel
                {
                    Scientist = scientistName,
                    Contributions = contributions
                });
            }

            return result;
        }
    }
}
