using Microsoft.EntityFrameworkCore;
using SavageOrcs.BusinessObjects;


namespace SavageOrcs.DbContext
{
    public partial class SavageOrcsDbContext
    {
        public virtual DbSet<Mark> Marks { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Map> Maps { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<AreaType> AreaTypes { get; set; }
        public virtual DbSet<Curator> Curators { get; set; }
        public virtual DbSet<Cluster> Clusters { get; set; }
        public virtual DbSet<Text> Texts { get; set; }
        public virtual DbSet<Block> Blocks { get; set; }
        public virtual DbSet<TextToCluster> TextsToClusters { get; set; }
        public virtual DbSet<TextToMark> TextsToMarks { get; set; }
        public virtual DbSet<KeyWord> KeyWords { get; set; }
        public virtual DbSet<Place> Places { get; set; }
    }
}
