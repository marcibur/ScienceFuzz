using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Table;
using ScienceFuzz.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScienceFuzz.Serverless.Scientists
{
    public static class GetScientistNames
    {
        [FunctionName(nameof(GetScientistNames))]
        public static async Task<IActionResult> ExecuteAsync(
            [HttpTrigger(AuthorizationLevel.Function, HTTP.GET, Route = "Scientists/Names")] HttpRequest httpRequest,
            [Table(CONST.STORAGE_TABLE_NAMES.SCIENTISTS, Connection = ENV.STORAGE_CONNECTION)] CloudTable scientistsTable)
        {
            var query = new TableQuery<Scientist>().Select(new string[] { nameof(Scientist.RowKey) });

            var scientistNames = new List<string>();
            foreach (Scientist scientist in await scientistsTable.ExecuteQuerySegmentedAsync(query, null))
            {
                scientistNames.Add(scientist.RowKey);
            }

            scientistNames = scientistNames.OrderBy(x => int.Parse(x.Split('_')[1])).ToList();
            return new OkObjectResult(scientistNames);
        }
    }
}
