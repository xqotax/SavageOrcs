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
    }
}
