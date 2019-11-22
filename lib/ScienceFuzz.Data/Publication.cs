using Microsoft.Azure.Cosmos.Table;

namespace ScienceFuzz.Data
{
    public class Publication : TableEntity
    {
        public int Count { get; set; }
    }
}
