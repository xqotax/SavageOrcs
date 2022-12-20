using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.BusinessObjects
{
    public class TextToCluster
    {
        public Guid Id { get; set; }

        public Guid ClusterId { get; set; }

        public virtual Cluster Cluster { get; set; }

        public Guid TextId { get; set; }

        public virtual Text Text { get; set; }
    }
}
