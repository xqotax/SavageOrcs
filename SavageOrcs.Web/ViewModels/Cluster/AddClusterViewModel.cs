using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Cluster
{
    public class AddClusterViewModel
    {
        public Guid? Id { get; set; }

        public string? Lat { get; set; }

        public string? Lng { get; set; }

        public string? Name { get; set; }

        public string? NameEng { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }
        public string? Zoom { get; set; }

        public bool IsNew { get; set; }

        public Guid? AreaId { get; set; }

        public string? AreaName { get; set; }

        public Guid? CuratorId { get; set; }

        public string? CuratorName { get; set; }

        public string? ResourceUrl { get; set; }

        public string? ResourceName { get; set; }

        public string? ResourceNameEng{ get; set; }

        public bool? ToDelete { get; set; }

        public GuidIdAndNameViewModel[] Areas { get; set; } = new GuidIdAndNameViewModel[0];

        public GuidIdAndNameViewModel[] Curators { get; set; } = new GuidIdAndNameViewModel[0];

        public GuidIdAndNameViewModel[] Places { get; set; } = new GuidIdAndNameViewModel[0];

        public string GoogleMapKey { get;  set; } = string.Empty;
    }
}
