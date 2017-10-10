using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Niam.XRM.ECGenerator.Core.Templates;
using Niam.XRM.ECGenerator.Core.Tests.Templates;
using Xunit;
using Xunit.Abstractions;

namespace Niam.XRM.ECGenerator.Core.Tests
{
    public class OptionsGeneratorTest
    {
        private readonly ITestOutputHelper _output;

        public OptionsGeneratorTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Can_generate()
        {
            var openOption = new OptionMetadata(1)
            {
                Label = new Label { UserLocalizedLabel = new LocalizedLabel("Open", 1033) }
            };

            var releaseOption = new OptionMetadata(2)
            {
                Label = new Label { UserLocalizedLabel = new LocalizedLabel("Release", 1033) }
            };

            var open1Option = new OptionMetadata(3)
            {
                Label = new Label { UserLocalizedLabel = new LocalizedLabel("Open", 1033) }
            };

            var release2Option = new OptionMetadata(4)
            {
                Label = new Label { UserLocalizedLabel = new LocalizedLabel("Release", 1033) }
            };

            var release3Option = new OptionMetadata(5)
            {
                Label = new Label { UserLocalizedLabel = new LocalizedLabel("Release", 1033) }
            };

            var optionSet = new OptionSetMetadata();
            optionSet.Options.AddRange(openOption, releaseOption, open1Option, release2Option, release3Option);

            var picklistMetadata = new PicklistAttributeMetadata
            {
                SchemaName = "xts_status",
                OptionSet = optionSet
            };

            var generator = new OptionsGenerator(picklistMetadata, Template.Options);
            var result = generator.Generate();
            _output.WriteLine(result);
            Assert.Equal(ExpectedTemplate.Options, result);
        }
    }
}
