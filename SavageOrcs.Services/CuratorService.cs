using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects.Curators;
using SavageOrcs.DataTransferObjects.Texts;
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
    public class CuratorService : UnitOfWorkService, ICuratorService
    {
        private readonly IRepository<Curator> _curatorRepository;
        public CuratorService(IUnitOfWork unitOfWork, IRepository<Curator> curatorRepositor) : base(unitOfWork)
        {
            _curatorRepository = curatorRepositor;
        }

        public async Task<CuratorDto[]> GetCurators()
        {
            var curators = await _curatorRepository.GetAllAsync();

            return curators.Select(x => CreateCuratorDto(x)).ToArray();
        }

        public async Task<CuratorDto> GetCuratorById(Guid id)
        {
            var curator = await _curatorRepository.GetTAsync(x => x.Id == id);

            curator ??= new Curator();

            return CreateCuratorDto(curator);
        }


        private static CuratorDto CreateCuratorDto(Curator curator)
        {
            return new CuratorDto
            {
                Id = curator.Id,
                DisplayName = curator.Name,
                Description = curator.Description,
                Image = curator.Image,
                UserId = curator.UserId,
                TextDtos = curator.Texts.Select(x => new TextShortDto { 
                    Id = x.Id,
                    Name = x.Name,
                    Subject = x.Subject
                }).ToArray()

            };
        }
    }
}
