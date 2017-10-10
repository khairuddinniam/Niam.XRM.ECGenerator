using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Niam.XRM.ECGenerator.Core.ExtensionsGenerator.Templates
{
    public static class Template
    {
        private static readonly IDictionary<string, string> TemplateCache = new Dictionary<string, string>();

        public static string GetRelated => GetStringTemplate(nameof(GetRelated));
        public static string Extensions => GetStringTemplate(nameof(Extensions));
        public static string ExtensionClass => GetStringTemplate(nameof(ExtensionClass));

        private static string GetStringTemplate(string name)
        {
            string cachedTemplate;
            if (TemplateCache.TryGetValue(name, out cachedTemplate))
                return cachedTemplate;

            var resourceName = $"{typeof (Template).Namespace}.{name}.template";
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
