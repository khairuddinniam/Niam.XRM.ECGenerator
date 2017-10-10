using System.Linq;
using Microsoft.Xrm.Sdk.Metadata;
using Niam.XRM.ECGenerator.Core.ExtensionsGenerator;
using Niam.XRM.ECGenerator.Core.Tests.ExtensionsGenerator.Templates;
using Xunit;

namespace Niam.XRM.ECGenerator.Core.Tests.ExtensionsGenerator
{
    public class EntityMetadataHolderTest
    {
        [Fact]
        public void Can_convert_to_holders()
        {
            var teamMetadata = new EntityMetadata
            {
                LogicalName = "team",
                SchemaName = "team"
            };
            typeof(EntityMetadata).GetProperty("Attributes").SetValue(teamMetadata, new AttributeMetadata[] { });

            var userMetadata = new EntityMetadata
            {
                LogicalName = "systemuser",
                SchemaName = "systemuser"
            };
            typeof(EntityMetadata).GetProperty("Attributes").SetValue(userMetadata, new AttributeMetadata[] { });

            var productNameMetadata = new StringAttributeMetadata
            {
                LogicalName = "xts_name",
                SchemaName = "xts_name"
            };

            var productMetadata = new EntityMetadata
            {
                LogicalName = "xts_product",
                SchemaName = "xts_product"
            };
            typeof(EntityMetadata).GetProperty("Attributes").SetValue(productMetadata, new AttributeMetadata[] { productNameMetadata });

            var salesOrderProductMetadata = new LookupAttributeMetadata
            {
                LogicalName = "xts_productid",
                SchemaName = "xts_productid",
                Targets = new[] { "xts_product" }
            };

            var salesOrderOwnerMetadata = new LookupAttributeMetadata
            {
                LogicalName = "ownerid",
                SchemaName = "ownerid",
                Targets = new[] { "team", "systemuser" }
            };

            var salesOrderNameMetadata = new StringAttributeMetadata
            {
                LogicalName = "xts_name",
                SchemaName = "xts_name"
            };

            var salesOrderMetadata = new EntityMetadata
            {
                LogicalName = "xts_salesorder",
                SchemaName = "xts_salesorder"
            };

            typeof(EntityMetadata).GetProperty("Attributes").SetValue(salesOrderMetadata, new AttributeMetadata[]
            {
                salesOrderProductMetadata,
                salesOrderOwnerMetadata,
                salesOrderNameMetadata
            });

            var entitiesMetadata = new[]
            {
                teamMetadata,
                userMetadata,
                productMetadata,
                salesOrderMetadata
            };
            var results = EntityMetadataHolder.Convert(entitiesMetadata);

            Assert.Equal(1, results.Length);
            var entityHolder = results.First();
            Assert.Equal(salesOrderMetadata, entityHolder.EntityMetadata);
            Assert.Equal(3, entityHolder.LookupAttributeMetadataHolders.Length);
            var attributeHolders = entityHolder.LookupAttributeMetadataHolders
                .OrderBy(h => h.LookupMetadata.SchemaName)
                .ThenBy(h => h.LookupToEntityMetadata.SchemaName)
                .ToArray();

            Assert.Equal(salesOrderOwnerMetadata, attributeHolders[0].LookupMetadata);
            Assert.Equal(userMetadata, attributeHolders[0].LookupToEntityMetadata);

            Assert.Equal(salesOrderOwnerMetadata, attributeHolders[1].LookupMetadata);
            Assert.Equal(teamMetadata, attributeHolders[1].LookupToEntityMetadata);

            Assert.Equal(salesOrderProductMetadata, attributeHolders[2].LookupMetadata);
            Assert.Equal(productMetadata, attributeHolders[2].LookupToEntityMetadata);
        }

        [Fact]
        public void Can_generate()
        {
            var entityMetadata = new EntityMetadata
            {
                LogicalName = "businessunit",
                SchemaName = "BusinessUnit"
            };
            var holder = new EntityMetadataHolder(entityMetadata, new LookupAttributeMetadataHolder[0]);
            Assert.Equal(ExpectedTemplate.ExtensionClass, holder.Generate());
        }
    }
}
