using CsvHelper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ScienceFuzz.Models;
using ScienceFuzz.Serverless.Configuration;
using ScienceFuzz.Serverless.State;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[assembly: FunctionsStartup(typeof(Startup))]
namespace ScienceFuzz.Serverless.Configuration
{
    public class PublicationInitModel
    {
        public string Author { get; set; }
        public string Journal { get; set; }
        public int Count { get; set; }
    }

    public class JournalDisciplineInitModel
    {
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string DisciplinesString { get; set; }
        public string[] Disciplines => DisciplinesString.Split(';');
    }

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var appState = new AppState();

            // Load domains
            using (var streamReader = new StreamReader(@"Data\domains.txt"))
            {
                var text = streamReader.ReadToEnd();
                appState.Domains = text.Replace("\r", "").Split("\n");
            }

            // Load disciplines
            using (var streamReader = new StreamReader(@"Data\disciplines.txt"))
            {
                var text = streamReader.ReadToEnd();
                appState.Disciplines = text.Replace("\r", "").Split("\n");
            }



            // Load scientists + publications
            IEnumerable<PublicationInitModel> publications;
            List<Scientist> scientists = new List<Scientist>();
            using (var streamReader = new StreamReader(@"Data\author_journal.csv"))
            using (var csv = new CsvReader(streamReader))
            {
                publications = csv.GetRecords<PublicationInitModel>().ToList();
            }

            foreach (var publication in publications)
            {
                var scientist = scientists.FirstOrDefault(x => x.Name == publication.Author);
                if (scientist == null)
                {
                    scientist = new Scientist
                    {
                        Name = publication.Author
                    };

                    scientists.Add(scientist);
                }

                scientist.Publications.Add(new Publication
                {
                    Journal = publication.Journal,
                    Count = publication.Count
                });
            }

            appState.Scientists = scientists;


            // Add starting discipline contributions at 0 value for each scientist
            foreach (var scientist in scientists)
            {
                foreach (var discipline in appState.Disciplines)
                {
                    scientist.DisciplineContributions.Add(new Contribution
                    {
                        Name = discipline,
                        Value = 0
                    });
                }
            }

            // Load journal - discipline relation
            IEnumerable<JournalDisciplineInitModel> journalDiscipline;
            using (var streamReader = new StreamReader(@"Data\journal_discipline.csv"))
            using (var csv = new CsvReader(streamReader))
            {
                journalDiscipline = csv.GetRecords<JournalDisciplineInitModel>().ToList();
            }

            // Calculate discipline contributions for each scientist
            const double A = 0.01;
            foreach (var scientist in scientists)
            {
                foreach (var publication in scientist.Publications)
                {
                    var relation = journalDiscipline.FirstOrDefault(x => x.Title1 == publication.Journal || x.Title2 == publication.Journal);
                    if (relation != null)
                    {
                        foreach (var discipline in relation.Disciplines)
                        {
                            var contribution = scientist.DisciplineContributions.First(x => x.Name == discipline);
                            for (int i = 0; i < publication.Count; i++)
                            {
                                contribution.Value = S(contribution.Value, 1 * A);
                            }
                        }
                    }
                }
            }

            builder.Services.AddSingleton(appState);
        }

        //private double Calculate(List<double> scores)
        //{
        //    const double A = 0.01;
        //    double result = 0;

        //    foreach (var score in scores)
        //    {
        //        result = S(result, score * A);
        //    }

        //    return result;
        //}

        private double S(double x, double y)
        {
            return x + y - x * y;
        }
    }
}