using System.Collections.Generic;

namespace ScienceFuzz.Models
{
    public class Publication
    {
        public Publication(IEnumerable<Domain> domains)
        {
            Domains = domains;
        }

        public string Description { get; set; }
        public IEnumerable<Domain> Domains { get; }
    }
}
