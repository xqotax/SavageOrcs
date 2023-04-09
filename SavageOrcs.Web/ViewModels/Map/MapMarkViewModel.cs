namespace SavageOrcs.Web.ViewModels.Map
{
    public class MapMarkViewModel
    {
        public string? Lat { get; set; }

        public string? Lng { get; set; }

        public Guid Id { get; set; }

        public string? Name { get; set; }

        public bool? IsCluster { get; set; }
    }
}
