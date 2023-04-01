using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.DataTransferObjects.Clusters;
using SavageOrcs.DataTransferObjects.Marks;
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

        public string? DescriptionEng { get; set; }

        public string? ResourceUrl { get; set; }

        public string? ResourceName { get; set; }

        public string? ResourceNameEng { get; set; }

        public GuidIdAndStringName Curator { get; set; } = new GuidIdAndStringName();

        public AreaShortDto? Area { get; set; }

        public ClusterMarkDto[] Marks { get; set; } = new ClusterMarkDto[0];

        public GuidIdAndStringNameWithEnglishName[] Places { get; set; } = Array.Empty<GuidIdAndStringNameWithEnglishName>();
    }
}
