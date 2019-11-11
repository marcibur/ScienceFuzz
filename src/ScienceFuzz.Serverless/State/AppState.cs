using System.Collections.Generic;

namespace ScienceFuzz.Serverless.State
{
    public class PublicationInitModel
    {
        public string Author { get; set; }
        public string Journal { get; set; }
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
