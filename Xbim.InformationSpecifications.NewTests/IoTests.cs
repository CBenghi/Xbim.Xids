using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.InformationSpecifications.Tests.Helpers;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{
    public class IoTests
    {
        [Fact]
        public void CanLoadRestrictionXml()
        {
            var f = new FileInfo(@"bsFiles\Others\pass-name_restrictions_will_match_any_result_1_3.ids");
            Xids.CanLoad(f).Should().BeTrue();

            var x = Xids.Load(f);
            x.Should().NotBeNull();

            var spec = x.AllSpecifications().FirstOrDefault();
            spec.Should().NotBeNull();

            var attr = spec.Requirement.Facets[0];
            attr.Should().BeOfType<AttributeFacet>();

            var asAttr = attr.As<AttributeFacet>();
            asAttr.AttributeName.Should().NotBeNull();

            var av1 = asAttr.AttributeName.AcceptedValues[0];
            av1.Should().NotBeNull();

            av1.Should().BeOfType<PatternConstraint>();

            


        }

        [Fact]
        public void CanLoadXml()
        {
            var f = new FileInfo(@"Files\IDS_example-with-restrictions.xml");
            Xids.CanLoad(f).Should().BeTrue();

            var x = Xids.Load(f);
            x.Should().NotBeNull();
        }

        [Fact]
        public void CanLoadJson()
        {
            var f = new FileInfo(@"Files\newFormat.json");
            Xids.CanLoad(f).Should().BeTrue();

            var x = Xids.Load(f);
            x.Should().NotBeNull();
        }

        [Fact]
        public void WarnsJsonVersion()
        {
            var loggerMock = new Mock<ILogger<BuildingSmartCompatibilityTests>>(); // this is to check events
            var f = new FileInfo(@"Files\FutureFormat.json");
            Xids.CanLoad(f, loggerMock.Object).Should().BeTrue();
            var loggingCalls = loggerMock.Invocations.Select(x => x.ToString()).ToArray(); // this creates the array of logging calls
            loggingCalls.Where(x => x.Contains("Error") || x.Contains("Warning")).Should().NotBeEmpty("a calls to warning is expected");
        }


        [Fact]
        public void NoWarnsOnCurrentJsonVersion()
        {
            Xids x = XidsTestHelpers.GetSimpleXids();
            var filename = Path.GetTempFileName();
            x.SaveAsJson(filename);
            Assert.True(File.Exists(filename));

            var loggerMock = new Mock<ILogger<BuildingSmartCompatibilityTests>>(); // this is to check events
            var f = new FileInfo(filename);
            Debug.WriteLine(f.FullName);
            Xids.CanLoad(f, loggerMock.Object).Should().BeTrue();
            var loggingCalls = loggerMock.Invocations.Select(x => x.ToString()).ToArray(); // this creates the array of logging calls
            loggingCalls.Where(x => x.Contains("Error") || x.Contains("Warning")).Should().BeEmpty("no calls to warning is expected");

            File.Delete(filename);
        }
    }
}
