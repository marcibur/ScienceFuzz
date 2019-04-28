namespace ScienceFuzz.Models
{
    public static class PublicationKeys
    {
        public const string Author = "Autor";
        public const string Title = "Tytuł";
        public const string JournalShort = "Skrót czasopisma";
        public const string JournalFull = "Pełny tytuł czasopisma";
        public const string FormalType = "Formalny typ publikacji";
    }

    public class Publication
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string JournalShort { get; set; }
        public string JournalFull { get; set; }
        public string FormalType { get; set; }

        public override string ToString() => Title;
    }
}
