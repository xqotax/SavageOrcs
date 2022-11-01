using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects.Areas;
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
    public class AreaService : UnitOfWorkService, IAreaService
    {
        private readonly IRepository<Area> _areaRepository;
        public AreaService(IUnitOfWork unitOfWork, IRepository<Area> areaRepository) : base(unitOfWork)
        {
            _areaRepository = areaRepository;
        }

        public async Task<AreaShortDto[]> GetAreasAsync()
        {
            var areas = await _areaRepository.GetAllAsync();
            if ((areas == null) || (!areas.Any()))
                throw new NotImplementedException();

            return areas.Select(x => new AreaShortDto
            {
                Community = x.Community,
                Id = x.Id,
                Region = x.Region,
                Name = x.Name 
            }).ToArray();
        } 

    }
}
