using SavageOrcs.DataTransferObjects.Cluster;
using SavageOrcs.DataTransferObjects.Clusters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services.Interfaces
{
    public interface IClusterService
    {
        Task<bool> DeleteCluster(Guid id, bool withMarks);

        Task<ClusterDto?> GetClusterById(Guid id, bool withImages = false);

        Task<ClusterSaveResultDto> SaveCluster(ClusterSaveDto clusterSaveDto);

        Task<ClusterDto[]> GetClusters();

        Task<ClusterDto[]> GetClustersByFilters(Guid[]? keyWordIds, Guid[]? clusterIds, Guid[]? areaIds);
    }
}
