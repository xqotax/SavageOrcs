using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services
{
    public class KeyWordService : UnitOfWorkService, IKeyWordService
    {
        private readonly IRepository<KeyWord> _keyWordReposoitroy;

        public KeyWordService(IUnitOfWork unitOfWork, IRepository<KeyWord> keyWordReposoitroy) : base(unitOfWork)
        {
            _keyWordReposoitroy = keyWordReposoitroy;
        }

        public async Task<GuidIdAndStringName[]> GetKeyWords()
        {
            var keyWords = await _keyWordReposoitroy.GetAllAsync();

            return keyWords.OrderBy(x => x.Name).Select(x => new GuidIdAndStringName { 
                Id = x.Id,
                Name = x.Name
            }).ToArray();
        }

        public async Task<StringNameAndBool[]> GetKeyWordNamesByIds(Guid[] keyWordIds)
        {
            var keyWords = await _keyWordReposoitroy.GetAllAsync(x => keyWordIds.Contains(x.Id));

            return keyWords is null ? Array.Empty<StringNameAndBool>() : keyWords.Select(x => new StringNameAndBool
            {
                Name = x.Name is null? "" :x.Name,
                Bool = x.RegisterIsImportant
            }).ToArray();
        }
    }
}
