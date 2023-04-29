using SavageOrcs.DataTransferObjects.Areas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Cluster
{
    public class ClusterSaveDto
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public string? NameEng { get; set; }

        public double Lng { get; set; }

        public double Lat { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public string? ResourceName { get; set; }

        public string? ResourceNameEng { get; set; }

        public string? ResourceUrl { get; set; }

        public Guid? CuratorId { get; set; }

        public Guid? AreaId { get; set; }

        public int? MapId { get; set; }

        public AreaShortDto? Area { get; set; }
    }
}
