using Microsoft.WindowsAzure.Storage.Table;
using ScienceFuzz.Data;
using ScienceFuzz.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScienceFuzz.Serverless.Domains.Logic
{
    public class Calculation
    {
        private readonly string _scientistName;
        private readonly CloudTable _publicationsTable;
        private readonly CloudTable _domainsTable;

        public Calculation(string scientistName, CloudTable publicationsTable, CloudTable domainsTable)
        {
            _scientistName = scientistName;
            _publicationsTable = publicationsTable;
            _domainsTable = domainsTable;
        }

        public async Task<IEnumerable<ContributionModel>> Execute()
        {
            var publicationsQuery = new TableQuery<Publication>()
             .Where(TableQuery.GenerateFilterCondition(nameof(Publication.PartitionKey), QueryComparisons.Equal, _scientistName))
             .Select(new string[] { nameof(Publication.RowKey), nameof(Publication.Count) });
            var publicationsQueryResult = await _publicationsTable.ExecuteQuerySegmentedAsync(publicationsQuery, null);
            var publications = publicationsQueryResult.Results.Select(x => x as Publication).ToList();

            var contributions = new List<ContributionModel>();
            foreach (var publication in publications)
            {
                var domainsQuery = new TableQuery<DomainContribution>()
                    .Where(TableQuery.GenerateFilterCondition(nameof(DomainContribution.PartitionKey), QueryComparisons.Equal, publication.RowKey))
                    .Select(new string[] { nameof(DomainContribution.RowKey), nameof(DomainContribution.Count) });
                var domainsQueryResult = await _domainsTable.ExecuteQuerySegmentedAsync(domainsQuery, null);
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
