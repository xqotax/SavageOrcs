using SavageOrcs.DataTransferObjects.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services.Interfaces
{
    public interface IMapService
    {
        Task<MapDto> GetMap(int id);
    }
}
