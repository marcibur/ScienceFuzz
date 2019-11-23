using Microsoft.WindowsAzure.Storage.Table;

namespace ScienceFuzz.Data
{
    public class Publication : TableEntity
    {
        public int Count { get; set; }
    }
}
