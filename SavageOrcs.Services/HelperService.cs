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
        private readonly IRepository<Place> _placesRepository;
        public HelperService(IUnitOfWork unitOfWork, IRepository<KeyWord> keyWordRepository, IRepository<Place> placesRepository) : base(unitOfWork)
        {
            _keyWordRepository = keyWordRepository;
            _placesRepository = placesRepository;
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

        public async Task<GuidIdAndStringName[]> GetAllPlaces()
        {
            var places = await _placesRepository.GetAllAsync();

            return places.Select(x => new GuidIdAndStringName
            {
                Id = x.Id,
                Name = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk" ? x.Name : x.NameEng,
            }).ToArray();
        }
    }
}
