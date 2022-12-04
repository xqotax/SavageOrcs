using SavageOrcs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Blocks
{
    public class BlockDto
    {
        public Guid Id { get; set; }

        public BlockType Type { get; set; }

        public object? Content { get; set; }

        public int Index { get; set; }
    }
}
