using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SavageOrcs.Services
{
    public class MarkService : UnitOfWorkService, IMarkService
    {
        private readonly IRepository<Mark> _markRepository;
        private readonly IRepository<Image> _imageRepository;


        public MarkService(IUnitOfWork unitOfWork, IRepository<Mark> markRepository, IRepository<Image> imageRepository) : base(unitOfWork)
        {
            _markRepository = markRepository;
            _imageRepository = imageRepository;
        }

        public async Task<MarkDto> GetMarkById(Guid id)
        {
            var mark = await _markRepository.GetTAsync(x => x.Id == id);
            if (mark is null) { 
                //error
            }

            return CreateMarkDto(mark);
        }

        public async Task<MarkDto[]> GetMarksByNameAndArea(string area, string keyWord)
        {
            var marks = await _markRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(keyWord))
            {
                
                marks = marks.Where(x => x.Name.Contains(keyWord, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(area))
            {
                marks = marks.Where(x => x.Area.Name.Contains(area, StringComparison.OrdinalIgnoreCase));
            }

            return marks.Select(x => CreateMarkDto(x)).ToArray();

        }

        public async Task<MarkDto[]> GetMarks()
        {
            var marks = await _markRepository.GetAllAsync();

            return marks.Select(x => CreateMarkDto(x)).ToArray();
        }

        private static MarkDto CreateMarkDto(Mark? mark)
        {
            return new MarkDto
            {
                Id = mark.Id,
                Name = mark.Name,
                Description = mark.Description,
                DescriptionEng = mark.DescriptionEng,
                Lat = mark.Lat,
                Lng = mark.Lng,
                Area = mark.Area is null? null: new AreaShortDto
                {
                    Id = mark.Area.Id,
                    Name = mark.Area.Name,
                    Region = mark.Area.Region,
                    Community = mark.Area.Community
                },
                ResourceUrl = mark.ResourceUrl,
                Images = mark.Images.Select(x => x.Content).ToArray()
            };
        }

        public async Task<MarkSaveResultDto> SaveMark(MarkSaveDto markSaveDto)
        {
            var mark = new Mark();
            
            if (markSaveDto.Id is not null)
                mark = await _markRepository.GetTAsync(x => x.Id == markSaveDto.Id);
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
            mark.DescriptionEng = markSaveDto.DescriptionEng;
            mark.ResourceUrl = markSaveDto.ResourceUrl;
            mark.UserId = markSaveDto.UserId;
            mark.MapId = markSaveDto.MapId.Value;
            mark.Lat = markSaveDto.Lat;
            mark.Lng = markSaveDto.Lng;

            foreach (var image in mark.Images)
            {
                if (markSaveDto.Images.All(x => x.SequenceEqual(image.Content)))
                {
                    _imageRepository.Delete(image);
                }
            }

            foreach (var imageDto in markSaveDto.Images)
            {
                var image = new Image();
                if (mark.Images.All(x => x.Content.SequenceEqual(imageDto)))
                {
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
    }
}
