namespace SavageOrcs.Web.ViewModels.Cluster
{
    public class SaveResultClusterViewModel
    {
        public Guid Id { get; set; }

        public bool Success { get; set; }

        public bool LatOrLngInNullOrEmpty { get; set; }
    }
}
