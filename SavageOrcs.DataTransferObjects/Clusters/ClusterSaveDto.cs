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

        public double Lng { get; set; }

        public double Lat { get; set; }

        public string? Description { get; set; }

        public int? MapId { get; set; }

        public Guid? AreaId { get; set; } 

        public AreaShortDto? Area { get; set; }
    }
}
