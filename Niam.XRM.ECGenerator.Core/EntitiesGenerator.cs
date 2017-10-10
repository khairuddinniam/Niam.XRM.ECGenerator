using System.Text;
using Microsoft.Xrm.Sdk.Metadata;
using Niam.XRM.ECGenerator.Core.Templates;

namespace Niam.XRM.ECGenerator.Core
{
    public class EntitiesGenerator
    {
        private readonly EntityMetadata[] _entitiesMetadata;
        private readonly string _namespaceName;

        public EntitiesGenerator(EntityMetadata[] entitiesMetadata, string namespaceName)
        {
            _entitiesMetadata = entitiesMetadata;
            _namespaceName = namespaceName;
        }

        public IEntityOutput Generate()
        {
            var content = GenerateContent();
            return new EntityOutput("Entities.cs", content);
        }

        private string GenerateContent()
        {
            var classBuilder = new StringBuilder();
            foreach (var entityMetadata in _entitiesMetadata)
            {
                var entityClass = new EntityClassGenerator(entityMetadata).Generate();
                classBuilder.AppendLine(entityClass);
            }

            var builder = new StringBuilder(Template.Entities);
            builder.Replace("{{namespace}}", _namespaceName);
            builder.Replace("{{contents}}", classBuilder.ToString());

            return builder.ToString();
        }
    }
}
