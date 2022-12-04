using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services
{
    public class ImageService: UnitOfWorkService, IImageService
    {
        public ImageService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public byte[] GetBytes(string data)
        {
            return Encoding.ASCII.GetBytes(data);
        }
        public string GetImage(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }
    }
}
