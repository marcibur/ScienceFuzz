namespace ScienceFuzz
{
    public static class Constants
    {
        public static class StorageTableNames
        {
            public const string Scientists = "Scientists";
            public const string Publications = "Publications";
            public const string DisciplineContributions = "DisciplineContributions";
            public const string DomainContributions = "DomainContributions";
        }

        public static class StorageContainerNames
        {
            public const string Disciplines = "disciplines";
        }

        public static class FileNames
        {
            public const string DisciplineList = "disciplinesList.txt";
        }

        public static class CacheTableNames
        {
            public const string DiscipilneContributions = "DisciplineContributions";
            public const string DomainContributions = "DomainContributions";
            public const string Tsne = "Tsne";
            public const string Kmeans = "Kmeans";
        }

        public static class ScienceDomainNames
        {
            public const string Humanities = "Dziedzina nauk humanistycznych";
            public const string Technology = "Dziedzina nauk inżynieryjno-technicznych";
            public const string Medical = "Dziedzina nauk medycznych i nauk o zdrowiu";
            public const string Farm = "Dziedzina nauk rolniczych";
            public const string Social = "Dziedzina nauk społecznych";
            public const string Science = "Dziedzina nauk ścisłych i przyrodniczych";
            public const string Religion = "Dziedzina nauk teologicznych";
            public const string Arts = "Dziedzina sztuki";
        }
    }
}
