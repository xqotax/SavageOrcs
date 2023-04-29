using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Maps
{
    public class MapClusterDto
    {
        public double? Lat { get; set; }

        public double? Lng { get; set; }

        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? NameEng { get; set; }
    }
}
