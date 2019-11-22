namespace ScienceFuzz.Initialization.Console.Models
{
    public class DisciplineContributionCsvModel
    {
        public string Journal { get; set; }
        public string DisciplinesString { get; set; }

        public string[] Disciplines => DisciplinesString.Split(';');
    }
}
