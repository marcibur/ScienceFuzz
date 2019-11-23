using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace ScienceFuzz.Initialization.Console.Extensions
{
    public static class CloudTableExtensions
    {
        public static async Task RecreateTableAsync(this CloudTableClient tableClient, string tableName)
        {
            var table = tableClient.GetTableReference(tableName);
            System.Console.WriteLine($"'{tableName}' table recreation...");
            await table.DeleteIfExistsAsync();
            await table.CreateAsync();
            System.Console.WriteLine($"'{tableName}' table recreated successfully.");
        }
    }
}
