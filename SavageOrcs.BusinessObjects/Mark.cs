namespace SavageOrcs.BusinessObjects
{
    public class Mark
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double? Lng { get; set; }

        public double? Lat { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public string? ResourceUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public bool? IsApproximate { get; set; }

        public Guid? ClusterId { get; set; }

        public virtual Cluster? Cluster { get; set; }

        public Guid? AreaId { get; set; }

        public virtual Area? Area { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }

        public int MapId { get; set; }

        public virtual Map Map { get; set;  }

        public virtual ICollection<Image> Images { get; set; } = new HashSet<Image>();

        public virtual ICollection<TextToMark> TextsToMarks { get; set; } = new HashSet<TextToMark>();
    }
}