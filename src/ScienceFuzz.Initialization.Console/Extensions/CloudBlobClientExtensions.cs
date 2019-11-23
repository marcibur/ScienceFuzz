using Microsoft.Azure.Storage.Blob;
using System.Threading.Tasks;

namespace ScienceFuzz.Initialization.Console.Extensions
{
    public static class CloudBlobClientExtensions
    {
        public static async Task RecreateContainerAsync(this CloudBlobClient blobClient, string containerName)
        {
            var container = blobClient.GetContainerReference(containerName);
            System.Console.WriteLine($"'{containerName}' container recreation...");
            await container.DeleteIfExistsAsync();
            await container.CreateAsync();
            System.Console.WriteLine($"'{containerName}' container recreated successfully.");
        }
    }
}
