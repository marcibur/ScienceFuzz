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
            var windowsEncoding = Encoding.GetEncoding("Windows-1250");



            using (var reader = new StreamReader(@"wwwroot\init\publications.csv", windowsEncoding))
            using (var csv = new CsvReader(reader))
            {
                InMemoryData.Publications = csv.GetRecords<Publication>().ToList();
                foreach (var item in InMemoryData.Publications)
                {
                    item.Title = item.Title.Trim();
                }
            }

            using (var reader = new StreamReader(@"wwwroot\init\journals.csv", windowsEncoding))
            using (var csv = new CsvReader(reader))
            {
                InMemoryData.Journals = csv.GetRecords<Journal>().ToList();
                foreach (var item in InMemoryData.Journals)
                {
                    item.Title = item.Title.Trim();
                }
            }

            using (var reader = new StreamReader(@"wwwroot\init\disciplines.csv", windowsEncoding))
            using (var csv = new CsvReader(reader))
            {
                InMemoryData.Disciplines = csv.GetRecords<Discipline>().ToList();
                foreach (var item in InMemoryData.Disciplines)
                {
                    item.Title = item.Title.Trim();
                }
            }



            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
