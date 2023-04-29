using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Cluster
{
    public class SaveClusterViewModel
    {
        public Guid? Id { get; set; }

        public string? Lat { get; set; }

        public string? Lng { get; set; }

        public string? Name { get; set; }

        public string? NameEng { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public string? ResourceName { get; set; }

        public string? ResourceNameEng { get; set; }

        public string? ResourceUrl { get; set; }

        public Guid? CuratorId { get; set; }

        public Guid? AreaId { get; set; }
    }
}
