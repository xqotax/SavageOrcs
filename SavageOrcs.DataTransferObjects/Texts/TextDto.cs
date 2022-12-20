using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Texts
{
    public class TextDto
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Subject { get; set; }

        public GuidIdAndStringName? Curator { get; set; }

        public BlockDto[]? BlockDtos { get; set; }
    }
}
