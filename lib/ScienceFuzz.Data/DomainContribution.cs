using Microsoft.WindowsAzure.Storage.Table;

namespace ScienceFuzz.Data
{
    public class DomainContribution : TableEntity
    {
        public int Count { get; set; }
    }
}
