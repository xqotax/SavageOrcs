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

    }
}
