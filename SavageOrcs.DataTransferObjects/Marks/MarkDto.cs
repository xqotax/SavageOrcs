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

        public string? NameEng { get; set; }

        public double? Lng { get; set; }

        public double? Lat { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public string? ResourceUrl { get; set; }

        public string? ResourceName { get; set; }

        public string? ResourceNameEng { get; set; }

        public bool IsVisible { get; set; }

        public GuidIdAndStringNameWithEnglishName Curator { get; set; } = new GuidIdAndStringNameWithEnglishName();

        public AreaShortDto? Area { get; set; }

        public GuidIdAndStringNameWithEnglishName Cluster { get; set; } = new GuidIdAndStringNameWithEnglishName();

        public ByteContentAndBooIsVisible[] Images { get; set; } = Array.Empty<ByteContentAndBooIsVisible>();

        public DateTime CreatedDate { get; set; }
    }

    public class MarkShortDto
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public string? NameEng { get; set; }

        public double? Lng { get; set; }

        public double? Lat { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public string? ResourceUrl { get; set; }

        public string? ResourceName { get; set; }

        public string? ResourceNameEng{ get; set; }

        public string? CuratorName { get; set; }

        public string? CuratorNameEng { get; set; }

        public string? ClusterName { get; set; }

        public string? ClusterNameEng { get; set; }

        public bool IsVisible { get; set; }

        public AreaShortDto? Area { get; set; }
    }
}
