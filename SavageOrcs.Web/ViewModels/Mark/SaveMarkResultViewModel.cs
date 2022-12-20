namespace SavageOrcs.Web.ViewModels.Mark
{
    public class SaveMarkResultViewModel
    {
        public Guid? Id { get; set; }

        public bool Success { get; set; }

        public string Url { get; set; }

        public string Text { get; set; }
    }
}
