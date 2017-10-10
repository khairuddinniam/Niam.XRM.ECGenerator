using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Niam.XRM.ECGenerator.Core.Tests.Templates
{
    public static class ExpectedTemplate
    {
        private static readonly IDictionary<string, string> TemplateCache = new Dictionary<string, string>();

        public static string EntityClass => GetStringTemplate(nameof(EntityClass));
        public static string Property => GetStringTemplate(nameof(Property));
        public static string AttributeProperties => GetStringTemplate(nameof(AttributeProperties));
        public static string Using => GetStringTemplate(nameof(Using));
        public static string LowestLayer => GetStringTemplate(nameof(LowestLayer));
        public static string Options => GetStringTemplate(nameof(Options));
        public static string OptionsClass => GetStringTemplate(nameof(OptionsClass));

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
