using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.DataTransferObjects.Texts;

namespace SavageOrcs.DataTransferObjects.Curators
{
    public class CuratorDto
    {
        public Guid Id { get; set; }

        public string? DisplayName { get; set; }

        public string? DisplayNameEng { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public byte[]? Image { get; set; }

        public TextShortDto[]? TextDtos { get; set; }

        public MarkShortDto[]? MarkDtos { get; set; }
    }
}
