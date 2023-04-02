using Microsoft.Extensions.Localization;
using System.Reflection;

namespace SavageOrcs.Web.Resources.Classes
{
    public class LanguageService
    {
        private readonly IStringLocalizer _localizer;

        public LanguageService(IStringLocalizerFactory factory)
        {
            var type = typeof(MainResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("MainResource", assemblyName.Name);
        }   
        public LocalizedString GetKey(string key)
        {
            return _localizer[key];
        }
    }
}
