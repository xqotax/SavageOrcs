using SavageOrcs.DataTransferObjects.Areas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Clusters
{
    public class ClusterMarkDto
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public string? ResourceUrl { get; set; }

        public string? ResourceName{ get; set; }

        public string? ResourceNameEng { get; set; }

        public byte[][] Images { get; set; } = Array.Empty<byte[]>();

        public AreaShortDto? Area { get; set; }
    }
}
