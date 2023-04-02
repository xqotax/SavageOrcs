using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Curators
{
    public class CuratorSaveResultDto
    {
        public bool Success {get;set;}

        public Guid? Id { get;set; }
    }
}
