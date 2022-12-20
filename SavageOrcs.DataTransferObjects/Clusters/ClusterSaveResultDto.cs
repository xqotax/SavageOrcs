using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Clusters
{
    public class ClusterSaveResultDto
    {
        public Guid Id { get; set; }

        public bool Success { get; set; }
    }
}
