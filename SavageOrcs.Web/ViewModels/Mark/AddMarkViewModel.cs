using Microsoft.AspNetCore.Mvc.Rendering;
using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Mark
{
    public class AddMarkViewModel
    {
        public string Lng { get; set; }

        public string Lat { get; set; }

        public string Zoom { get; set; }

        public Guid? AreaId { get; set; }

        public List<SelectListItem> Areas { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DescriptionEng { get; set; }

        public string ResourceUrl { get; set; }

        public ImageMarkSaveViewModel[] ImageMarkSaveViewModels { get; set; }

        public Guid UserId { get; set; }

    }
}
