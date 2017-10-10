using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Metadata;
using Niam.XRM.ECGenerator.Core.ExtensionsGenerator.Templates;

namespace Niam.XRM.ECGenerator.Core.ExtensionsGenerator
{
    public class LookupAttributeMetadataHolder
    {
        private const string MethodPrefix = "Get";
        public string EntitySchemaName { get; }
        public LookupAttributeMetadata LookupMetadata { get; }
        public EntityMetadata LookupToEntityMetadata { get; }

        public LookupAttributeMetadataHolder(string entitySchemaName,
            LookupAttributeMetadata lookupMetadata, EntityMetadata lookupToEntityMetadata)
        {
            EntitySchemaName = entitySchemaName;
            LookupMetadata = lookupMetadata;
            LookupToEntityMetadata = lookupToEntityMetadata;
        }

        public string GetMethodName(IEnumerable<string> blacklist)
        {
            var exludes = blacklist?.ToArray() ?? new string[0];
            var candidates = GetMethodNameCandidates();
            var name = candidates.Except(exludes).FirstOrDefault();
            if (name != null) return name;

            var number = 0;
            var lastNameCandidate = candidates.Last();
            name = lastNameCandidate + (++number);
            while (exludes.Any(exclude => exclude == name))
                name = lastNameCandidate + (++number);

            return name;
        }

        private string[] GetMethodNameCandidates()
        {
            var lookupAttributeName = GetLookupAttributeName();
            var result = new[]
            {
                lookupAttributeName,
                lookupAttributeName + GetLookupEntityName()
            };
            
            return result.Select(name => Helper.AdjustName(MethodPrefix + name)).ToArray();
        }

        private string GetLookupAttributeName()
        {
            return LookupMetadata.DisplayName?.UserLocalizedLabel?.Label
                ?? LookupMetadata.SchemaName;
        }

        private string GetLookupEntityName()
        {
            return LookupToEntityMetadata.DisplayName?.UserLocalizedLabel?.Label
                ?? LookupToEntityMetadata.SchemaName;
        }

        public Result Generate(IEnumerable<string> blacklist)
        {
            var methodName = GetMethodName(blacklist);
            var multiLookup = LookupMetadata.Targets.Length > 1;
            var extraInfo = multiLookup ? "Multi lookup attribute. " : "";
            var content = new StringBuilder(Template.GetRelated)
                .Replace("{{extra-info}}", extraInfo)
                .Replace("{{entity-schema-name}}", EntitySchemaName)
                .Replace("{{attr-logical-name}}", LookupMetadata.LogicalName)
                .Replace("{{attr-schema-name}}", LookupMetadata.SchemaName)
                .Replace("{{method-name}}", methodName)
                .Replace("{{lookup-logical-name}}", LookupToEntityMetadata.LogicalName)
                .Replace("{{lookup-schema-name}}", LookupToEntityMetadata.SchemaName)
                .ToString();

            return new Result
            {
                MethodName = methodName,
                Content = content
            };
        }

        public class Result
        {
            public string MethodName { get; set; }
            public string Content { get; set; }
        }
    }
}
