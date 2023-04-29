using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.BusinessObjects
{
    public class Area
    { 
        public Guid Id { get; set; }

        public long? Lvl_1 { get; set; }

        public long? Lvl_2 { get; set; }

        public long? Lvl_3 { get; set; }

        public long? Lvl_4 { get; set; }

        public string Name { get; set; }

        public long Code { get; set; }

        public string Region { get; set; }

        public string Community { get; set; }

        public string NameEng { get; set; }

        public string RegionEng { get; set; }

        public string CommunityEng { get; set; }

        public virtual ICollection<Mark> Marks { get; set; } = new HashSet<Mark>();
    }
}
