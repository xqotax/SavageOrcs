using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Users
{
    public class UserSaveResultDto
    {
        public bool Success { get; set; }

        public bool UserNotFound { get; set; }

        public bool ErrorDuringSave { get; set; }
    }
}
