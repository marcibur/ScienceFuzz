using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Table;
using ScienceFuzz.Data;
using ScienceFuzz.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScienceFuzz.Serverless.Disciplines.Functions
{
    // TODO: Refactor
    // TODO: 404
    public static class CalculateDomains
    {
        [FunctionName(nameof(CalculateDomains))]
        public static async Task<IEnumerable<ContributionModel>> ExecuteAsync(
            [HttpTrigger(AuthorizationLevel.Function, HTTP.GET, Route = "Scientists/{scientistName}/Domains/Contributions")] HttpRequest httpRequest,
            [Table(CONST.STORAGE_TABLE_NAMES.PUBLICATIONS, Connection = ENV.STORAGE_CONNECTION)] CloudTable publicationsTable,
            [Table(CONST.STORAGE_TABLE_NAMES.DOMAIN_CONTRIBUTIONS, Connection = ENV.STORAGE_CONNECTION)] CloudTable domainsTable,
            string scientistName)
        {
            var publicationsQuery = new TableQuery<Publication>()
                .Where(TableQuery.GenerateFilterCondition(nameof(Publication.PartitionKey), QueryComparisons.Equal, scientistName))
                .Select(new string[] { nameof(Publication.RowKey), nameof(Publication.Count) });
            var publicationsQueryResult = await publicationsTable.ExecuteQuerySegmentedAsync(publicationsQuery, null);
            var publications = publicationsQueryResult.Results.Select(x => x as Publication).ToList();

            List<ContributionModel> contributions = new List<ContributionModel>();
            foreach (var publication in publications)
            {
                var domainsQuery = new TableQuery<DomainContribution>()
                    .Where(TableQuery.GenerateFilterCondition(nameof(DomainContribution.PartitionKey), QueryComparisons.Equal, publication.RowKey))
                    .Select(new string[] { nameof(DomainContribution.RowKey), nameof(DomainContribution.Count) });
                var domainsQueryResult = await domainsTable.ExecuteQuerySegmentedAsync(domainsQuery, null);
                var domains = domainsQueryResult.Results.ToList();

                foreach (var domain in domains)
                {
                    var contribution = contributions.FirstOrDefault(x => x.Name == domain.RowKey);
                    if (contribution == null)
                    {
                        contribution = new ContributionModel { Name = domain.RowKey, Value = 0 };
                        contributions.Add(contribution);
                    }
                    for (int i = 0; i < publication.Count * domain.Count; i++)
                    {
                        contribution.Value = S(contribution.Value, 1 * 0.001);
                    }
                }
            }

            return contributions;
        }



        private static double S(double x, double y)
        {
            return x + y - x * y;
        }
    }
}
