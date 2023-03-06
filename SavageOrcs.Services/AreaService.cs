using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;

namespace SavageOrcs.Services
{
    public class AreaService : UnitOfWorkService, IAreaService
    {
        private readonly IRepository<Area> _areaRepository;
        private readonly IRepository<Mark> _markRepository;
        private readonly IRepository<Cluster> _clusterRepository;
        public AreaService(IUnitOfWork unitOfWork, IRepository<Area> areaRepository, IRepository<Mark> markRepository, IRepository<Cluster> clusterRepository) : base(unitOfWork)
        {
            _areaRepository = areaRepository;
            _markRepository = markRepository;
            _clusterRepository = clusterRepository;
        }

        public async Task<AreaShortDto[]> GetUsedAreasAsync()
        {
            var areasFromMarks = (await _markRepository.GetAllAsync(x => x.Area != null))
                .Select(x => new AreaShortDto
                {
                    Community = x.Area.Community,
                    Id = x.Area.Id,
                    Region = x.Area.Region,
                    Name = x.Area.Name
                });

            var areasFromCluster = (await _clusterRepository.GetAllAsync(x => x.Area != null))
                .Select(x => new AreaShortDto
                {
                    Community = x.Area.Community,
                    Id = x.Area.Id,
                    Region = x.Area.Region,
                    Name = x.Area.Name
                });

            return areasFromCluster.Concat(areasFromMarks).ToArray();
        }

        public async Task<AreaShortDto[]> GetAreasByNameAsync(string name)
        {
            var areas = await _areaRepository.GetAllAsync(x => x.Name.StartsWith(name));
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

        public async Task<AreaShortDto[]> GetAreasAsync()
        {
            var areas = await _areaRepository.GetAllAsync();

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
