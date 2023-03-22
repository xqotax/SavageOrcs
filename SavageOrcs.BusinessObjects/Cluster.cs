using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.BusinessObjects
{
    public class Cluster
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

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int? MapId { get; set; }

        public virtual Map? Map { get; set; }

        public Guid? AreaId { get; set; }

        public virtual Area? Area { get; set; }

        public Guid? CuratorId { get; set; }

        public virtual Curator? Curator { get; set; }

        public virtual ICollection<Mark> Marks { get; set; } = new HashSet<Mark>();

        public virtual ICollection<TextToCluster> TextsToClusters { get; set; } = new HashSet<TextToCluster>();

        public virtual ICollection<PlaceToCluster> PlaceToClusters { get; set; } = new HashSet<PlaceToCluster>();
    }
}
