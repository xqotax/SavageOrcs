using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.DataTransferObjects.Texts;
using SavageOrcs.DbContext.Migrations;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;

namespace SavageOrcs.Services
{
    public class MarkService : UnitOfWorkService, IMarkService
    {
        private readonly IRepository<Mark> _markRepository;
        private readonly IHelperService _helperService;
        private readonly IKeyWordService _keyWordService;
        private readonly IRepository<Image> _imageRepository;
        private readonly IRepository<PlaceToMark> _placeToMarkRepository;
        private readonly IRepository<TextToMark> _textsToMarksRepository;


        public MarkService(IUnitOfWork unitOfWork, IRepository<Mark> markRepository, IRepository<Image> imageRepository, IRepository<TextToMark> textsToMarksRepository, IKeyWordService keyWordService, IHelperService helperService, IRepository<PlaceToMark> placeToMarkRepository) : base(unitOfWork)
        {
            _markRepository = markRepository;
            _imageRepository = imageRepository;
            _textsToMarksRepository = textsToMarksRepository;
            _keyWordService = keyWordService;
            _helperService = helperService;
            _placeToMarkRepository = placeToMarkRepository;
        }

        public async Task<MarkDto?> GetMarkById(Guid id)
        {
            var mark = await _markRepository.GetTAsync(x => x.Id == id);
            return mark is null ? null : CreateMarkDto(mark);
        }

        public async Task<MarkDto[]> GetMarksByFilters(string? areaName, Guid[] keyWordIds,  Guid[] markIds, Guid[] areaIds, string? markDescription, bool notIncludeCluster = false)
        {
            
            var marks = await _markRepository.GetAllAsync();

            var keyWords = await _keyWordService.GetKeyWordNamesByIds(keyWordIds);
            if (notIncludeCluster)
                marks = marks.Where(x => x.ClusterId is null);

            if (keyWords.Length > 0)
            {
                marks = marks.Where(x => (x.Name is not null && keyWords.Any(y => 
                                (y.Bool.HasValue && y.Bool.Value) ? x.Name.Contains(y.Name) :
                                    x.Name.Contains(y.Name, StringComparison.OrdinalIgnoreCase))) ||
                (x.Description is not null && keyWords.Any(y =>
                                (y.Bool.HasValue && y.Bool.Value) ? x.Description.Contains(y.Name) :
                                    x.Description.Contains(y.Name, StringComparison.OrdinalIgnoreCase))));

            }


            if (markIds.Length > 0)
            {
                marks = marks.Where(x => markIds.Contains(x.Id));
            }

            if (areaIds.Length > 0)
            {
                marks = marks.Where(x => x.AreaId != null && areaIds.Contains(x.AreaId.Value));
            }
            //if (!string.IsNullOrEmpty(markName))
            //{
            //    marks = marks.Where(x => x.Name is not null && x.Name.Contains(markName, StringComparison.OrdinalIgnoreCase));
            //}

            if (!string.IsNullOrEmpty(markDescription))
            {
                marks = marks.Where(x => (x.Description is not null && x.Description.Contains(markDescription, StringComparison.OrdinalIgnoreCase)) ||
                (x.DescriptionEng is not null && x.DescriptionEng.Contains(markDescription, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(areaName))
            {
                marks = marks.Where(x => x.Area is not null && (x.Area.Name + ", " + x.Area.Community + ", " + x.Area.Region).Contains(areaName, StringComparison.OrdinalIgnoreCase));
            }

            return marks.Select(CreateMarkDto).ToArray();
        }

        public async Task<MarkShortDto[]> GetShortMarks()
        {
            var marks = await _markRepository.GetAllAsync();
            return marks.Select(x => new MarkShortDto
            {
                Id = x.Id,
                CuratorName = x.Curator is null ? "" :x.Curator.Name ?? "",
                Description = x.Description,
                DescriptionEng = x.DescriptionEng,
                Lat = x.Lat,
                Lng = x.Lng,
                ResourceName = x.ResourceName,
                ResourceNameEng = x.ResourceNameEng,
                Name = x.Name,
                ResourceUrl = x.ResourceUrl,
                IsVisible = x.IsVisible,
                Area = x.Area is null ? (x.Cluster?.Area is null ? null : new AreaShortDto
                {
                    Id = x.Cluster.Area.Id,
                    Name = x.Cluster.Area.Name,
                    Region = x.Cluster.Area.Region,
                    Community = x.Cluster.Area.Community
                }) : new AreaShortDto
                {
                    Id = x.Area.Id,
                    Name = x.Area.Name,
                    Region = x.Area.Region,
                    Community = x.Area.Community
                },
            }).ToArray();
        }

        public async Task<MarkDto[]> GetMarks()
        {
            var marks = await _markRepository.GetAllAsync();

            return marks.Select(CreateMarkDto).ToArray();
        }

        public async Task<GuidIdAndStringName[]> GetMarkNames()
        {
            return (await _markRepository.GetAllAsync()).OrderBy(x => x.Name).Select(x => new GuidIdAndStringName
            {
                Id = x.Id,
                Name = x.Name
            }).ToArray();
        }

        private static MarkDto CreateMarkDto(Mark mark)
        {
            return new MarkDto
            {
                Id = mark.Id,
                Name = mark.Name,
                Description = mark.Description,
                DescriptionEng = mark.DescriptionEng,
                Lat = mark.Cluster?.Lat ?? mark.Lat,
                Lng = mark.Cluster?.Lng ?? mark.Lng,
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
                ResourceUrl = mark.ResourceUrl,
                ResourceName = mark.ResourceName,
                ResourceNameEng = mark.ResourceNameEng,
                Images = mark.Images.Select(x => new ByteContentAndBooIsVisible 
                { 
                    Content = x.Content, 
                    IsVisible = x.IsVisible 
                }).ToArray(),
                CreatedDate = mark.CreatedDate,
                Cluster = mark.Cluster is null ? new GuidIdAndStringName() : new GuidIdAndStringName
                {
                    Id = mark.Cluster.Id,
                    Name = mark.Cluster.Name ?? ""
                },
                Curator = mark.Curator is null ? new GuidIdAndStringName() : new GuidIdAndStringName
                {
                    Id = mark.Curator.Id,
                    Name = mark.Curator.Name ?? ""
                },
                Places = mark.PlaceToMarks.Select(x => new GuidIdAndStringNameWithEnglishName
                {
                    Id = x.Place.Id,
                    Name = x.Place.Name,
                    NameEng = x.Place.NameEng
                }).ToArray()
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
            mark.Description = markSaveDto.Description;
            mark.AreaId = markSaveDto.AreaId;
            mark.ClusterId = markSaveDto.ClusterId;
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
                if ( markSaveDto.Images.All(x => !x.Content.SequenceEqual(image.Content)))
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

            foreach(var placeToMark in mark.PlaceToMarks)
            {
                if (!markSaveDto.PlaceIds.Contains(placeToMark.PlaceId))
                    _placeToMarkRepository.Delete(placeToMark);
            }


            foreach (var placeId in markSaveDto.PlaceIds)
            {
                var oldPlaceIds = mark.PlaceToMarks.Select(x => x.PlaceId).ToArray();

                if (!oldPlaceIds.Contains(placeId))
                    _placeToMarkRepository.Add(new PlaceToMark
                    {
                        Id = new Guid(),
                        MarkId = mark.Id,
                        PlaceId = placeId
                    });
            }

            await UnitOfWork.SaveChangesAsync();
            return new MarkSaveResultDto()
            {
                Success = true,
                Id = mark.Id
            };
        }

        public async Task<bool> DeleteMark (Guid id)
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
                .Select(x => new MarkShortDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Lat = x.Lat,
                    CuratorName = x.Curator.Name,
                    Lng = x.Lng,
                    ResourceUrl = x.ResourceUrl,
                    Description = x.Description,
                    DescriptionEng = x.DescriptionEng,
                    ResourceName = x.ResourceName,
                    ResourceNameEng = x.ResourceNameEng,
                    IsVisible = x.IsVisible,
                    Area = x.Cluster is null ? (x.Area is null ? null : new AreaShortDto
                    {
                        Community = x.Area.Community,
                        Region = x.Area.Region,
                        Name = x.Area.Name,
                        Id = x.Area.Id
                    }) : x.Cluster.Area is null ? null : new AreaShortDto
                    {
                        Community = x.Cluster.Area.Community,
                        Region = x.Cluster.Area.Region,
                        Name = x.Cluster.Area.Name,
                        Id = x.Cluster.Area.Id
                    }
                    //AreaName = x.Cluster is null ? 
                    //    (x.Area is null ?
                    //        "" 
                    //        : x.Area.Name + ", " + x.Area.Community + ", " + x.Area.Region) 
                    //    : (x.Cluster.Area is null ?
                    //        "" : x.Cluster.Area.Name + ", " + x.Cluster.Area.Community + ", " + x.Cluster.Area.Region) 
                }).ToArray();

        }
    }
}
