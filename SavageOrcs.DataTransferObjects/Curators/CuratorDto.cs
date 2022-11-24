using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Curators
{
    public class CuratorDto
    {
        public Guid Id { get; set; }

        public string? DisplayName { get; set; }

        public string? Description { get; set; }

        public byte[]? Image { get; set; }

        public string UserId { get; set; }
    }
}
