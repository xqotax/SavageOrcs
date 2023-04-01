using SavageOrcs.DataTransferObjects.Marks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Texts;

namespace SavageOrcs.Services.Interfaces
{
    public interface IMarkService
    {
        Task<MarkDto[]> GetMarksByFilters(string? areaName, Guid[] keyWordIds, Guid[] markIds, Guid[] areaIds, string? markDescription, bool notIncludeCluster = false);

        Task<MarkDto[]> GetMarks();

        Task<MarkShortDto[]> GetShortMarks();

        Task<GuidIdAndStringName[]> GetMarkNames();

        Task<MarkSaveResultDto> SaveMark(MarkSaveDto markSaveDto);

        Task<MarkDto?> GetMarkById(Guid id);

        Task<bool> DeleteMark(Guid id);

        Task<MarkShortDto[]> GetMarksByCuratorIds(Guid curatorId);
    }
}
