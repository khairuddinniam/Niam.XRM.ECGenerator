using Microsoft.Xrm.Sdk.Metadata;
using Niam.XRM.ECGenerator.Core.Templates;
using System;
using System.Linq;
using System.Text;

namespace Niam.XRM.ECGenerator.Core
{
    public class EntityClassGenerator
    {
        private readonly EntityMetadata _entityMetadata;

        public EntityClassGenerator(EntityMetadata entityMetadata)
        {
            _entityMetadata = entityMetadata;
        }

        public string Generate()
        {
            if (!Validate())
            {
                return "";
            }
            var builder = new StringBuilder(Template.EntityClass);
            builder.Replace("{{logical-name}}", _entityMetadata.LogicalName);
            builder.Replace("{{schema-name}}", _entityMetadata.SchemaName);
            var baseClassName = GenerateBaseClassName();
            builder.Replace("{{base-class}}", baseClassName);
            builder.Replace("{{attributes}}", GenerateAttributes());
            builder.Replace("{{primary-id-attribute}}", _entityMetadata.PrimaryIdAttribute);
            builder.Replace("{{options-class}}", GenerateOptions());

            return builder.ToString();
        }

        private readonly string[] _excludeEntities = new[] { "entity", "ribbonclientmetadata" };

        private bool Validate()
        {
            var notValid = _excludeEntities.Contains(_entityMetadata.LogicalName.ToLower());

            if (notValid)
            {
                Console.WriteLine("Test");
            }
            return !notValid;
        }

        public string GenerateBaseClassName()
        {
            if (!_entityMetadata.IsCustomEntity.GetValueOrDefault())
                return EntityBaseClass.Entity;

            if (_entityMetadata.IsOrganizationOwnership())
                return EntityBaseClass.Organization;

            if (_entityMetadata.IsUserOrTeamOwnership())
                return EntityBaseClass.UserOrTeam;

            return EntityBaseClass.Entity;
        }

        private string GenerateAttributes()
        {
            var attributes = GetFilteredAttributes();
            var builder = new StringBuilder();
            var propertyTemplate = Template.Property;
            foreach (var attributeMetadata in attributes)
            {
                var attribute = new PropertyGenerator(attributeMetadata, propertyTemplate).Generate();
                builder.AppendLine(attribute);
            }

            return builder.ToString();
        }

        private AttributeMetadata[] GetFilteredAttributes()
        {
            var baseClassName = GenerateBaseClassName();
            var attributes = PropertyGenerator.Filter(_entityMetadata.Attributes);
            attributes = Helper.FilterAttributes(attributes, baseClassName).ToArray();
            return attributes;
        }

        private string GenerateOptions()
        {
            var attributes = OptionsGenerator.Filter(_entityMetadata.Attributes);
            if (!attributes.Any()) return "";

            var enumsBuilder = new StringBuilder();
            foreach (var attributeMetadata in attributes)
            {
                var options = new OptionsGenerator(attributeMetadata, Template.Options).Generate();
                enumsBuilder.AppendLine(options);
            }

            var builder = new StringBuilder(Template.OptionsClass);
            builder.Replace("{{enum-values}}", enumsBuilder.ToString());
            return builder.ToString();
        }
    }
}
