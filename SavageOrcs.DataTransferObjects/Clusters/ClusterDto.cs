using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.DataTransferObjects.Clusters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Cluster
{
    public class ClusterDto
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double Lng { get; set; }

        public double Lat { get; set; }

        public string? Description { get; set; }

        public AreaShortDto? Area { get; set; }

        public ClusterMarkDto[]? Marks { get; set; }
    }
}
