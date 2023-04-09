using Microsoft.Extensions.Localization;
using System.Reflection;

namespace SavageOrcs.Web.Resources.Classes
{
    public class LanguageService
    {
        private readonly IStringLocalizer _mainLocalizer;
        private readonly IStringLocalizer _markLocalizer;
        private readonly IStringLocalizer _textLocalizer;
        private readonly IStringLocalizer _curatorLocalizer;

        public LanguageService(IStringLocalizerFactory factory)
        {
            var mainType = typeof(MainResource);
            var mainAssemblyName = new AssemblyName(mainType.GetTypeInfo().Assembly.FullName);
            _mainLocalizer = factory.Create("MainResource", mainAssemblyName.Name);

            var markType = typeof(MarkResource);
            var markAssemblyName = new AssemblyName(markType.GetTypeInfo().Assembly.FullName);
            _markLocalizer = factory.Create("MarkResource", markAssemblyName.Name);

            var textType = typeof(TextResource);
            var textAssemblyName = new AssemblyName(textType.GetTypeInfo().Assembly.FullName);
            _textLocalizer = factory.Create("TextResource", textAssemblyName.Name);

            var curatorType = typeof(CuratorResource);
            var curatorAssemblyName = new AssemblyName(curatorType.GetTypeInfo().Assembly.FullName);
            _curatorLocalizer = factory.Create("CuratorResource", curatorAssemblyName.Name);
        }

        public LocalizedString GetMainKey(string key)
        {
            return _mainLocalizer[key];
        }

        public LocalizedString GetMarkKey(string key)
        {
            return _markLocalizer[key];
        }

        public LocalizedString GetTextKey(string key)
        {
            return _textLocalizer[key];
        }

        public LocalizedString GetCuratorKey(string key)
        {
            return _curatorLocalizer[key];
        }
    }
}
