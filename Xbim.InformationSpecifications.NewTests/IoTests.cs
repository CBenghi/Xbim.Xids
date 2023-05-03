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
        public void CanIgnoreAnnotationsInRestriction()
        {
            var f = new FileInfo(@"bsFiles\bsFilesSelf\annotation.ids");
            Xids.CanLoad(f).Should().BeTrue();
            var x = Xids.Load(f);
            x.Should().NotBeNull();
            Assert.NotNull(x);
            var spec = x.AllSpecifications().First();
            var tpf = spec.Applicability.Facets.OfType<IfcTypeFacet>().First();
            tpf.Should().NotBeNull();
            Assert.NotNull(tpf);
            var tp = tpf.IfcType as ValueConstraint;
            tp.Should().NotBeNull();
            Assert.NotNull(tp);
            var frst = tp.AcceptedValues!.First();
            frst.Should().BeOfType<PatternConstraint>();



        }

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
        public void CanLoadRestrictionXml2()
        {
            var f = new FileInfo(@"bsFiles\bsFilesSelf\TestFile.ids");
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

            spec.Requirement.Facets.Should().HaveCount(4); 

            var ifcType = spec.Requirement.Facets[1];
            ifcType.Should().BeOfType<IfcTypeFacet>();

            var asType = ifcType.As<IfcTypeFacet>();
            asType.IfcType.Should().NotBeNull();

            av1 = asType.IfcType.AcceptedValues[0];
            av1.Should().NotBeNull();

            av1.Should().BeOfType<ExactConstraint>();
            asType.IfcType.AcceptedValues.Should().HaveCount(2);


            // Pset

            var pset = spec.Requirement.Facets[2];
            pset.Should().BeOfType<IfcPropertyFacet>();

            var psType = pset.As<IfcPropertyFacet>();
            Assert.NotNull(psType.PropertyName);
            av1 = psType.PropertyName.AcceptedValues.First();
            av1.Should().NotBeNull();

            av1.Should().BeOfType<PatternConstraint>();

            psType.PropertyName.AcceptedValues.Count().Should().Be(1);

            // Material without value
            var mat = spec.Requirement.Facets[3];
            mat.Should().BeOfType<MaterialFacet>();

            var matType = mat.As<MaterialFacet>();
            matType.Value.Should().BeNull();
        }

        [Fact]
        public void CanLoadOptionalFacet()
        {
            var f = new FileInfo(@"bsFiles\bsFilesSelf\OptionalFacets.ids");
            Xids.CanLoad(f).Should().BeTrue();

            var xids = Xids.Load(f);
            Assert.NotNull(xids);

            var spec = xids.AllSpecifications().FirstOrDefault();
            Assert.NotNull(spec);
            Assert.NotNull(spec.Requirement);

            var attr = spec.Requirement.FirstOrDefault();
            attr.Should().BeOfType<AttributeFacet>();
            spec.Requirement.RequirementOptions.Should().HaveCount(1);
            spec.Requirement.RequirementOptions!.First().Should().Be(RequirementCardinalityOptions.Optional);
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
            LoggingTestHelper.SomeIssues(loggerMock);

            
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
            LoggingTestHelper.NoIssues(loggerMock);
            File.Delete(filename);
        }
    }
}
