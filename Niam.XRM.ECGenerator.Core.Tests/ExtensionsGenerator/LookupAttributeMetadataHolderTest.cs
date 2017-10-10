using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Niam.XRM.ECGenerator.Core.ExtensionsGenerator;
using Niam.XRM.ECGenerator.Core.Tests.ExtensionsGenerator.Templates;
using Xunit;

namespace Niam.XRM.ECGenerator.Core.Tests.ExtensionsGenerator
{
    public class LookupAttributeMetadataHolderTest
    {
        [Theory]
        [MemberData(nameof(GetGetMethodNameData))]
        public void Can_get_method_name(LookupAttributeMetadata lookupMetadata, EntityMetadata lookupToEntityMetadata, string expectedName)
        {
            var holder = new LookupAttributeMetadataHolder("EntitySchemaName", lookupMetadata, lookupToEntityMetadata);
            Assert.Equal(expectedName, holder.GetMethodName(null));
        }

        private static IEnumerable<object[]> GetGetMethodNameData()
        {
            // Single lookup, attribute have label
            yield return new object[]
            {
                new LookupAttributeMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("User Label", 1033)
                    },
                    SchemaName = "ThisIsASchemaName",
                    Targets = new[] { "xts_entity" }
                },

                new EntityMetadata
                {
                    LogicalName = "xts_entity",
                    SchemaName = "EntitySchemaName"
                },

                "GetUserLabel"
            };

            // Single lookup, attribute doesn't have label
            yield return new object[]
            {
                new LookupAttributeMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = null
                    },
                    SchemaName = "ThisIsASchemaName",
                    Targets = new[] { "xts_entity" }
                },

                new EntityMetadata
                {
                    LogicalName = "xts_entity",
                    SchemaName = "EntitySchemaName"
                },

                "GetThisIsASchemaName"
            };

            // Multi lookup, attribute have label, lookup entity have label
            yield return new object[]
            {
                new LookupAttributeMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("User Label", 1033)
                    },
                    SchemaName = "ThisIsASchemaName",
                    Targets = new[] { "xts_entity", "xts_anotherentity" }
                },

                new EntityMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("Entity Label", 1033)
                    },
                    LogicalName = "xts_entity",
                    SchemaName = "EntitySchemaName"
                },

                "GetUserLabel"
            };

            // Multi lookup, attribute have label, lookup entity doesn't have label
            yield return new object[]
            {
                new LookupAttributeMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("User Label", 1033)
                    },
                    SchemaName = "ThisIsASchemaName",
                    Targets = new[] { "xts_entity", "xts_anotherentity" }
                },

                new EntityMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = null
                    },
                    LogicalName = "xts_entity",
                    SchemaName = "EntitySchemaName"
                },

                "GetUserLabel"
            };

            // Multi lookup, attribute doesn't have label, lookup entity have label
            yield return new object[]
            {
                new LookupAttributeMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = null
                    },
                    SchemaName = "ThisIsASchemaName",
                    Targets = new[] { "xts_entity", "xts_anotherentity" }
                },

                new EntityMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("Entity Label", 1033)
                    },
                    LogicalName = "xts_entity",
                    SchemaName = "EntitySchemaName"
                },

                "GetThisIsASchemaName"
            };

            // Multi lookup, attribute doesn't have label, lookup entity doesn't have label
            yield return new object[]
            {
                new LookupAttributeMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = null
                    },
                    SchemaName = "ThisIsASchemaName",
                    Targets = new[] { "xts_entity", "xts_anotherentity" }
                },

                new EntityMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = null
                    },
                    LogicalName = "xts_entity",
                    SchemaName = "EntitySchemaName"
                },

                "GetThisIsASchemaName"
            };
        }

        [Theory]
        [MemberData(nameof(GetGetMethodNameWithBlackListData))]
        public void Can_get_method_name_with_blacklist(LookupAttributeMetadata lookupMetadata,
            EntityMetadata lookupToEntityMetadata, IEnumerable<string> blacklist,
            string expectedName)
        {
            var holder = new LookupAttributeMetadataHolder("EntitySchemaName", lookupMetadata, lookupToEntityMetadata);
            Assert.Equal(expectedName, holder.GetMethodName(blacklist));
        }

        private static IEnumerable<object[]> GetGetMethodNameWithBlackListData()
        {
            // Single lookup, blacklist GetUserLabel
            yield return new object[]
            {
                new LookupAttributeMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("User Label", 1033)
                    },
                    SchemaName = "ThisIsASchemaName",
                    Targets = new[] { "xts_entity" }
                },

                new EntityMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("Entity Label", 1033)
                    },
                    LogicalName = "xts_entity",
                    SchemaName = "EntitySchemaName"
                },

                new[] { "GetUserLabel" },
                "GetUserLabelEntityLabel"
            };

            // Single lookup, blacklist GetUserLabel and GetUserLabelEntityLabel
            yield return new object[]
            {
                new LookupAttributeMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("User Label", 1033)
                    },
                    SchemaName = "ThisIsASchemaName",
                    Targets = new[] { "xts_entity" }
                },

                new EntityMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("Entity Label", 1033)
                    },
                    LogicalName = "xts_entity",
                    SchemaName = "EntitySchemaName"
                },

                new[] { "GetUserLabel", "GetUserLabelEntityLabel" },
                "GetUserLabelEntityLabel1"
            };

            // Single lookup, blacklist GetUserLabel, GetUserLabelEntityLabel, and GetUserLabelEntityLabel1
            yield return new object[]
            {
                new LookupAttributeMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("User Label", 1033)
                    },
                    SchemaName = "ThisIsASchemaName",
                    Targets = new[] { "xts_entity" }
                },

                new EntityMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("Entity Label", 1033)
                    },
                    LogicalName = "xts_entity",
                    SchemaName = "EntitySchemaName"
                },

                new[] { "GetUserLabel", "GetUserLabelEntityLabel", "GetUserLabelEntityLabel1" },
                "GetUserLabelEntityLabel2"
            };

            // Multi lookup, blacklist GetUserLabelEntityLabel
            yield return new object[]
            {
                new LookupAttributeMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("User Label", 1033)
                    },
                    SchemaName = "ThisIsASchemaName",
                    Targets = new[] { "xts_entity", "xts_anotherentity" }
                },

                new EntityMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("Entity Label", 1033)
                    },
                    LogicalName = "xts_entity",
                    SchemaName = "EntitySchemaName"
                },

                new[] { "GetUserLabel" },
                "GetUserLabelEntityLabel"
            };

            // Multi lookup, blacklist GetUserLabelEntityLabel and GetUserLabelEntityLabel1
            yield return new object[]
            {
                new LookupAttributeMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("User Label", 1033)
                    },
                    SchemaName = "ThisIsASchemaName",
                    Targets = new[] { "xts_entity", "xts_anotherentity" }
                },

                new EntityMetadata
                {
                    DisplayName = new Label
                    {
                        UserLocalizedLabel = new LocalizedLabel("Entity Label", 1033)
                    },
                    LogicalName = "xts_entity",
                    SchemaName = "EntitySchemaName"
                },

                new[] { "GetUserLabel", "GetUserLabelEntityLabel", "GetUserLabelEntityLabel1" },
                "GetUserLabelEntityLabel2"
            };
        }

        [Fact]
        public void Can_generate_get_related_single_lookup()
        {
            var lookupAttributeMetadata = new LookupAttributeMetadata
            {
                DisplayName = new Label
                {
                    UserLocalizedLabel = new LocalizedLabel("User Label", 1033)
                },
                LogicalName = "xts_lookupid",
                SchemaName = "Xts_SchemaLookupId",
                Targets = new[] { "xts_entity" }
            };
            typeof(LookupAttributeMetadata).GetProperty("EntityLogicalName").SetValue(lookupAttributeMetadata, "xts_containerentity");

            var lookupToEntityMetadata = new EntityMetadata
            {
                LogicalName = "xts_entity",
                SchemaName = "EntitySchemaName"
            };

            var holder = new LookupAttributeMetadataHolder("MyEntity", lookupAttributeMetadata, lookupToEntityMetadata);
            var result = holder.Generate(null);
            Assert.Equal("GetUserLabel", result.MethodName);
            Assert.Equal(ExpectedTemplate.GetRelatedSingleLookup, result.Content);
        }

        [Fact]
        public void Can_generate_get_related_multi_lookup()
        {
            var lookupAttributeMetadata = new LookupAttributeMetadata
            {
                DisplayName = new Label
                {
                    UserLocalizedLabel = new LocalizedLabel("User Label", 1033)
                },
                LogicalName = "xts_lookupid",
                SchemaName = "Xts_SchemaLookupId",
                Targets = new[] { "xts_entity", "xts_anotherentity" }
            };
            typeof(LookupAttributeMetadata).GetProperty("EntityLogicalName").SetValue(lookupAttributeMetadata, "xts_containerentity");

            var lookupToEntityMetadata = new EntityMetadata
            {
                DisplayName = new Label
                {
                    UserLocalizedLabel = new LocalizedLabel("Entity Label", 1033)
                },
                LogicalName = "xts_entity",
                SchemaName = "EntitySchemaName"
            };

            var holder = new LookupAttributeMetadataHolder("MyEntity", lookupAttributeMetadata, lookupToEntityMetadata);
            var result = holder.Generate(null);
            Assert.Equal("GetUserLabel", result.MethodName);
            Assert.Equal(ExpectedTemplate.GetRelatedMultiLookup, result.Content);
        }
    }
}
