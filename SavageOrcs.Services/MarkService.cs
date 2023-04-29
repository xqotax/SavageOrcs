using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.DataTransferObjects.Texts;
using SavageOrcs.DbContext.Migrations;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;
using System.Text.RegularExpressions;

namespace SavageOrcs.Services
{
    public class MarkService : UnitOfWorkService, IMarkService
    {
        private readonly IRepository<Mark> _markRepository;
        private readonly IHelperService _helperService;
        private readonly IRepository<Image> _imageRepository;
        private readonly IRepository<TextToMark> _textsToMarksRepository;


        public MarkService(IUnitOfWork unitOfWork, IRepository<Mark> markRepository, IRepository<Image> imageRepository, IRepository<TextToMark> textsToMarksRepository, IHelperService helperService) : base(unitOfWork)
        {
            _markRepository = markRepository;
            _imageRepository = imageRepository;
            _textsToMarksRepository = textsToMarksRepository;
            _helperService = helperService;
        }

        public async Task<MarkDto?> GetMarkById(Guid id)
        {
            var mark = await _markRepository.GetTAsync(x => x.Id == id);
            return mark is null ? null : CreateMarkDto(mark);
        }

        public async Task<MarkShortDto[]> GetMarksByFilters(Guid[]? keyWordIds, Guid[]? markIds, Guid[]? clusterIds, Guid[]? areaIds)
        {

            var marks = await _markRepository.GetAllAsync();

            var resultMarks = new List<Mark>();

            var filterByMarkIds = markIds is not null && markIds.Length > 0;
            var filterByClusterIds = clusterIds is not null && clusterIds.Length > 0;
            var filterByKeyWordIds = keyWordIds is not null && keyWordIds.Length > 0;
            var filterByAreaIds = areaIds is not null && areaIds.Length > 0;

            if (!filterByMarkIds && !filterByClusterIds && !filterByKeyWordIds && !filterByAreaIds)
                return marks.Select(CreateMarkShortDto).ToArray();

            if (filterByKeyWordIds)
            {
                var keyWords = (await _helperService.GetAllKeyWords()).Where(x => keyWordIds.Contains(x.Id) && !string.IsNullOrEmpty(x.Name)).Select(x => _helperService.GetTranslation(x.Name, x.NameEng)).ToArray();

                foreach (var mark in marks)
                {
                    if (!string.IsNullOrEmpty(mark.DescriptionEng)
                        && keyWords.Any(y => Regex.IsMatch(mark.DescriptionEng, @"\b" + Regex.Escape(y) + @"\b", RegexOptions.IgnoreCase)
                        || Regex.IsMatch(y, @"\b" + Regex.Escape(mark.DescriptionEng) + @"\b", RegexOptions.IgnoreCase)))
                    {
                        resultMarks.Add(mark);
                        continue;
                    }
                    if (!string.IsNullOrEmpty(mark.Description)
                        && keyWords.Any(y => Regex.IsMatch(mark.Description, @"\b" + Regex.Escape(y) + @"\b", RegexOptions.IgnoreCase)
                        || Regex.IsMatch(y, @"\b" + Regex.Escape(mark.Description) + @"\b", RegexOptions.IgnoreCase)))
                    {
                        resultMarks.Add(mark);
                        continue;
                    }
                    if (!string.IsNullOrEmpty(mark.Name)
                        && keyWords.Any(y => Regex.IsMatch(mark.Name, @"\b" + Regex.Escape(y) + @"\b", RegexOptions.IgnoreCase)
                        || Regex.IsMatch(y, @"\b" + Regex.Escape(mark.Name) + @"\b", RegexOptions.IgnoreCase)))
                    {
                        resultMarks.Add(mark);
                        continue;
                    }
                }
            }


            if (filterByMarkIds)
            {
                resultMarks.AddRange(marks.Where(x => markIds.Contains(x.Id)).ToList());
            }

            if (filterByClusterIds)
            {
                resultMarks.AddRange(marks.Where(x => x.ClusterId.HasValue && markIds.Contains(x.ClusterId.Value)).ToList());
            }

            if (filterByAreaIds)
            {
                resultMarks.AddRange(marks.Where(x => x.AreaId.HasValue && areaIds.Contains(x.AreaId.Value)).ToList());
            }

            resultMarks = resultMarks.GroupBy(x => x.Id).Select(x => x.First()).ToList();

            return resultMarks.Select(CreateMarkShortDto).ToArray();
        }

        public async Task<MarkShortDto[]> GetShortMarks()
        {
            var marks = await _markRepository.GetAllAsync();
            return marks.Select(CreateMarkShortDto).ToArray();
        }

        public async Task<MarkDto[]> GetMarks()
        {
            var marks = await _markRepository.GetAllAsync();

            return marks.Select(CreateMarkDto).ToArray();
        }

        public async Task<GuidIdAndStringNameWithEnglishName[]> GetMarkNames()
        {
            return (await _markRepository.GetAllAsync()).OrderBy(x => x.Name).Select(x => new GuidIdAndStringNameWithEnglishName
            {
                Id = x.Id,
                Name = x.Name,
                NameEng = x.NameEng
            }).ToArray();
        }
        private static MarkShortDto CreateMarkShortDto(Mark mark)
        {
            return new MarkShortDto
            {
                Id = mark.Id,
                CuratorName = mark.Curator?.Name,
                ClusterName = mark.Cluster?.Name,
                Description = mark.Description,
                DescriptionEng = mark.DescriptionEng,
                Lat = mark.Lat,
                Lng = mark.Lng,
                ResourceName = mark.ResourceName,
                ResourceNameEng = mark.ResourceNameEng,
                Name = mark.Name,
                NameEng = mark.NameEng, 
                ResourceUrl = mark.ResourceUrl,
                IsVisible = mark.IsVisible,
                Area = mark.Area is null ? (mark.Cluster?.Area is null ? null : new AreaShortDto
                {
                    Id = mark.Cluster.Area.Id,
                    Name = mark.Cluster.Area.Name,
                    Region = mark.Cluster.Area.Region,
                    Community = mark.Cluster.Area.Community
                }) : new AreaShortDto
                {
                    Id = mark.Area.Id,
                    Name = mark.Area.Name,
                    Region = mark.Area.Region,
                    Community = mark.Area.Community
                },
            };
        }


        private static MarkDto CreateMarkDto(Mark mark)
        {
            return new MarkDto
            {
                Id = mark.Id,
                Name = mark.Name,
                NameEng = mark.NameEng,
                Description = mark.Description,
                DescriptionEng = mark.DescriptionEng,
                Lat = mark.Lat ?? mark.Cluster?.Lat,
                Lng = mark.Lng ?? mark.Cluster?.Lng,
                IsVisible = mark.IsVisible,
                Area = mark.Area is null ? (mark.Cluster?.Area is null ? null : new AreaShortDto
                {
                    Id = mark.Cluster.Area.Id,
                    Name = mark.Cluster.Area.Name,
                    Region = mark.Cluster.Area.Region,
                    Community = mark.Cluster.Area.Community,
                    NameEng = mark.Cluster.Area.NameEng,
                    CommunityEng = mark.Cluster.Area.CommunityEng,
                    RegionEng = mark.Cluster.Area.RegionEng,
                }) : new AreaShortDto
                {
                    Id = mark.Area.Id,
                    Name = mark.Area.Name,
                    Region = mark.Area.Region,
                    Community = mark.Area.Community,
                    NameEng = mark.Area.NameEng,
                    CommunityEng = mark.Area.CommunityEng,
                    RegionEng = mark.Area.RegionEng,
                },
                ResourceUrl = mark.ResourceUrl,
                ResourceName = mark.ResourceName,
                ResourceNameEng = mark.ResourceNameEng,
                Images = mark.Images.Select(x => new ByteContentAndBooIsVisible
                {
                    Content = x.Content,
                    IsVisible = x.IsVisible
                }).ToArray(),
                CreatedDate = mark.CreatedDate,
                Cluster = mark.Cluster is null ? new GuidIdAndStringNameWithEnglishName() : new GuidIdAndStringNameWithEnglishName
                {
                    Id = mark.Cluster.Id,
                    Name = mark.Cluster.Name ?? "",
                    NameEng = mark.Cluster.NameEng ?? "",
                },
                Curator = mark.Curator is null ? new GuidIdAndStringNameWithEnglishName() : new GuidIdAndStringNameWithEnglishName
                {
                    Id = mark.Curator.Id,
                    Name = mark.Curator.Name ?? "",
                    NameEng = mark.Curator.NameEng ?? "",
                }
            };
        }

        public async Task<MarkSaveResultDto> SaveMark(MarkSaveDto markSaveDto)
        {
            var mark = new Mark();

            if (markSaveDto.Id is not null)
            {
                mark = await _markRepository.GetTAsync(x => x.Id == markSaveDto.Id);
                mark ??= new Mark();
            }
            else
            {
                mark.Id = Guid.NewGuid();
                mark.CreatedDate = DateTime.Now;
                await _markRepository.AddAsync(mark);
            }

            mark.UpdatedDate = DateTime.Now;
            mark.Name = markSaveDto.Name;
            mark.NameEng = markSaveDto.NameEng;
            mark.Description = markSaveDto.Description;

            if (markSaveDto.AreaId == _Constants.EmptySelect)
                mark.AreaId = null;
            else
                mark.AreaId = markSaveDto.AreaId;

            if (markSaveDto.ClusterId == _Constants.EmptySelect)
                mark.ClusterId = null;
            else
                mark.ClusterId = markSaveDto.ClusterId;

            if (markSaveDto.CuratorId == _Constants.EmptySelect)
                mark.CuratorId = null;
            else
                mark.CuratorId = markSaveDto.CuratorId;

            mark.DescriptionEng = markSaveDto.DescriptionEng;
            mark.ResourceUrl = markSaveDto.ResourceUrl;
            mark.ResourceName = markSaveDto.ResourceName;
            mark.MapId = markSaveDto.MapId;
            mark.Lat = markSaveDto.Lat;
            mark.Lng = markSaveDto.Lng;
            mark.IsVisible = markSaveDto.Images.Any(x => x.IsVisible);

            foreach (var image in mark.Images)
            {
                if (markSaveDto.Images.All(x => !x.Content.SequenceEqual(image.Content)))
                {
                    _imageRepository.Delete(image);
                }
            }

            foreach (var imageDto in markSaveDto.Images)
            {
                var image = new Image();
                if (mark.Images.Any(x => x.Content.SequenceEqual(imageDto.Content)))
                {
                    image = mark.Images.First(x => x.Content.SequenceEqual(imageDto.Content));
                    image.IsVisible = imageDto.IsVisible;
                    continue;
                }

                image.Mark = mark;
                image.Content = imageDto.Content;
                image.IsVisible = imageDto.IsVisible;
                await _imageRepository.AddAsync(image);
            }

            await UnitOfWork.SaveChangesAsync();
            return new MarkSaveResultDto()
            {
                Success = true,
                Id = mark.Id
            };
        }

        public async Task<bool> DeleteMark(Guid id)
        {
            try
            {
                var mark = await _markRepository.GetTAsync(x => x.Id == id);

                if (mark is null) return false;

                _imageRepository.DeleteRange(mark.Images);

                foreach (var textToMark in mark.TextsToMarks)
                {
                    _textsToMarksRepository.Delete(textToMark);
                }

                _markRepository.Delete(mark);

                await UnitOfWork.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<MarkShortDto[]> GetMarksByCuratorIds(Guid curatorId)
        {
            var marks = await _markRepository.GetAllAsync();

            return marks.Where(x => x.CuratorId.HasValue && x.CuratorId == curatorId)
                .Select(CreateMarkShortDto).ToArray();

        }
    }
}
