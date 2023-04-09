using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Texts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SavageOrcs.Services.Interfaces
{
    public interface IHelperService
    {
        byte[] GetBytes(string data);

        string GetImage(byte[] data);

        byte[] GetBytesForText(string data);

        string GetStringForText(byte[] data);

        UrlDto? FindOurUrl(string url, out string anotherText);

        Task<GuidIdAndStringName[]> GetAllKeyWords();

        Task SaveKeyWords(GuidNullIdAndStringName[] keyWordDtos);

        Task<GuidIdAndStringNameWithEnglishName[]> GetAllPlaces();

        Task<GuidIdAndStringNameWithEnglishName[]> GetPlacesByMarkId(Guid markId);

        Task SavePlaces(GuidNullIdAndStringNameWhitEngName[] placeDtos);

        string? GetTranslation(string? urk, string? eng);

        GuidIdAndStringName GetEmptySelect();
    }
}
