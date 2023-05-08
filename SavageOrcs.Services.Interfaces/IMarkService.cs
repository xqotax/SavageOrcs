using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.DataTransferObjects._Constants;

namespace SavageOrcs.Services.Interfaces
{
    public interface IMarkService
    {
        Task<MarkShortDto[]> GetMarksByFilters(Guid[]? keyWordIds, Guid[]? markIds, Guid[]? clusterIds, Guid[]? areaIds);

        Task<MarkDto[]> GetMarks();

        Task<MarkShortDto[]> GetShortMarks();

        Task<GuidIdAndStringNameWithEnglishName[]> GetMarkNames();

        Task<MarkSaveResultDto> SaveMark(MarkSaveDto markSaveDto);

        Task<MarkDto?> GetMarkById(Guid id);

        Task<bool> DeleteMark(Guid id);

        Task<MarkShortDto[]> GetMarksByCuratorIds(Guid curatorId);
    }
}
