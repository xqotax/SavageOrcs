using Microsoft.AspNetCore.Mvc.Rendering;
using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Mark
{
    public class AddMarkViewModel
    {
        public Guid? Id { get; set; }

        public string? Lng { get; set; }

        public string? Lat { get; set; }

        public string? Zoom { get; set; }

        public Guid? AreaId { get; set; }

        public string? AreaName { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public string? ResourceUrl { get; set; }

        public string[]? Images { get; set; }

        public Guid UserId { get; set; }
        
        public bool? IsNew { get; set; }

        public bool? ToDelete { get; set; }

        public GuidIdAndNameViewModel[]? Areas { get; set; }

    }
}
