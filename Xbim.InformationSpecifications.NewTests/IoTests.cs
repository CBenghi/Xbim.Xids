using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Xbim.InformationSpecifications.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Xbim.InformationSpecifications.Tests
{
    public class IoTests
    {
        public IoTests(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }
        private ITestOutputHelper OutputHelper { get; }

        internal ILogger<BuildingSmartIDSLoadTests> GetXunitLogger()
        {
            var services = new ServiceCollection()
                        .AddLogging((builder) => builder.AddXUnit(OutputHelper));
            IServiceProvider provider = services.BuildServiceProvider();
            var logg = provider.GetRequiredService<ILogger<BuildingSmartIDSLoadTests>>();
            Assert.NotNull(logg);
            return logg;
        }

        [Fact]
        public void CanIgnoreAnnotationsInRestriction()
        {
            var f = new FileInfo(@"bsFiles/bsFilesSelf/annotation.ids");
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
            var f = new FileInfo(@"bsFiles/others/pass-name_restrictions_will_match_any_result_1_3.ids");
            f.Exists.Should().BeTrue("file is deployed with test suite");
            Xids.CanLoad(f).Should().BeTrue();

            var xids = Xids.Load(f);
            Assert.NotNull(xids);
            var spec = xids.AllSpecifications().FirstOrDefault();
            spec.Should().NotBeNull();
            Assert.NotNull(spec);

            var attr = spec.Requirement!.Facets.First();
            attr.Should().BeOfType<AttributeFacet>();
            var asAttr = attr.As<AttributeFacet>();
            Assert.NotNull(asAttr.AttributeName);
            var av1 = asAttr.AttributeName.AcceptedValues!.First();
            av1.Should().NotBeNull();
            av1.Should().BeOfType<PatternConstraint>();
        }

        [Fact]
        public void CanLoadRestrictionXml2()
        {
            var file = new FileInfo(@"bsFiles/bsFilesSelf/TestFile.ids");
            Xids.CanLoad(file).Should().BeTrue();

            var xids = Xids.Load(file, GetXunitLogger());
            Assert.NotNull(xids);
            var spec = xids.AllSpecifications().Single();

            var reqs = spec.Requirement;
            Assert.NotNull(reqs);

            var attr = reqs.Facets.First().As<AttributeFacet>();
            Assert.NotNull(attr);
            attr.AttributeName.Should().NotBeNull();
            var av1 = attr.AttributeName!.AcceptedValues!.First();
            av1.Should().NotBeNull();
            av1.Should().BeOfType<PatternConstraint>();

            reqs.Facets.Should().HaveCount(4);
            
            var ifcType = reqs.Facets[1].As<IfcTypeFacet>();
            Assert.NotNull(ifcType);
            av1 = ifcType.IfcType!.AcceptedValues![0];
            av1.Should().NotBeNull();
            av1.Should().BeOfType<ExactConstraint>();
            ifcType.IfcType.AcceptedValues.Should().HaveCount(2);

            // Pset
            var pset = spec.Requirement!.Facets[2];
            pset.Should().BeOfType<IfcPropertyFacet>();

            var psType = pset.As<IfcPropertyFacet>();
            Assert.NotNull(psType.PropertyName);
            av1 = psType.PropertyName.AcceptedValues!.First();
            av1.Should().NotBeNull();

            av1.Should().BeOfType<PatternConstraint>();
            psType.PropertyName.AcceptedValues.Should().HaveCount(1);

            // Material without value
            var mat = spec.Requirement.Facets[3];
            mat.Should().BeOfType<MaterialFacet>();

            var matType = mat.As<MaterialFacet>();
            matType.Value.Should().BeNull();
        }

        [Fact]
        public void CanLoadOptionalFacet()
        {
            var f = new FileInfo(@"bsFiles/bsFilesSelf/OptionalFacets.ids");
            Xids.CanLoad(f).Should().BeTrue();

            var xids = Xids.Load(f);
            Assert.NotNull(xids);

            var spec = xids.AllSpecifications().FirstOrDefault();
            Assert.NotNull(spec);
            Assert.NotNull(spec.Requirement);

            var attr = spec.Requirement.Facets.FirstOrDefault();
            attr.Should().BeOfType<AttributeFacet>();
            spec.Requirement.RequirementOptions.Should().HaveCount(1);
            spec.Requirement.RequirementOptions!.First().Should().Be(RequirementCardinalityOptions.Optional);
        }


        [Fact]
        public void CanLoadXml()
        {
            var f = new FileInfo(@"Files/IDS_example-with-restrictions.xml");
            Xids.CanLoad(f).Should().BeTrue();

            var x = Xids.Load(f);
            x.Should().NotBeNull();
        }

        [Fact]
        public void CanLoadJson()
        {
            var f = new FileInfo(@"Files/newFormat.json");
            Xids.CanLoad(f).Should().BeTrue();

            var x = Xids.Load(f);
            x.Should().NotBeNull();
        }

        [Fact]
        public void WarnsJsonVersion()
        {
            var loggerMock = new Mock<ILogger<BuildingSmartCompatibilityTests>>(); // this is to check events
            var f = new FileInfo(@"Files/FutureFormat.json");
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

        [Fact]
        public void CanSaveXmlAsZip()
        {
            Xids? x = BuildMultiSpecGroupIDS();

            using var ms = new MemoryStream();

            x.ExportBuildingSmartIDS(ms);

            // Check the stream is a PK Zip stream by looking at 'magic' first 4 bytes
            Xids.IsZipped(ms).Should().BeTrue();
        }

        [Fact]
        public void ZippedIDSSpecsContainsIDS()
        {
            Xids? x = BuildMultiSpecGroupIDS();

            using var ms = new MemoryStream();

            x.ExportBuildingSmartIDS(ms);

            // Check Contains IDS files & content
            using var archive = new ZipArchive(ms, ZipArchiveMode.Read, false);

            archive.Entries.Should().HaveCount(2);

            archive.Entries.Should().AllSatisfy(e => e.Name.Should().EndWith(".ids", "IDS file extension expected"));
            archive.Entries.Should().AllSatisfy(e => e.Length.Should().BeGreaterThan(0, "Content expected"));
        }

        [Fact]
        public void CanLoadXmlAsZip()
        {
            Xids? x = BuildMultiSpecGroupIDS();

            using var ms = new MemoryStream();

            x.ExportBuildingSmartIDS(ms);
            ms.Position = 0;

            var newIds = Xids.LoadBuildingSmartIDS(ms);

            newIds.Should().NotBeNull();
            newIds!.SpecificationsGroups.Should().HaveCount(2);
        }

        [Fact]
        public void CanLoadXmlFromZipFile()
        {
            Xids? x = BuildMultiSpecGroupIDS();
            var tempXmlFile = Path.ChangeExtension(Path.GetTempFileName(), "zip");
            using (var fs = new FileStream(tempXmlFile, FileMode.Create))
            {
                x.ExportBuildingSmartIDS(fs);
            }

            var newIds = Xids.LoadBuildingSmartIDS(tempXmlFile);

            newIds.Should().NotBeNull();
            newIds!.SpecificationsGroups.Should().HaveCount(2);
        }

        private static Xids BuildMultiSpecGroupIDS()
        {
            var file = new FileInfo(@"bsFiles/bsFilesSelf/TestFile.ids");
            var x = Xids.Load(file);
            x.Should().NotBeNull();
            x!.AllSpecifications().Should().HaveCount(1);

            // Add a 2nd group with a basic spec
            var specGroup = new SpecificationsGroup(x);
            x.SpecificationsGroups.Add(specGroup);

            var newSpec = x.PrepareSpecification(specGroup, IfcSchemaVersion.Undefined);
            newSpec.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "Door" });

            x!.AllSpecifications().Should().HaveCount(2);
            return x;
        }
    }
}
