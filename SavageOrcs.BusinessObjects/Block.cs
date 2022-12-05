using SavageOrcs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.BusinessObjects
{
    public class Block
    {
        public Guid Id { get; set; }

        public string? CustomId { get; set; }

        public BlockType Type { get; set; }

        public byte[]? Content { get; set; }

        public string? AdditionalParameter { get; set; }

        public int Index { get; set; }

        public Guid? TextId { get; set; }

        public virtual Text? Text { get; set; }
    }
}
