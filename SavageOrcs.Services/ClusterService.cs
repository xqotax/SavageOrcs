using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.DataTransferObjects.Cluster;
using SavageOrcs.DataTransferObjects.Clusters;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.DbContext.Migrations;
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
    public class ClusterService : UnitOfWorkService, IClusterService
    {
        private readonly IRepository<Cluster> _clusterRepository;
        private readonly IRepository<Mark> _markRepository;
        private readonly IRepository<Image> _imageRepository;
        private readonly IRepository<TextToCluster> _textsToClustersRepository;

        public ClusterService(IUnitOfWork unitOfWork, IRepository<Cluster> clusterRepository, IRepository<Image> imageRepository, IRepository<Mark> markRepository, IRepository<TextToCluster> textsToClustersRepository) : base(unitOfWork)
        {
            _clusterRepository = clusterRepository;
            _imageRepository = imageRepository;
            _markRepository = markRepository;
            _textsToClustersRepository = textsToClustersRepository;
        }

        public async Task<ClusterDto[]> GetClusters()
        {
            var clusters = await _clusterRepository.GetAllAsync();

            return clusters.Select(x => CreateClusterDto(x)).ToArray();
        }

        public async Task<ClusterDto?> GetClusterById(Guid id)
        {
            var cluster = await _clusterRepository.GetTAsync(x => x.Id == id);

            return cluster is null ? null : CreateClusterDto(cluster);
        }

        public async Task<ClusterDto[]> GetClustersByFilters(string? keyWord, string? clusterName, string? clusterDescription, string? areaName, int? minCountOfMarks)
        {
            var clusters = await _clusterRepository.GetAllAsync();

            if (minCountOfMarks is not null)
            {
                clusters = clusters.Where(x => x.Marks.Count >= minCountOfMarks);
            }

            if (!string.IsNullOrEmpty(clusterName))
            {
                clusters = clusters.Where(x => x.Name is not null && x.Name.Contains(clusterName));
            }

            if (!string.IsNullOrEmpty(clusterDescription))
            {
                clusters = clusters.Where(x => x.Description is not null && x.Description.Contains(clusterDescription));
            }

            if (!string.IsNullOrEmpty(areaName))
            {
                clusters = clusters.Where(x => x.Area is not null && x.Area.Name.Contains(areaName));
            }

            return clusters.Select(x => CreateClusterDto(x)).ToArray();
        }

        public async Task<ClusterSaveResultDto> SaveCluster(ClusterSaveDto clusterSaveDto)
        {
            var cluster = new Cluster();

            if (clusterSaveDto.Id is not null)
            {
                cluster = await _clusterRepository.GetTAsync(x => x.Id == clusterSaveDto.Id);
                cluster ??= new Cluster();
            }
            else
            {
                cluster.Id = Guid.NewGuid();
                cluster.CreatedDate = DateTime.Now;
                await _clusterRepository.AddAsync(cluster);
            }

            cluster.UpdatedDate = DateTime.Now;
            cluster.Name = clusterSaveDto.Name;
            cluster.Description = clusterSaveDto.Description;
            cluster.Lat = clusterSaveDto.Lat;
            cluster.Lng = clusterSaveDto.Lng;
            cluster.MapId = clusterSaveDto.MapId;
            cluster.AreaId = clusterSaveDto.AreaId;

            await UnitOfWork.SaveChangesAsync();

            return new ClusterSaveResultDto
            {
                Id = cluster.Id,
                Success = true
            };
        }



        private static ClusterDto CreateClusterDto(Cluster cluster)
        {
            return new ClusterDto
            {
                Id = cluster.Id,
                Name = cluster.Name,
                Description = cluster.Description,
                DescriptionEng = cluster.DescriptionEng,
                ResourceName = cluster.ResourceName,
                ResourceUrl = cluster.ResourceUrl,
                CuratorName = cluster.Curator?.Name,
                Lat = cluster.Lat,
                Lng = cluster.Lng,
                Area = cluster.Area is null ? null : new AreaShortDto
                {
                    Id = cluster.Area.Id,
                    Name = cluster.Area.Name,
                    Region = cluster.Area.Region,
                    Community = cluster.Area.Community
                },
                Marks = cluster.Marks.Select(x => new ClusterMarkDto { 
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Images = x.Images.Select(y => y.Content).ToArray(),
                    DescriptionEng = x.DescriptionEng,
                    ResourceUrl = x.ResourceUrl,
                    Area = x.Area is null? null : new AreaShortDto
                    {
                        Id = x.Area.Id,
                        Name = x.Area.Name,
                        Community = x.Area.Community,
                        Region = x.Area.Region
                    }
                }).ToArray()
            };
        }

        public async Task<bool> DeleteCluster(Guid id, bool withMarks)
        {
            try
            {
                var cluster = await _clusterRepository.GetTAsync(x => x.Id == id);

                if (cluster is null) return false;

                if (withMarks)
                {
                    foreach (var mark in cluster.Marks)
                    {
                        _imageRepository.DeleteRange(mark.Images);
                        _markRepository.Delete(mark);
                    }
                }
                
                else
                {
                    foreach (var mark in cluster.Marks)
                    {
                        mark.Lat = cluster.Lat;
                        mark.Lng = cluster.Lng;
                        mark.AreaId = cluster.AreaId;
                        mark.ClusterId = null;
                    }
                }

                foreach (var textToCluster in cluster.TextsToClusters)
                {
                    _textsToClustersRepository.Delete(textToCluster);
                }

                _clusterRepository.Delete(cluster);

                await UnitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
