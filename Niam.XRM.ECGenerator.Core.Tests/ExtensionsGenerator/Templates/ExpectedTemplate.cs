using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Niam.XRM.ECGenerator.Core.Tests.ExtensionsGenerator.Templates
{
    public static class ExpectedTemplate
    {
        private static readonly IDictionary<string, string> TemplateCache = new Dictionary<string, string>();

        public static string GetRelatedSingleLookup => GetStringTemplate(nameof(GetRelatedSingleLookup));
        public static string GetRelatedMultiLookup => GetStringTemplate(nameof(GetRelatedMultiLookup));
        public static string ExtensionClass => GetStringTemplate(nameof(ExtensionClass));

        private static string GetStringTemplate(string name)
        {
            string cachedTemplate;
            if (TemplateCache.TryGetValue(name, out cachedTemplate))
                return cachedTemplate;

            var resourceName = $"{typeof (ExpectedTemplate).Namespace}.{name}.template";
            var assembly = Assembly.GetExecutingAssembly();
            using (var templateStream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(templateStream))
            {
                var template = reader.ReadToEnd();
                TemplateCache[name] = template;
                return template;
            }
        }
    }
}
