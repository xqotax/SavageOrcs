using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects._Constants
{
    public class GuidIdAndStringName
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }
    }

    public class GuidIdAndStringNameWithEnglishName: GuidIdAndStringName
    {
        public string? NameEng { get; set; }
    }

    public class GuidNullIdAndStringName
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }
    }

    public class GuidNullIdAndStringNameWhitEngName : GuidNullIdAndStringName
    {
        public string? NameEng { get; set; }
    }
}
