using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Metadata;
using Niam.XRM.ECGenerator.Core.ExtensionsGenerator.Templates;

namespace Niam.XRM.ECGenerator.Core.ExtensionsGenerator
{
    public class EntityMetadataHolder
    {
        public EntityMetadata EntityMetadata { get; }
        public LookupAttributeMetadataHolder[] LookupAttributeMetadataHolders { get; }

        public EntityMetadataHolder(EntityMetadata entityMetadata, LookupAttributeMetadataHolder[] lookupAttributeMetadataHolders)
        {
            EntityMetadata = entityMetadata;
            LookupAttributeMetadataHolders = lookupAttributeMetadataHolders;
        }

        public string Generate()
        {
            return new StringBuilder(Template.ExtensionClass)
                .Replace("{{entity-schema-name}}", EntityMetadata.SchemaName)
                .Replace("{{extension-methods}}", CreateExtensionMethodsString())
                .ToString();
        }

        private string CreateExtensionMethodsString()
        {
            var blacklist = new List<string>();
            var extMethodsBuilder = new StringBuilder();
            foreach (var holderGroup in LookupAttributeMetadataHolders.GroupBy(h => h.LookupMetadata))
            {
                string methodName = null;
                foreach (var lookupAttributeMetadataHolder in holderGroup)
                {
                    var extMethodResult = lookupAttributeMetadataHolder.Generate(blacklist);
                    methodName = extMethodResult.MethodName;
                    extMethodsBuilder.AppendLine(extMethodResult.Content);
                }

                blacklist.Add(methodName);
            }
            return extMethodsBuilder.ToString();
        }

        public static EntityMetadataHolder[] Convert(EntityMetadata[] entitiesMetadata)
        {
            var results = new List<EntityMetadataHolder>();
            foreach (var entityMetadata in entitiesMetadata)
            {
                var lookupAttributesMetadata = GetLookupAttributesMetadata(entityMetadata);
                if (!lookupAttributesMetadata.Any()) continue;

                var lookupTargets = lookupAttributesMetadata.SelectMany(am => am.Targets.Select(t => new
                {
                    Metadata = am,
                    Target = t
                }));

                var attributeHolders =
                    from lt in lookupTargets
                    join em in entitiesMetadata
                        on lt.Target equals em.LogicalName
                    orderby lt.Metadata.SchemaName
                        orderby em.SchemaName 
                    select new LookupAttributeMetadataHolder(entityMetadata.SchemaName, lt.Metadata, em);

                results.Add(new EntityMetadataHolder(entityMetadata, attributeHolders.ToArray()));
            }

            return results.OrderBy(r => r.EntityMetadata.SchemaName).ToArray();
        }

        private static LookupAttributeMetadata[] GetLookupAttributesMetadata(EntityMetadata entityMetadata)
        {
            var ignoredAttributeTypes = new[]
            {
                AttributeTypeCode.Virtual,
                AttributeTypeCode.PartyList
            };
            var lookupAttributesMetadata = entityMetadata.Attributes
                .Where(a => a is LookupAttributeMetadata)
                .Where(a => a.AttributeOf == null)
                .Where(a => !ignoredAttributeTypes.Any(ignore => ignore == a.AttributeType))
                .Cast<LookupAttributeMetadata>();

            if (entityMetadata.IsUserOrTeamOwnership())
                return lookupAttributesMetadata.Where(a => Helper.UserOrTeamExcludeAttributeSchemaNames.All(ex => ex != a.SchemaName))
                    .ToArray();

            if (entityMetadata.IsOrganizationOwnership())
                return lookupAttributesMetadata.Where(a => Helper.OrganizationExcludeAttributeSchemaNames.All(ex => ex != a.SchemaName))
                    .ToArray();

            return lookupAttributesMetadata.ToArray();
        }
    }
}
