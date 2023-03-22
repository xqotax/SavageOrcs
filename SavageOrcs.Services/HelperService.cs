using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Texts;
using SavageOrcs.Enums;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SavageOrcs.Services
{
    public class HelperService: UnitOfWorkService, IHelperService
    {
        private readonly IRepository<KeyWord> _keyWordRepository;
        private readonly IRepository<Place> _placeRepository;
        private readonly IRepository<PlaceToMark> _placeToMarkRepository;
        public HelperService(IUnitOfWork unitOfWork, IRepository<KeyWord> keyWordRepository, IRepository<Place> placeRepository, IRepository<PlaceToMark> placeToMarkRepository) : base(unitOfWork)
        {
            _keyWordRepository = keyWordRepository;
            _placeRepository = placeRepository;
            _placeToMarkRepository = placeToMarkRepository;
        }
        public byte[] GetBytesForText(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public string GetStringForText(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public byte[] GetBytes(string data)
        {
            return Encoding.ASCII.GetBytes(data);
        }
        public string GetImage(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }

        public UrlDto? FindOurUrl(string url, out string anotherText)
        {
            var tag = url[url.IndexOf("<a href=\"")..];

            var regex = new Regex(Regex.Escape("<a href=\""));
            tag = regex.Replace(tag, "", 1);

            anotherText = tag[(tag.IndexOf("</a>")+4)..];

            tag = tag[..tag.IndexOf('\"')];
            if (tag.Contains("Mark/Revision?id="))
            {
                return new UrlDto
                {
                    Type = ObjectType.Mark,
                    Id = new Guid(tag[(tag.IndexOf('=')+1)..])
                };
            }
            //else if (tag.Contains("Text/Revision?id="))
            //{
            //    return new UrlDto
            //    {
            //        UrlType = ObjectType.Text,
            //        UrlId = new Guid(tag[tag.IndexOf('=')..])
            //    };
            //}
            else if (tag.Contains("Cluster/Revision?id="))
            {
                return new UrlDto
                {
                    Type = ObjectType.Cluster,
                    Id = new Guid(tag[(tag.IndexOf('=') + 1)..])
                };
            }
            //else if (tag.Contains("Curator/Revision?id="))
            //{
            //    return new UrlDto
            //    {
            //        UrlType = ObjectType.Curator,
            //        UrlId = new Guid(tag[tag.IndexOf('=')..])
            //    };
            //}
            return null;
        }

        public async Task<GuidIdAndStringName[]> GetAllKeyWords()
        {
            var keyWords = await _keyWordRepository.GetAllAsync();

            return keyWords.Select(x => new GuidIdAndStringName
            {
                Id = x.Id,
                Name = x.Name
            }).ToArray();
        }

        public async Task<GuidIdAndStringNameWithEnglishName[]> GetAllPlaces()
        {
            var places = await _placeRepository.GetAllAsync();

            return places.Select(x => new GuidIdAndStringNameWithEnglishName
            {
                Id = x.Id,
                Name = x.Name,
                NameEng = x.NameEng
            }).ToArray();
        }

        public async Task<GuidIdAndStringNameWithEnglishName[]> GetPlacesByMarkId(Guid markId)
        {
            return (await _placeToMarkRepository.GetAllAsync()).Where(x => x.MarkId == markId)
                .Select(x => new GuidIdAndStringNameWithEnglishName
            {
                Id = x.PlaceId,
                Name = x.Place.Name,
                NameEng = x.Place.NameEng
            }).ToArray();
        }

        public async Task SaveKeyWords(GuidNullIdAndStringName[] keyWordDtos)
        {
            var dateTimeNow = DateTime.Now;

            var keyWords = await _keyWordRepository.GetAllAsync();

            var newKeyWordIds = keyWordDtos.Where(x => x.Id.HasValue).Select(x => x.Id).ToArray();

            foreach (var keyWord in keyWords)
            {
                if (!newKeyWordIds.Contains(keyWord.Id))
                    _keyWordRepository.Delete(keyWord);
            }   

            foreach(var keyWordDto in keyWordDtos)
            {
                if (keyWordDto.Id.HasValue)
                {
                    var keyWord = keyWords.First(x => x.Id == keyWordDto.Id.Value);
                    keyWord.UpdatedDate = dateTimeNow;
                    keyWord.Name = keyWordDto.Name;
                    keyWord.RegisterIsImportant = false;
                }
                else
                {
                    var keyWord = new KeyWord
                    {
                        Id = new Guid(),
                        Name = keyWordDto.Name,
                        CreatedDate = dateTimeNow,
                        UpdatedDate = dateTimeNow,
                        RegisterIsImportant = false
                    };

                    await _keyWordRepository.AddAsync(keyWord);
                }
            }

            await UnitOfWork.SaveChangesAsync();

            return;
        }

        public async Task SavePlaces(GuidNullIdAndStringNameWhitEngName[] placeDtos)
        {
            var dateTimeNow = DateTime.Now;

            var places = await _placeRepository.GetAllAsync();

            var newPlaceIds = placeDtos.Where(x => x.Id.HasValue).Select(x => x.Id).ToArray();

            foreach (var place in places)
            {
                if (!newPlaceIds.Contains(place.Id))
                {
                    var placeToMarks = await _placeToMarkRepository.GetAllAsync(x => x.PlaceId == place.Id);

                    _placeToMarkRepository.DeleteRange(placeToMarks);

                    _placeRepository.Delete(place);
                }
            }

            foreach (var placeDto in placeDtos)
            {
                if (placeDto.Id.HasValue)
                {
                    var place = places.First(x => x.Id == placeDto.Id.Value);
                    place.Name = placeDto.Name;
                    place.NameEng = placeDto.NameEng;
                }
                else
                {
                    var place = new Place
                    {
                        Id = new Guid(),
                        Name = placeDto.Name,
                        NameEng = placeDto.NameEng,
                        CreatedDate = dateTimeNow
                    };

                    await _placeRepository.AddAsync(place);
                }
            }

            await UnitOfWork.SaveChangesAsync();

            return;
        }
    }
}
