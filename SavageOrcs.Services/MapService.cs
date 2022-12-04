using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects.Maps;
using SavageOrcs.DataTransferObjects.Marks;
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
    public class MapService : UnitOfWorkService, IMapService
    {
        private readonly IRepository<Map> _mapRepository;

        public MapService(IUnitOfWork unitOfWork, IRepository<Map> mapRepository) : base(unitOfWork)
        {
            _mapRepository = mapRepository;
        }

        public async Task<MapDto> GetMap(int id)
        {
            var map = await _mapRepository.GetTAsync(x => x.Id == id);
            if (map == null)
                return new MapDto();

            return new MapDto
            {
                Name = map.Name,
                MapMarkDtos = map.Marks.Where(x => x.ClusterId is null).Select(x => new MapMarkDto
                {
                    Lat = x.Lat,
                    Lng = x.Lng,
                    Id = x.Id,
                    Name = x.Name,
                    IsApproximate = x.IsApproximate
                }).ToArray(),
                MapClusterDtos = map.Clusters.Select(x => new MapClusterDto
                {
                    Lat = x.Lat,
                    Lng = x.Lng,
                    Id = x.Id,
                    Name = x.Name
                }).ToArray()
            };
        }
    }
}
