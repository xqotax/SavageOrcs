using SavageOrcs.DataTransferObjects._Constants;

namespace SavageOrcs.Web.ViewModels.Mark
{
    public class MarkCatalogueViewModelShortData
    {
        public GuidIdAndStringName? Area { get; set; }

        public string? Name { get; set; }

        public Guid? Id { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }
    }
}
