using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Maps
{
    public class MapDto
    {
        public double Lat { get; set; }

        public double Lng { get; set; }

        public string Zoom { get; set; }

        public string Name { get; set; }

        public MapMarkDto[] MapMarkDtos { get; set; }
    }
}
