using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Curators
{
    public class CuratorSaveDto
    {
        public Guid? Id { get; set; }

        public string? DisplayName { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public byte[]? Image { get; set; }

    }
}
