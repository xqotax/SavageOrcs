using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.BusinessObjects
{
    public class Curator
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public byte[]? Image { get; set; }

        public virtual ICollection<Mark> Marks { get; set; } = new HashSet<Mark>();

        public virtual ICollection<Cluster> Clusters { get; set; } = new HashSet<Cluster>();

        public virtual ICollection<Text> Texts { get; set; } = new HashSet<Text>();
    }
}
