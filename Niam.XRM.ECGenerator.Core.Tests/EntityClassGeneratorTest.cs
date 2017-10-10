using Microsoft.Xrm.Sdk.Metadata;
using Niam.XRM.ECGenerator.Core.Config;
using Niam.XRM.ECGenerator.Core.Tests.Templates;
using Xunit;
using Xunit.Abstractions;

namespace Niam.XRM.ECGenerator.Core.Tests
{
    public class EntityClassGeneratorTest
    {
        private readonly ITestOutputHelper _output;

        public EntityClassGeneratorTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Can_generate()
        {
            var entityMetadata = new EntityMetadata
            {
                LogicalName = "businessunit",
                SchemaName = "BusinessUnit",
                OwnershipType = OwnershipTypes.None
            };

            typeof(EntityMetadata).GetProperty("PrimaryIdAttribute").SetValue(entityMetadata, "businessunitid");
            typeof(EntityMetadata).GetProperty("Attributes").SetValue(entityMetadata, new AttributeMetadata[] { });

            var result = new EntityClassGenerator(entityMetadata).Generate();
            _output.WriteLine(result);
            Assert.Equal(ExpectedTemplate.EntityClass, result);
        }

        [Theory]
        [InlineData(OwnershipTypes.OrganizationOwned, "OrganizationEntity")]
        [InlineData(OwnershipTypes.UserOwned, "UserOrTeamEntity")]
        [InlineData(OwnershipTypes.TeamOwned, "UserOrTeamEntity")]
        public void Can_get_base_class_name(OwnershipTypes ownership, string expected)
        {
            var entityMetadata = new EntityMetadata
            {
                LogicalName = "entity",
                OwnershipType = ownership
            };

            var generator = new EntityClassGenerator(entityMetadata);
            Assert.Equal("Entity", generator.GenerateBaseClassName());

            typeof(EntityMetadata).GetProperty("IsCustomEntity").SetValue(entityMetadata, true);
            Assert.Equal(expected, generator.GenerateBaseClassName());
        }
    }
}
