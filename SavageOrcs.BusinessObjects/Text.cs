using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.BusinessObjects
{
    public class Text
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Subject { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public Guid? CuratorId { get; set; }

        public virtual Curator? Curator { get; set; }

        public virtual ICollection<Block> Blocks { get; set; } = new HashSet<Block>();
    }
}
