namespace SavageOrcs.Web.ViewModels.Constants
{
    public class GuidIdAndNameViewModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }
    }

    public class GuidIdAndNameViewModelWithEnglishName : GuidIdAndNameViewModel
    {
        public string? NameEng { get; set; }
    }
}
