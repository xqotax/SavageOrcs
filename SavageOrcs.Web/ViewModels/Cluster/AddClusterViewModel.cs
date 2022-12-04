using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Cluster
{
    public class AddClusterViewModel
    {
        public Guid? Id { get; set; }

        public string? Lat { get; set; }

        public string? Lng { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Zoom { get; set; }

        public bool IsNew { get; set; }

        public Guid? AreaId { get; set; }

        public string? AreaName { get; set; }

        public GuidIdAndNameViewModel[]? Areas { get; set; }

        public bool? ToDelete { get; set; }
    }
}
