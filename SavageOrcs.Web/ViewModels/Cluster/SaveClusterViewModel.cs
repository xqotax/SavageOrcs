using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Cluster
{
    public class SaveClusterViewModel
    {
        public Guid? Id { get; set; }

        public string? Lat { get; set; }

        public string? Lng { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public Guid? AreaId { get; set; }
    }
}
