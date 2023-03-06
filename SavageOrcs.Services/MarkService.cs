using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;

namespace SavageOrcs.Services
{
    public class MarkService : UnitOfWorkService, IMarkService
    {
        private readonly IRepository<Mark> _markRepository;
        private readonly IKeyWordService _keyWordService;
        private readonly IRepository<Image> _imageRepository;
        private readonly IRepository<TextToMark> _textsToMarksRepository;


        public MarkService(IUnitOfWork unitOfWork, IRepository<Mark> markRepository, IRepository<Image> imageRepository, IRepository<TextToMark> textsToMarksRepository, IKeyWordService keyWordService) : base(unitOfWork)
        {
            _markRepository = markRepository;
            _imageRepository = imageRepository;
            _textsToMarksRepository = textsToMarksRepository;
            _keyWordService = keyWordService;
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
                IsApproximate = mark.IsApproximate,
                Lat = mark.Cluster?.Lat ?? mark.Lat,
                Lng = mark.Cluster?.Lng ?? mark.Lng,
                Area = mark.Area is null ? (mark.Cluster?.Area is null ? null : new AreaShortDto
                {
                    Id = mark.Cluster.Area.Id,
                    Name = mark.Cluster.Area.Name,
                    Region = mark.Cluster.Area.Region,
                    Community = mark.Cluster.Area.Community
                }): new AreaShortDto
                {
                    Id = mark.Area.Id,
                    Name = mark.Area.Name,
                    Region = mark.Area.Region,
                    Community = mark.Area.Community
                },
                ResourceUrl = mark.ResourceUrl,
                Images = mark.Images.Select(x => x.Content).ToArray(),
                CreatedDate = mark.CreatedDate,
                Cluster = mark.Cluster is null ? null: new GuidIdAndStringName
                {
                    Id = mark.Cluster.Id,
                    Name = mark.Cluster.Name ?? ""
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
            mark.Description = markSaveDto.Description;
            mark.AreaId = markSaveDto.AreaId;
            mark.ClusterId = markSaveDto.ClusterId;
            mark.DescriptionEng = markSaveDto.DescriptionEng;
            mark.ResourceUrl = markSaveDto.ResourceUrl;
            mark.IsApproximate = markSaveDto.IsApproximate;
            mark.UserId = markSaveDto.UserId;
            mark.MapId = markSaveDto.MapId;
            mark.Lat = markSaveDto.Lat;
            mark.Lng = markSaveDto.Lng;


            if (markSaveDto.Images is not null)
            {
                foreach (var image in mark.Images)
                {
                    if (markSaveDto.Images is not null && markSaveDto.Images.All(x => !x.SequenceEqual(image.Content)))
                    {
                        _imageRepository.Delete(image);
                    }
                }

                if (markSaveDto.Images != null)
                    foreach (var imageDto in markSaveDto.Images)
                    {
                        var image = new Image();
                        if (mark.Images.Any(x => x.Content.SequenceEqual(imageDto)))
                            continue;


                        image.Mark = mark;
                        image.Content = imageDto;
                        await _imageRepository.AddAsync(image);
                    }
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
    }
}
