using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.BusinessObjects
{
    public class Place
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? NameEng { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<PlaceToMark> PlaceToMarks { get; set; } = new HashSet<PlaceToMark>();

        public virtual ICollection<PlaceToCluster> PlaceToClusters { get; set; } = new HashSet<PlaceToCluster>();
    }
}
