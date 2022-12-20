using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Marks
{
    public class MarkSaveDto
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public double? Lng { get; set; }

        public double? Lat { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public string? ResourceUrl { get; set; }

        public bool? IsApproximate { get; set; }

        public Guid? AreaId { get; set; }

        public Guid? ClusterId { get; set; }

        public string UserId { get; set; }

        public int MapId { get; set; }

        public byte[][]? Images { get; set; } 
    }
}
