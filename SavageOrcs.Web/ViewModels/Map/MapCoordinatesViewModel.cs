using SavageOrcs.Web.ViewModels.Mark;

namespace SavageOrcs.Web.ViewModels.Map
{
    public class MapCoordinatesViewModel: UnitedCatalogueViewModel
    {
        public int Id { get; set; }

        public string? Lat { get; set; }

        public string? Lng { get; set; }

        public string Zoom { get; set; }

        public string? Name { get; set; }

        public MapMarkViewModel[] MapMarkViewModels { get; set; }

        public MapClusterViewModel[] MapClusterViewModels { get; set; }

        public string GoogleMapKey { get; set; } = string.Empty;
    }
}
