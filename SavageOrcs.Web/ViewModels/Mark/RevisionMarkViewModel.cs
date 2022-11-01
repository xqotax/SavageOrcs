using SavageOrcs.DataTransferObjects.Areas;

namespace SavageOrcs.Web.ViewModels.Mark
{
    public class RevisionMarkViewModel
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public double Lng { get; set; }

        public double Lat { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public string? ResourceUrl { get; set; }

        public string? Area { get; set; }

        public string[]? Images { get; set; }
    }
}
