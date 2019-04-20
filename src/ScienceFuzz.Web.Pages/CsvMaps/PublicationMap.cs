using CsvHelper.Configuration;
using ScienceFuzz.Models;

namespace ScienceFuzz.Web.Pages.CsvMaps
{
    public class PublicationMap : ClassMap<Publication>
    {
        public PublicationMap()
        {
            Map(m => m.Author).Index(0).Name(PublicationKeys.Author);
            Map(m => m.Title).Index(1).Name(PublicationKeys.Title);
            Map(m => m.JournalShort).Index(2).Name(PublicationKeys.JournalShort);
            Map(m => m.JournalFull).Index(3).Name(PublicationKeys.JournalFull);
            Map(m => m.FormalType).Index(4).Name(PublicationKeys.FormalType);
        }
    }
}
