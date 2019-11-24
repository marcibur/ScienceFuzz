using System.Collections.Generic;

namespace ScienceFuzz.Models.Shared
{
    public class ContributionsModel
    {
        public string Scientist { get; set; }
        public IEnumerable<ContributionModel> Contributions { get; set; }
    }
}
