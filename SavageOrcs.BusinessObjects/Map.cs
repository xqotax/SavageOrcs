using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.BusinessObjects
{
    public class Map
    {
        public int Id { get; set; }  

        public string? Name { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Mark> Marks { get; set; } = new HashSet<Mark>();

        public virtual ICollection<Cluster> Clusters { get; set; } = new HashSet<Cluster>();
    }
}
