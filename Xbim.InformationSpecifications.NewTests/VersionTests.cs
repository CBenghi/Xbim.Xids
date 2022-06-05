using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Xids.AssemblyVersion.Should().Be(fileVersion.FileVersion);
        }
    }
}
