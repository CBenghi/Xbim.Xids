using FluentAssertions;
using System.Diagnostics;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{
    public class VersionTests
    {
        [Fact]
        public void HardCodedVersion_Matches()
        {
            var assembly = typeof(Xids).Assembly;
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            Xids.AssemblyVersion.Should().Be(fileVersion.FileVersion); // <== fix Xids.AssemblyVersion
        }
    }
}
