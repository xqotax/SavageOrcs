using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects._Constants
{
    public record StringIdAndStringName
    {
        public string Id { get; set; }
        public string? Name { get; set; }
    }
}
