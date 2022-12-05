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
        Task<TextSaveResultDto> SaveText(TextSaveDto textSaveDto);
        Task<TextDto> GetTextById(Guid id);
        Task<TextDto[]> GetTextsByFilters(string? textName, string? textSubject, string? curatorName);
        Task<bool> DeleteCluster(Guid id);
    }
}
