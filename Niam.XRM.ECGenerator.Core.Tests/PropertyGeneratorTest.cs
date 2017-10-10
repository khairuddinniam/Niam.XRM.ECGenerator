using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Niam.XRM.ECGenerator.Core.Templates;
using Niam.XRM.ECGenerator.Core.Tests.Templates;
using Xunit;

namespace Niam.XRM.ECGenerator.Core.Tests
{
    public class PropertyGeneratorTest
    {
        [Theory]
        [InlineData(AttributeTypeCode.BigInt, "int?")]
        [InlineData(AttributeTypeCode.Boolean, "bool?")]
        [InlineData(AttributeTypeCode.CalendarRules, "")]
        [InlineData(AttributeTypeCode.Customer, "EntityReference")]
        [InlineData(AttributeTypeCode.DateTime, "DateTime?")]
        [InlineData(AttributeTypeCode.Decimal, "decimal?")]
        [InlineData(AttributeTypeCode.Double, "double?")]
        [InlineData(AttributeTypeCode.EntityName, "string")]
        [InlineData(AttributeTypeCode.Integer, "int?")]
        [InlineData(AttributeTypeCode.Lookup, "EntityReference")]
        [InlineData(AttributeTypeCode.Memo, "string")]
        [InlineData(AttributeTypeCode.Money, "Money")]
        [InlineData(AttributeTypeCode.Owner, "EntityReference")]
        [InlineData(AttributeTypeCode.PartyList, "EntityCollection")]
        [InlineData(AttributeTypeCode.Picklist, "OptionSetValue")]
        [InlineData(AttributeTypeCode.String, "string")]
        [InlineData(AttributeTypeCode.State, "OptionSetValue")]
        [InlineData(AttributeTypeCode.Status, "OptionSetValue")]
        [InlineData(AttributeTypeCode.Uniqueidentifier, "Guid?")]
        [InlineData(AttributeTypeCode.Virtual, "")]
        public void GetPropertyTypeString(AttributeTypeCode code, string expected)
        {
            var attributeMetadata = new AttributeMetadata();
            typeof(AttributeMetadata).GetProperty("AttributeType").SetValue(attributeMetadata, code);

            Assert.Equal(expected, new PropertyGenerator(attributeMetadata, Template.Property).GetPropertyTypeString());
        }

        [Fact]
        public void GetPropertyTypeString_ManagedProperty()
        {
            var attributeMetadata = new ManagedPropertyAttributeMetadata();
            typeof(ManagedPropertyAttributeMetadata).GetProperty("ValueAttributeTypeCode").SetValue(attributeMetadata, AttributeTypeCode.Boolean);

            Assert.Equal("bool?", new PropertyGenerator(attributeMetadata, Template.Property).GetPropertyTypeString());
        }

        [Fact]
        public void GetPropertyTypeString_ImageAttributeMetadata()
        {
            var imageAttributeMetadata = new ImageAttributeMetadata();
            Assert.Equal("byte[]", new PropertyGenerator(imageAttributeMetadata, Template.Property).GetPropertyTypeString());
        }

        [Theory]
        [InlineData("name", "name", "name")]
        [InlineData("capital", "CaPITaL", "CaPITaL")]
        [InlineData("id", "Id", "Id1")]
        [InlineData("entity", "entity", "entity1")]
        public void GetPropertyNameString(string logicalName, string schemaName, string expected)
        {
            var metadata = new AttributeMetadata
            {
                LogicalName = logicalName,
                SchemaName = schemaName
            };
            typeof(AttributeMetadata).GetProperty("EntityLogicalName").SetValue(metadata, "entity");

            Assert.Equal(expected, new PropertyGenerator(metadata, Template.Property).GetPropertyNameString());
        }

        [Theory]
        [InlineData("name", "name", null)]
        [InlineData("CaPITaL", "capital", null)]
        [InlineData("Id1", "id", "[System.ComponentModel.DataAnnotations.Schema.Column(\"id\")]")]
        [InlineData("entity1", "entity", "[System.ComponentModel.DataAnnotations.Schema.Column(\"entity\")]")]
        public void GetColumnAttributeString(string propertyName, string logicalName, string expected)
        {
            var metadata = new AttributeMetadata
            {
                LogicalName = logicalName
            };

            Assert.Equal(expected, new PropertyGenerator(metadata, Template.Property).GetColumnAttributeString(propertyName));
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(false, null)]
        [InlineData(true, "[System.ComponentModel.DataAnnotations.KeyAttribute]")]
        public void GetKeyAttributeString(bool? isPrimaryName, string expected)
        {
            var metadata = new AttributeMetadata();
            typeof(AttributeMetadata).GetProperty("IsPrimaryName").SetValue(metadata, isPrimaryName);

            Assert.Equal(expected, new PropertyGenerator(metadata, Template.Property).GetKeyAttributeString());
        }

        [Theory]
        [InlineData("xts_status", "Active", 1, null)]
        [InlineData("statecode", "NonActive", 2, null)]
        [InlineData("statecode", "Active", 3, "[System.ComponentModel.DescriptionAttribute(\"{\\\"a\\\":3}\")]")]
        public void GetAttributeInfoAttributeString(
            string logicalName, string optionLabel, int optionValue, string expected)
        {
            var localizedLabel = new LocalizedLabel(optionLabel, 1033);
            var metadata = new PicklistAttributeMetadata
            {
                LogicalName = logicalName,
                OptionSet = new OptionSetMetadata
                {
                    Options =
                    {
                        new OptionMetadata
                        {
                            Label = new Label(localizedLabel, new[] { localizedLabel }),
                            Value = optionValue
                        }
                    }
                }
            };

            Assert.Equal(expected, new PropertyGenerator(metadata, Template.Property).GetAttributeInfoAttributeString());
        }

        [Fact]
        public void Generate()
        {
            var metadata = new AttributeMetadata
            {
                LogicalName = "entity",
                SchemaName = "entity"
            };
            typeof(AttributeMetadata).GetProperty("IsPrimaryName").SetValue(metadata, true);
            typeof(AttributeMetadata).GetProperty("AttributeType").SetValue(metadata, AttributeTypeCode.String);
            typeof(AttributeMetadata).GetProperty("EntityLogicalName").SetValue(metadata, "entity");

            Assert.Equal(ExpectedTemplate.Property, new PropertyGenerator(metadata, Template.Property).Generate());
        }
    }
}
