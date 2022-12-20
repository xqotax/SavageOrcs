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

        public byte[]? Image { get; set; }

        public virtual string UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Text> Texts { get; set; } = new HashSet<Text>();
    }
}
