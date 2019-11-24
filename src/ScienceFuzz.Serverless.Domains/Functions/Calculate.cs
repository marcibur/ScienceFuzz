using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Table;
using ScienceFuzz.Models.Shared;
using ScienceFuzz.Serverless.Domains.Logic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScienceFuzz.Serverless.Disciplines.Functions
{
    // TODO: Refactor
    // TODO: 404
    public static class Calculate
    {
        [FunctionName(nameof(Calculate))]
        public static async Task<IEnumerable<ContributionModel>> ExecuteAsync(
            [HttpTrigger(AuthorizationLevel.Function, HTTP.GET, Route = "Scientists/{scientistName}/Domains/Contributions")] HttpRequest httpRequest,
            [Table(CONST.STORAGE_TABLE_NAMES.PUBLICATIONS, Connection = ENV.STORAGE_CONNECTION)] CloudTable publicationsTable,
            [Table(CONST.STORAGE_TABLE_NAMES.DOMAIN_CONTRIBUTIONS, Connection = ENV.STORAGE_CONNECTION)] CloudTable domainsTable,
            string scientistName) =>
                await new Calculation(scientistName, publicationsTable, domainsTable).Execute();
    }
}
