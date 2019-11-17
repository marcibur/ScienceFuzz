using System.Collections.Generic;

namespace ScienceFuzz.Models
{
    public class Scientist
    {
        public string Name { get; set; }
        public List<Publication> Publications { get; set; } = new List<Publication>();
        public List<Contribution> DisciplineContributions { get; set; } = new List<Contribution>();
        public List<Contribution> DomainContributions { get; set; } = new List<Contribution>();
    }

    public class Publication
    {
        public string Journal { get; set; }
        public int Count { get; set; }
    }

    public class Contribution
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }
}
