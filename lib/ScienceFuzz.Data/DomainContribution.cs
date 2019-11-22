using Microsoft.Azure.Cosmos.Table;

namespace ScienceFuzz.Data
{
    public class DomainContribution : TableEntity
    {
        public int Count { get; set; }
    }
}
