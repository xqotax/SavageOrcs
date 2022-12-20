using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Areas
{
    public class AreaShortDto
    {
        public Guid Id { get; set; } 

        public string Name { get; set; }

        public string Region { get; set; }

        public string Community { get; set; }
    }
}
