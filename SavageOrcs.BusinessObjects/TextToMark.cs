using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.BusinessObjects
{
    public class TextToMark
    {
        public Guid Id { get; set; }

        public Guid MarkId { get; set; }

        public virtual Mark Mark { get; set; }

        public Guid TextId { get; set; }

        public virtual Text Text { get; set; }
    }
}
