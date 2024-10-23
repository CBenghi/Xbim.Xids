#pragma warning disable xUnit1042
using FluentAssertions;
using IdsLib;
using IdsLib.IdsSchema.IdsNodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xbim.Ifc4.Interfaces;
using Xunit;
using Xunit.Abstractions;
using static Xbim.InformationSpecifications.Xids;

namespace Xbim.InformationSpecifications.Tests
{
    public class BuildingSmartCompatibilityTests
    {
        public BuildingSmartCompatibilityTests(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        private readonly ITestOutputHelper OutputHelper;

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
		public void InheritanceTypeIdsIoTest()
		{
			var x = new Xids();
			// at least one specification is needed
			//
			var t = x.PrepareSpecification(IfcSchemaVersion.IFC2X3);
			t.Requirement!.Facets.Add(new IfcTypeFacet() { IfcType = "IFCWALL", IncludeSubtypes = true });
			t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IFCWALL" });
			t.Instructions = "Some instructions";

			// ensure it's there.
			Assert.Single(x.AllSpecifications());

			// export
			var tmpFile = Path.GetTempFileName();
			x.ExportBuildingSmartIDS(tmpFile);

            // should have ifcWallStandardCase
            //
            var read = File.ReadAllText(tmpFile);
            read.Should().Contain("IFCWALLSTANDARDCASE");

			// audit schema
			//
			var c = Validate(tmpFile, GetXunitLogger());
			c.Should().Be(Audit.Status.Ok);

            // reload and check inheritance
            var reloaded = Xids.LoadBuildingSmartIDS(tmpFile);
            reloaded.Should().NotBeNull();

            var spec = reloaded!.AllSpecifications().Single();
            spec.Applicability.Should().NotBeNull();
            var type = spec!.Applicability.Facets.OfType<IfcTypeFacet>().First();
            type.IncludeSubtypes.Should().Be(true);
            var single = type.IfcType!.IsSingleExact<string>(out var typename);
            single.Should().Be(true);
            typename!.ToUpperInvariant().Should().Be("IFCWALL");

            // cleanup
			File.Delete(tmpFile);
		}


        [Fact]
        public void CanSaveExplictInheritance()
        {
            var logger = GetXunitLogger();
            var x = new Xids();
            // at least one specification is needed
            // at least one specification is needed
            //
            var t = x.PrepareSpecification(IfcSchemaVersion.IFC2X3);
            t.Requirement!.Facets.Add(new IfcTypeFacet() { IfcType = "IFCWALL" /* IncludeSubtypes defaults True */});
            var typeFacet = new IfcTypeFacet()
            {
                IfcType = new ValueConstraint()
            };
            // We should be able to add types explicitly, even when IncludeSubTypes = true
            typeFacet.IfcType.AddAccepted(new ExactConstraint("IFCWALL"));
            typeFacet.IfcType.AddAccepted(new ExactConstraint("IFCWALLSTANDARDCASE"));

            t.Applicability.Facets.Add(typeFacet);
            
            
            t.Instructions = "Some instructions";

            // ensure it's there.
            Assert.Single(x.AllSpecifications());

            // export
            var tmpFile = Path.GetTempFileName();
            x.ExportBuildingSmartIDS(tmpFile, logger);

            // should have ifcWallStandardCase
            //
            var read = File.ReadAllText(tmpFile);
            read.Should().Contain("IFCWALLSTANDARDCASE");

            // audit schema
            //
            var c = Validate(tmpFile, logger);
            c.Should().Be(Audit.Status.Ok);

            // reload and check inheritance
            var reloaded = Xids.LoadBuildingSmartIDS(tmpFile);
            reloaded.Should().NotBeNull();

            var spec = reloaded!.AllSpecifications().Single();
            spec.Applicability.Should().NotBeNull();
            var type = spec!.Applicability.Facets.OfType<IfcTypeFacet>().First();
            type.IncludeSubtypes.Should().Be(true);
            var single = type.IfcType!.IsSingleExact<string>(out var typename);
            single.Should().Be(true);
            typename!.ToUpperInvariant().Should().Be("IFCWALL");

            // cleanup
            File.Delete(tmpFile);
        }

        [Fact]
        public void MinimalFileExportTest()
        {
            var x = new Xids();
            // at least one specification is needed
            //
            var t = x.PrepareSpecification(IfcSchemaVersion.IFC2X3);
            t.Requirement!.Facets.Add(new IfcTypeFacet() { IfcType = "IFCWALL" });
            t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IFCWALL" });
            t.Instructions = "Some instructions";

            // ensure it's there.
            Assert.Single(x.AllSpecifications());

            // export
            var tmpFile = Path.GetTempFileName();
            x.ExportBuildingSmartIDS(tmpFile);

            // check schema
            //
            var c = Validate(tmpFile, GetXunitLogger());
            c.Should().Be(Audit.Status.Ok);
            File.Delete(tmpFile);
        }

        [Fact]
        public void CanRoundtripSpecificationIdentifier()
        {
            var id = Guid.NewGuid().ToString();
            var x = new Xids();
            // at least one specification is needed
            //
            var t = x.PrepareSpecification(IfcSchemaVersion.IFC2X3);
            t.Requirement!.Facets.Add(new IfcTypeFacet() { IfcType = "IFCWALL" });
            t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IFCWALL" });
            t.Instructions = "Some instructions";
            t.Guid = id;

            // export
            var tmpFile = Path.GetTempFileName();
            x.ExportBuildingSmartIDS(tmpFile);

            // check schema is valid with the identifier persisted
            var c = Validate(tmpFile, GetXunitLogger());
            c.Should().Be(Audit.Status.Ok);

            var round = LoadBuildingSmartIDS(tmpFile);
            var roundSpec = round?.SpecificationsGroups.FirstOrDefault()?.Specifications?.FirstOrDefault();
            roundSpec.Should().NotBeNull();
            roundSpec!.Guid.Should().Be(id);

            File.Delete(tmpFile);
        }

        [Fact]
        public void DoubleFileExportTest()
        {
            Xids x = new();
            // at least one specification is needed
            //
            var t = x.PrepareSpecification(IfcSchemaVersion.IFC2X3);
            t.Requirement!.Facets.Add(new IfcTypeFacet() { IfcType = "IFCWINDOW" });
            t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IFCWALL" });

            var newGroup = new SpecificationsGroup(x);
            x.SpecificationsGroups.Add(newGroup);

            t = x.PrepareSpecification(newGroup, IfcSchemaVersion.IFC4);
            t.Requirement!.Facets.Add(new IfcTypeFacet() { IfcType = "IFCWALL" });
            t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IFCWINDOW" });

            // export
            var tmpFile = Path.GetTempFileName();
            var type = x.ExportBuildingSmartIDS(tmpFile);
            type.Should().Be(ExportedFormat.ZIP, "multiple groups are defined in the file");

            using var archive = ZipFile.OpenRead(tmpFile);
            archive.Entries.Count.Should().Be(2);
        }

        private const string IdsTestcasesPath = @"..\..\..\..\..\..\BuildingSmart\IDS\Documentation\testcases";

        [Theory]
        [MemberData(nameof(GetIdsFiles))]    
        public void CanReadIdsSamples(string idsFile)
        {
            if (idsFile == string.Empty)
                return;

            var d = new DirectoryInfo(IdsTestcasesPath);
            var comb = d.FullName + idsFile;
            var f = new FileInfo(comb);
            f.Exists.Should().BeTrue("test file must be found");

            var logger = GetXunitLogger();
            logger.LogInformation("Processing `{filename}`", f.FullName);

			// first we try to load and see if it works with xunit logging
			var loadedIds = LoadBuildingSmartIDS(f.FullName, logger);
            loadedIds.Should().NotBeNull();

            // then we load again with mock logger to ensure there are no errors    
            var loggerMock = Substitute.For<ILogger<BuildingSmartCompatibilityTests>>(); // this is to check events
            loadedIds = LoadBuildingSmartIDS(f.FullName, loggerMock);
            loadedIds.Should().NotBeNull();
			var errorAndWarnings = loggerMock.ReceivedCalls().Where(call => call.IsErrorType(true, true, true));
			errorAndWarnings.Should().BeEmpty();
        }

        public static IEnumerable<object[]> GetIdsFiles()
        {
            // start from current directory and look in relative position for the bs IDS repository
            var d = new DirectoryInfo(IdsTestcasesPath);
            if (!d.Exists)
            {
                yield return new[] { string.Empty };
                yield break;
            }
            foreach (var f in d.GetFiles("*.ids", SearchOption.AllDirectories))
            {
                yield return new object[] { f.FullName.Replace(d.FullName, "") };
            }
            
        }

        [Theory]
        [InlineData("bsFiles/bsFilesSelf/TestFile.ids")]
        public void FullSchemaImportTest(string fileName)
        {
            var res = Validate(fileName, GetXunitLogger());
            res.Should().Be(Audit.Status.Ok, "the input file needs to be valid");
            var x = LoadBuildingSmartIDS(fileName);
            Assert.NotNull(x);
            var exportedFile = Path.GetTempFileName();

            ILogger<BuildingSmartIDSLoadTests> logg = GetXunitLogger();
            var loggerMock = Substitute.For<ILogger<BuildingSmartCompatibilityTests>>(); // this is to check events
            _ = x.ExportBuildingSmartIDS(exportedFile, loggerMock);
            _ = x.ExportBuildingSmartIDS(exportedFile, logg);

            GetXunitLogger().LogInformation("===== Logging exported file: {exportedFile}", exportedFile);
			LoggingTestHelper.NoIssues(loggerMock);
            res = Validate(exportedFile, GetXunitLogger());
            res.Should().Be(Audit.Status.Ok , "the generated file needs to be valid");

            // we should be able to save our format
            var exportedJsonFile = Path.GetTempFileName();
            x.SaveAsJson(exportedJsonFile);

            // more checks
            var outputCount = XmlReport(exportedFile);
            var inputCount = XmlReport(fileName);

            var fd = inputCount.FirstDifference(outputCount);
            fd.Should().Be("", "we don't expect differences");
            // outputCount.Should().Be(inputCount, "everything should be exported");
        }


		private static Audit.Status Validate(string fileName, ILogger? logger)
        {
            var opt = new SingleAuditOptions();
            using var stream = File.OpenRead(fileName);           
            var validationResult = Audit.Run(stream, opt, logger);
            return validationResult;
        }

        static private XmlElementSummary XmlReport(string tmpFile)
        {
            var main = XElement.Parse(File.ReadAllText(tmpFile));
            XmlElementSummary summary = new(main, null);
            return summary;
        }

        private static (int elements, int attributes) Count(XElement main)
        {
            var t = (elements: main.Elements().Count(), attributes: main.Attributes().Count());
            foreach (var sub in main.Elements())
            {
                var (elements, attributes) = Count(sub);
                t.elements += elements;
                t.attributes += attributes;
            }
            return t;
        }

        private class XmlElementSummary
        {
            public string type;
            public XmlElementSummary? parent;
            public int attributes = 0;
            public List<XmlElementSummary> Subs = new();

            public XmlElementSummary(XElement main, XmlElementSummary? parent)
            {
                type = main.Name.LocalName;
                this.parent = parent;
                attributes = main.Attributes().Count();
                Subs = main.Elements().Select(x => new XmlElementSummary(x, this)).ToList();
            }

            public string FirstDifference(XmlElementSummary other)
            {
                if (other == null)
                    return ReportDifference("Other is null");
                if (this.attributes != other.attributes)
                    return ReportDifference("Different attributes count");
                if (this.Subs.Count != other.Subs.Count)
                    return ReportDifference("Different elements count");
                for (int i = 0; i < Subs.Count; i++)
                {
                    var thisSub = this.Subs[i];
                    var otherSub = other.Subs[i];
                    var fd = thisSub.FirstDifference(otherSub);
                    if (string.IsNullOrEmpty(fd))
                        return fd;
                }
                return "";
            }

            private string ReportDifference(string message)
            {
                StringBuilder sb = new();

                Stack<XmlElementSummary> parents = new();
                var running = this;
                while (running.parent != null)
                {
                    parents.Push(running.parent);
                    running = running.parent;
                }
                var indent = "";
                while (parents.TryPop(out var current))
                {
                    sb.Append($"{indent}{current.type} - A: {current.attributes}");
                    indent += "\t";
                }
                sb.Append($"{indent}{message}");
                return sb.ToString();

            }
        }

        private static Xids GetSpec(IFacet tpFacet, IFacet partFacet)
        {
            var x = new Xids();
            var t = x.PrepareSpecification(IfcSchemaVersion.IFC4);
            t.Applicability.Facets.Add(tpFacet);
            t.Requirement!.Facets.Add(partFacet);
            return x;
        }
    }
}
