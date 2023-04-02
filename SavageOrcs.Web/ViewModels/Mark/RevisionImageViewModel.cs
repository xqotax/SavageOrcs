namespace SavageOrcs.Web.ViewModels.Mark
{
    public class RevisionImageViewModel
    {
        public bool IsCluster { get; set; }

        public bool IsRevision { get; set; } = false;

        public Guid Id { get; set; }

        public string[] Images { get; set; } = Array.Empty<string>();
    }
}
