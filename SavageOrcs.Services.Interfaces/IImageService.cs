using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services.Interfaces
{
    public interface IImageService
    {
        byte[] GetBytes(string data);

        string GetImage(byte[] data);
    }
}
