using SavageOrcs.DataTransferObjects.Marks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services.Interfaces
{
    public interface IMarkService
    {
        Task<MarkDto[]> GetMarksByFilters(string? areaName, string? keyWord, string? markName, string? markDescription, bool NotIncludeCluster = false);

        Task<MarkDto[]> GetMarks();

        Task<MarkSaveResultDto> SaveMark(MarkSaveDto markSaveDto);

        Task<MarkDto> GetMarkById(Guid id);

        Task<bool> DeleteMark(Guid id);
    }
}
