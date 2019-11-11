using CsvHelper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ScienceFuzz.Serverless.Configuration;
using ScienceFuzz.Serverless.State;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[assembly: FunctionsStartup(typeof(Startup))]
namespace ScienceFuzz.Serverless.Configuration
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var appState = new AppState();
            IEnumerable<PublicationInitModel> publicationsInit;

            using (var reader = new StreamReader(@"Data\publications.csv"))
            using (var csv = new CsvReader(reader))
            {
                publicationsInit = csv.GetRecords<PublicationInitModel>().ToList();
            }

            var scientists = new List<Scientist>();
            var journals = new List<Journal>();
            var publications = new List<Publication>();
            foreach (var publicationInit in publicationsInit)
            {
                var scientist = scientists.FirstOrDefault(x => x.Name == publicationInit.Author);
                if (scientist == null)
                {
                    scientist = new Scientist { Name = publicationInit.Author };
                    scientists.Add(scientist);
                }

                var journal = journals.FirstOrDefault(x => x.Title == publicationInit.Journal);
                if (journal == null)
                {
                    journal = new Journal { Title = publicationInit.Journal };
                    journals.Add(journal);
                }

                var publication = publications.FirstOrDefault(x => x.Scientist == scientist && x.Journal == journal);
                if (publication == null)
                {
                    publication = new Publication { Scientist = scientist, Journal = journal, Count = 1 };
                    scientist.Publications.Add(publication);
                    publications.Add(publication);
                }
                else
                {
                    publication.Count++;
                }
            }

            appState.Scientists = scientists;

            builder.Services.AddSingleton(appState);
        }
    }
}