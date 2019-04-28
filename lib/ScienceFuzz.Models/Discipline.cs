namespace ScienceFuzz.Models
{
    public class Discipline
    {
        public string Title { get; set; }
        public string DomainsA { get; set; }
        public string DomainsB { get; set; }

        public override string ToString() => Title;
    }
}
