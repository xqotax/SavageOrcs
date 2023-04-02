using SavageOrcs.DataTransferObjects.Texts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services.Interfaces
{
    public interface ITextService
    {
        Task<TextDto[]> GetTexts();
        Task<TextShortDto[]> GetShortTexts();
        Task<TextSaveResultDto> SaveText(TextSaveDto textSaveDto);
        Task<TextDto> GetTextById(Guid id);
        Task<TextDto[]> GetTextsByFilters(string? textName, string? textSubject, Guid[]? curatorIds);
        Task<bool> DeleteCluster(Guid id);
        Task<TextShortDto[]> GetTextsByCuratorIds(Guid curatorId);
    }
}
