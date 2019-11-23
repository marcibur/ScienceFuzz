using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Table;
using ScienceFuzz.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScienceFuzz.Serverless.Scientists.Functions
{
    // TODO: Refactor
    // TODO: 404
    public static class GetScientistNames
    {
        [FunctionName(nameof(GetScientistNames))]
        public static async Task<IEnumerable<string>> ExecuteAsync(
            [HttpTrigger(AuthorizationLevel.Function, HTTP.GET, Route = "Scientists/Names")] HttpRequest httpRequest,
            [Table(CONST.STORAGE_TABLE_NAMES.SCIENTISTS, Connection = ENV.STORAGE_CONNECTION)] CloudTable scientistsTable)
        {
            var query = new TableQuery<Scientist>().Select(new string[] { nameof(Scientist.RowKey) });
            var queryResult = await scientistsTable.ExecuteQuerySegmentedAsync(query, null);
            var scientistNames = queryResult.Results.Select(x => x.RowKey).OrderBy(x => int.Parse(x.Split('_')[1])).ToList();
            return scientistNames;
        }
    }
}
