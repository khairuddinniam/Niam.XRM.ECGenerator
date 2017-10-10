using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Niam.XRM.ECGenerator.Core.Templates
{
    public static class Template
    {
        private static readonly IDictionary<string, string> TemplateCache = new Dictionary<string, string>();
        
        public static string Entities => GetStringTemplate(nameof(Entities));
        public static string EntityClass => GetStringTemplate(nameof(EntityClass));
        public static string Property => GetStringTemplate(nameof(Property));
        public static string Options => GetStringTemplate(nameof(Options));
        public static string OptionsClass => GetStringTemplate(nameof(OptionsClass));

        private static string GetStringTemplate(string name)
        {
            if (TemplateCache.TryGetValue(name, out string cachedTemplate))
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
