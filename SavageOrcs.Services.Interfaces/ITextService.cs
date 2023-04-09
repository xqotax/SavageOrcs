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
        Task<TextShortDto[]> GetTextsByFilters(Guid[]? textIds, Guid[]? curatorIds);
        Task<bool> DeleteText(Guid id);
        Task<TextShortDto[]> GetTextsByCuratorIds(Guid curatorId);
    }
}
