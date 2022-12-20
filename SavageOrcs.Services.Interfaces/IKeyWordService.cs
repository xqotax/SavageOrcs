using SavageOrcs.DataTransferObjects._Constants;

namespace SavageOrcs.Services.Interfaces
{
    public interface IKeyWordService
    {
        Task<StringNameAndBool[]> GetKeyWordNamesByIds(Guid[] keyWordIds);

        Task<GuidIdAndStringName[]> GetKeyWords();
    }
}