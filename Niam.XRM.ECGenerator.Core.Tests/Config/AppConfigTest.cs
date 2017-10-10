using Niam.XRM.ECGenerator.Core.Config;
using Xunit;

namespace Niam.XRM.ECGenerator.Core.Tests.Config
{
    public class AppConfigTest
    {
        [Fact]
        public void Can_parse_config_json()
        {
            var config = AppConfig.Parse("config.json");
            Assert.Equal("This.Is.A.Namespace", config.Namespace);
            Assert.Equal("this-is-connection-string", config.ConnectionString);
            Assert.True(config.GenerateExtensions);
        }
    }
}
