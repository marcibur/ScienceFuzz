using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScienceFuzz.Console.Rtf
{
    public class Publication
    {
        public string Author { get; set; }
        public string Journal { get; set; }
        public int Count { get; set; }
    }

    public class Program
    {
        private static IEnumerable<string> WhiteList { get; set; }
        private static IEnumerable<Publication> Publications { get; set; }



        static async Task Main(string[] args)
        {
            await LoadWhiteListAsync();
            await LoadPublicationsAsync();
            WriteToCsv();
            WriteToZip();
        }



        private static async Task LoadWhiteListAsync()
        {
            string text;

            using (var streamReader = new StreamReader("whitelist.txt"))
            {
                text = await streamReader.ReadToEndAsync();
            }

            WhiteList = text.Replace("\r", "").Split("\n");
        }

        private static async Task LoadPublicationsAsync()
        {
            string text;

            using (var streamReader = new StreamReader("input.txt"))
            {
                text = await streamReader.ReadToEndAsync();
            }

            var chunks = Regex.Split(text, @"\d+. \r\n");
            var publications = new List<Publication>();

            for (int i = 1; i < chunks.Length; i++)
            {
                var authors = Regex.Matches(chunks[i], @"Aut.: .+\r\n")[0].Value
                    .Replace("Aut.: ", "")
                    .Replace("?", " ")
                    .Replace("\r\n", "")
                        .Split(", ");

                var journal = Regex.Matches(chunks[i], @"Pełny tytuł czasop.: .+\r\n")[0].Value
                    .Replace("Pełny tytuł czasop.: ", "")
                    .Replace("?", " ")
                    .Replace("\r\n", "");

                foreach (var author in authors)
                {
                    if (WhiteList.Any(x => author.Contains(x)))
                    {
                        var publication = publications.FirstOrDefault(x => x.Author == author.Split(' ')[0] && x.Journal == journal);
                        if (publication != null)
                        {
                            publication.Count++;
                        }
                        else
                        {
                            publications.Add(new Publication
                            {
                                Author = author.Split(' ')[0],
                                Journal = journal,
                                Count = 1
                            });
                        }
                    }
                }
            }

            Publications = publications;
        }

        private static void WriteToCsv()
        {
            var orderedPublications = Publications.OrderBy(x => x.Author).ThenBy(x => x.Journal).ToArray();

            using (var writer = new StreamWriter("output.csv", append: false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, new Configuration { Delimiter = "," }))
            {
                csv.WriteRecords(orderedPublications);
            }
        }

        private static void WriteToZip()
        {
            var publicationsOrderedByJournals = Publications.OrderBy(x => x.Journal).ToArray();

            var currentDirectory = Directory.GetCurrentDirectory();
            Directory.CreateDirectory($"{currentDirectory}/temp");

            foreach (var publication in publicationsOrderedByJournals)
            {
                File.AppendAllLines($"{currentDirectory}/temp/{publication.Author}.txt", new string[] { $"{publication.Journal};{publication.Count}" }, Encoding.UTF8);
            }

            File.Delete($"{currentDirectory}/output.zip");
            ZipFile.CreateFromDirectory($"{currentDirectory}/temp", "output.zip");
            Directory.Delete($"{currentDirectory}/temp", recursive: true);
        }
    }
}
