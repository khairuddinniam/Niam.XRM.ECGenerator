using Niam.XRM.ECGenerator.Core.Metadata;
using Xunit;

namespace Niam.XRM.ECGenerator.Core.Tests
{
    public class JsonSerializerTest
    {
        [Fact]
        public void Can_deserialize_attribute_info()
        {
            var info = JsonSerializer.Deserialize<AttributeInfo>("{\"a\":0}");
            Assert.Equal(0, info.StateCodeActiveValue);

            info = JsonSerializer.Deserialize<AttributeInfo>("{}");
            Assert.Null(info.StateCodeActiveValue);
        }

        [Fact]
        public void Can_serialize_attribute_info()
        {
            var info = new AttributeInfo
            {
                StateCodeActiveValue = 12
            };
            Assert.Equal("{\"a\":12}", JsonSerializer.Serialize(info));

            info = new AttributeInfo
            {
                StateCodeActiveValue = null
            };
            Assert.Equal("{}", JsonSerializer.Serialize(info));
        }
    }
}
