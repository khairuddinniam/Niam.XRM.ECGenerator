using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Metadata;
using Xunit;

namespace Niam.XRM.ECGenerator.Core.Tests
{
    public class HelperTest
    {
        [Fact]
        public void IsUserOrTeamOwnership()
        {
            var metadata = new EntityMetadata();
            metadata.OwnershipType = OwnershipTypes.TeamOwned;
            Assert.True(metadata.IsUserOrTeamOwnership());
            metadata.OwnershipType = OwnershipTypes.UserOwned;
            Assert.True(metadata.IsUserOrTeamOwnership());
            metadata.OwnershipType = null;
            Assert.False(metadata.IsUserOrTeamOwnership());
            metadata.OwnershipType = OwnershipTypes.OrganizationOwned;
            Assert.False(metadata.IsUserOrTeamOwnership());
        }

        [Fact]
        public void IsOrganizationOwnership()
        {
            var metadata = new EntityMetadata();
            metadata.OwnershipType = OwnershipTypes.TeamOwned;
            Assert.False(metadata.IsOrganizationOwnership());
            metadata.OwnershipType = OwnershipTypes.UserOwned;
            Assert.False(metadata.IsOrganizationOwnership());
            metadata.OwnershipType = null;
            Assert.False(metadata.IsOrganizationOwnership());
            metadata.OwnershipType = OwnershipTypes.OrganizationOwned;
            Assert.True(metadata.IsOrganizationOwnership());
        }

        [Theory]
        [InlineData("   SpaceBefore", "SpaceBefore")]
        [InlineData("SpaceAfter  ", "SpaceAfter")]
        [InlineData("Space  Between", "SpaceBetween")]
        [InlineData("1234", "_1234")]
        [InlineData("Bad*chars", "BadChars")]
        [InlineData("class", "Class")]
        [InlineData("int", "Int")]
        [InlineData("smallBig", "SmallBig")]
        [InlineData("", null)]
        [InlineData(" ", null)]
        [InlineData(null, null)]
        [InlineData("@!#!$@%#^", null)]
        public void Can_adjust_enum_option_name(string input, string expected)
        {
            Assert.Equal(expected, Helper.AdjustName(input));
        }
    }
}
