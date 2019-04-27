using ScienceFuzz.Models;
using System.Collections.Generic;

namespace ScienceFuzz.Data.InMemory
{
    public static class InMemoryData
    {
        public static IEnumerable<Publication> Publications { get; set; }
        public static IEnumerable<Journal> Journals { get; set; }
        public static IEnumerable<Discipline> Disciplines { get; set; }
    }
}
