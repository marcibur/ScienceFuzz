using CsvHelper;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using ScienceFuzz.Data.InMemory;
using ScienceFuzz.Models;
using System.IO;
using System.Linq;
using System.Text;

namespace ScienceFuzz.Web.Pages
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var iso = Encoding.GetEncoding("Windows-1250");

            using (var reader = new StreamReader(@"wwwroot\init\publications.csv", iso))
            using (var csv = new CsvReader(reader))
            {
                //csv.Configuration.RegisterClassMap<PublicationMap>();
                InMemoryData.Publications = csv.GetRecords<Publication>().ToList();
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
