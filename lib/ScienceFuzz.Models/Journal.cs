using System.Collections.Generic;

namespace ScienceFuzz.Models
{
    public class Journal
    {
        public string Title { get; set; }
        public string DisciplineA { get; set; }
        public string DisciplineB { get; set; }
        public string DisciplinesC { get; set; }

        public string DisciplinesCString => DisciplinesC.Replace(";", ", ");

        public IEnumerable<string> DisciplinesCEnum =>
            DisciplinesC.Split(';');
    }
}
