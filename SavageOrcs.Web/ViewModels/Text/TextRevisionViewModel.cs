namespace SavageOrcs.Web.ViewModels.Text
{
    public class TextRevisionViewModel
    {
        public Guid? Id { get; set; }

        public Guid? CuratorId { get; set; }

        public string? CuratorName { get; set; }

        public string? Name { get; set; }

        public string? Subject { get; set; }

        public string? Content { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
