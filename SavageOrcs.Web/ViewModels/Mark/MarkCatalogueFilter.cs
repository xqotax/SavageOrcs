namespace SavageOrcs.Web.ViewModels.Mark
{
    public class MarkCatalogueFilter
    {
        public bool FullData { get; set; }

        public string? KeyWord { get; set; }

        public string? AreaName { get; set; }

        public string? MarkName { get; set; }

        public string? MarkDescription { get; set; }

        public bool NotIncludeCluster { get; set; }

        public int? From { get; set; }

        public int? Count { get; set; }
    }
}
