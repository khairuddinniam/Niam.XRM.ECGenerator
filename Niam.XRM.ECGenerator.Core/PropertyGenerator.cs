using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Metadata;

namespace Niam.XRM.ECGenerator.Core
{
    public class PropertyGenerator
    {
        protected static readonly int Image = 1000;

        private static readonly IDictionary<int, string> PropertyTypeMap =
            new Dictionary<int, string>
            {
                { (int) AttributeTypeCode.BigInt, "int?" },
                { (int) AttributeTypeCode.Boolean, "bool?" },
                { (int) AttributeTypeCode.CalendarRules, "" },
                { (int) AttributeTypeCode.Customer, "EntityReference" },
                { (int) AttributeTypeCode.DateTime, "DateTime?" },
                { (int) AttributeTypeCode.Decimal, "decimal?" },
                { (int) AttributeTypeCode.Double, "double?" },
                { (int) AttributeTypeCode.EntityName, "string" },
                { (int) AttributeTypeCode.Integer, "int?" },
                { (int) AttributeTypeCode.Lookup, "EntityReference" },
                { (int) AttributeTypeCode.ManagedProperty, "" },
                { (int) AttributeTypeCode.Memo, "string" },
                { (int) AttributeTypeCode.Money, "Money" },
                { (int) AttributeTypeCode.Owner, "EntityReference" },
                { (int) AttributeTypeCode.PartyList, "EntityCollection" },
                { (int) AttributeTypeCode.Picklist, "OptionSetValue" },
                { (int) AttributeTypeCode.String, "string" },
                { (int) AttributeTypeCode.State, "OptionSetValue" },
                { (int) AttributeTypeCode.Status, "OptionSetValue" },
                { (int) AttributeTypeCode.Uniqueidentifier, "Guid?" },
                { (int) AttributeTypeCode.Virtual, "" },
                { Image, "byte[]" }
            };

        protected AttributeMetadata Metadata { get; }
        protected string Template { get; }

        public PropertyGenerator(AttributeMetadata metadata, string template)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Template = template ?? throw new ArgumentNullException(nameof(template));
        }

        public string Generate()
        {
            var type = GetPropertyTypeString();
            var name = GetPropertyNameString();
            var attributes = GetAttributesString(name);

            var builder = new StringBuilder(Template);
            builder.Replace("{{property-attributes}}", attributes);
            builder.Replace("{{property-type}}", type);
            builder.Replace("{{property-name}}", name);

            return builder.ToString();
        }

        protected virtual string GetAttributesString(string name)
        {
            var attributes = new[]
            {
                GetColumnAttributeString(name),
                GetKeyAttributeString(),
                GetAttributeInfoAttributeString()
            }.Where(a => a != null);

            return String.Join("", attributes);
        }

        public string GetPropertyTypeString()
        {
            var map = GetPropertyMap();

            if (Metadata is ManagedPropertyAttributeMetadata managedPropertyAttribute)
                return map[(int) managedPropertyAttribute.ValueAttributeTypeCode];

            if (Metadata is ImageAttributeMetadata imageAttribute)
                return map[Image];

            return Metadata.AttributeType.HasValue
                ? map[(int) Metadata.AttributeType.Value]
                : "";
        }

        protected virtual IDictionary<int, string> GetPropertyMap() => PropertyTypeMap;

        public virtual string GetPropertyNameString()
        {
            var lowerInvariantSchemaName = Metadata.SchemaName.ToLowerInvariant();
            var schemaNameAndEntityNameAreSame =
                lowerInvariantSchemaName == Metadata.EntityLogicalName.ToLowerInvariant();
            if (schemaNameAndEntityNameAreSame)
                return $"{Metadata.SchemaName}1";

            var isSchemaNameId = lowerInvariantSchemaName == "id";
            if (isSchemaNameId)
                return $"{Metadata.SchemaName}1";

            return Metadata.SchemaName;
        }

        public string GetColumnAttributeString(string propertyName)
        {
            return propertyName.ToLowerInvariant() != Metadata.LogicalName
                ? $"[System.ComponentModel.DataAnnotations.Schema.Column(\"{Metadata.LogicalName}\")]"
                : null;
        }

        public string GetKeyAttributeString()
        {
            return Metadata.IsPrimaryName.GetValueOrDefault()
                ? "[System.ComponentModel.DataAnnotations.KeyAttribute]"
                : null;
        }

        public string GetAttributeInfoAttributeString()
        {
            const string stateCode = "statecode";
            if (Metadata.LogicalName != stateCode) return null;

            var enumMetadata = (EnumAttributeMetadata) Metadata;
            var activeOption = enumMetadata.OptionSet.Options
                .FirstOrDefault(o => o.Label.UserLocalizedLabel?.Label == "Active");
            if (activeOption == null) return null;

            var info = new Metadata.AttributeInfo { StateCodeActiveValue = activeOption.Value };
            var infoJson = JsonSerializer.Serialize(info).Replace("\"","\\\"");
            return $"[System.ComponentModel.DescriptionAttribute(\"{infoJson}\")]";
        }

        public static AttributeMetadata[] Filter(AttributeMetadata[] attributes)
        {
            return attributes
                .Where(a =>
                {
                    if (a.LogicalName == "entityimageid")
                        return false;

                    if (a is ImageAttributeMetadata)
                        return true;

                    return a.AttributeOf == null && a.AttributeType != AttributeTypeCode.Virtual;
                })
                .ToArray(); 
        }
    }
}
