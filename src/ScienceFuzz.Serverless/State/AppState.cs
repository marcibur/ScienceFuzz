using System.Collections.Generic;

namespace ScienceFuzz.Serverless.State
{
    public class PublicationInitModel
    {
        public string Author { get; set; }
        public string Journal { get; set; }
    }

    public class DisciplineInitModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class DomainInitModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class JournalInitModel
    {
        public int Id { get; set; }
        public string Title_1 { get; set; }
        public string Title_2 { get; set; }
    }

    public class JournalDisciplineRealtionsInitModel
    {
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string DisciplineIdsString { get; set; }

        public string[] DisciplineIds => DisciplineIdsString.Split(' ');
    }


    public class Scientist
    {
        public string Name { get; set; }
        public List<Publication> Publications { get; set; } = new List<Publication>();
    }

    public class Publication
    {
        public Scientist Scientist { get; set; }
        public Journal Journal { get; set; }
        public int Count { get; set; }
    }

    public class Journal
    {
        public string Title { get; set; }
    }

    public class AppState
    {
        public IEnumerable<Scientist> Scientists { get; set; }
    }
}
