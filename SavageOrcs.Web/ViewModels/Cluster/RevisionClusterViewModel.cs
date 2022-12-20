namespace SavageOrcs.Web.ViewModels.Cluster
{
    public class RevisionClusterViewModel
    {
        public Guid Id { get; set; }
        
        public string? Name { get; set; }

        public string? Lat { get; set; }

        public string? Lng { get; set; }

        public string? Description { get; set; }

        public string? AreaName { get; set; }

        public Guid? AreaId { get; set; }

        public RevisionClusterMarksViewModel[]? Marks { get; set; }
    }
}
