using System.Collections.Generic;

namespace ScienceFuzz.Models
{
    public class Journal
    {
        public string Title { get; set; }
        public string Discipline1 { get; set; }
        public string Discipline2 { get; set; }
        public string More { get; set; }

        public string MoreDisciplinesString => More.Replace(";", ", ");

        public IEnumerable<string> MoreDisciplinesEnum =>
            More.Split(';');
    }
}
