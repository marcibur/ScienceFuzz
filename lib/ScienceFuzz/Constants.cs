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
            public const string Domains = "domains";
        }

        public static class CacheTableNames
        {
            public const string DiscipilneContributions = "DisciplineContributions";
            public const string DomainContributions = "DomainContributions";
            public const string Tsne = "Tsne";
            public const string Kmeans = "Kmeans";
        }
    }
}
