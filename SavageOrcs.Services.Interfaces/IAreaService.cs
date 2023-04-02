using SavageOrcs.DataTransferObjects.Areas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services.Interfaces
{
    public interface IAreaService
    {
        Task<AreaShortDto[]> GetAreasAsync();

        Task<AreaShortDto[]> GetUsedAreasAsync();

        Task<AreaShortDto[]> GetAreasByNameAsync(string name);
    }
}
