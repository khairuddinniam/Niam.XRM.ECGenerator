using System.Text;
using Microsoft.Xrm.Sdk.Metadata;
using Niam.XRM.ECGenerator.Core.Config;
using Niam.XRM.ECGenerator.Core.ExtensionsGenerator.Templates;

namespace Niam.XRM.ECGenerator.Core.ExtensionsGenerator
{
    public class ExtensionClassesGenerator
    {
        private readonly IAppConfig _config;
        private readonly EntityMetadata[] _entitiesMetadata;

        public ExtensionClassesGenerator(IAppConfig config, EntityMetadata[] entitiesMetadata)
        {
            _config = config;
            _entitiesMetadata = entitiesMetadata;
        }

        public IEntityOutput Generate()
        {
            var content = new StringBuilder(Template.Extensions)
                .Replace("{{namespace}}", _config.Namespace)
                .Replace("{{extension-classes}}", GenerateExtensionClassesString())
                .ToString();

            return new EntityOutput("Entities.Extensions.cs", content);
        }

        private string GenerateExtensionClassesString()
        {
            var container = new StringBuilder();
            var holders = EntityMetadataHolder.Convert(_entitiesMetadata);
            foreach (var holder in holders)
                container.AppendLine(holder.Generate());
            return container.ToString();
        }
    }
}
