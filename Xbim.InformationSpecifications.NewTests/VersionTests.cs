using FluentAssertions;
using System.Diagnostics;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{
    public class VersionTests
    {
        /// <summary>
        /// This makes sure that the <see cref="Xids.AssemblyVersion"/> is kept up to date.
        /// </summary>
        [Fact]
        public void HardCodedVersion_Matches()
        {
            var assembly = typeof(Xids).Assembly;
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            Xids.AssemblyVersion.Should().Be(fileVersion.FileVersion); // <== fix Xids.AssemblyVersion
        }
    }
}
