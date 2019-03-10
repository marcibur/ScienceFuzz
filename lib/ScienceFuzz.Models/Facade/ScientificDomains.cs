using System.Collections.Generic;

namespace ScienceFuzz.Models.Facade
{
    public static class ScientificDomains
    {
        public static Domain Technology => new Domain("Technology");
        public static Domain Health => new Domain("Health");
        public static Domain Exact => new Domain("Exact");
        public static Domain Natural => new Domain("Natural");
        public static Domain Humanities => new Domain("Humanities");
        public static Domain Arts => new Domain("Arts");
        public static Domain Social => new Domain("Social");
        public static Domain Agriculture => new Domain("Agriculture");

        public static IEnumerable<Domain> All => new Domain[]
        {
            Technology,
            Health,
            Exact,
            Natural,
            Humanities,
            Arts,
            Social,
            Agriculture
        };
    }
}
