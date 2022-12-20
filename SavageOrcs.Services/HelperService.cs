using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects.Texts;
using SavageOrcs.Enums;
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
        public HelperService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
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
    }
}
