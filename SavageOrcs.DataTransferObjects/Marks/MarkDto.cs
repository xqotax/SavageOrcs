using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Areas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Marks
{
    public class MarkDto
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public double? Lng { get; set; }

        public double? Lat { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public string? ResourceUrl { get; set; }

        public bool? IsApproximate { get; set; }

        public AreaShortDto? Area { get; set; }

        public GuidIdAndStringName? Cluster { get; set; }

        public byte[][]? Images { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
