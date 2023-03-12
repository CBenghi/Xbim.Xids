using FluentAssertions;
using IdsLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;
using static Xbim.InformationSpecifications.Xids;

namespace Xbim.InformationSpecifications.Tests
{
    public class BuildingSmartCompatibilityTests
    {
        private readonly ITestOutputHelper OutputHelper;

        public BuildingSmartCompatibilityTests(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }


        [Fact]
        public void MinimalFileExportTest()
        {
            var x = new Xids();
            // at least one specification is needed
            //
            var t = x.PrepareSpecification(IfcSchemaVersion.IFC2X3);
            t.Requirement.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });
            t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });
            t.Instructions = "Some instructions";

            // ensure it's there.
            Assert.Single(x.AllSpecifications());

            // export
            var tmpFile = Path.GetTempFileName();
            x.ExportBuildingSmartIDS(tmpFile);

            // check schema
            //
            var c = GetValidator(tmpFile);

            StringWriter s = new();
            var res = CheckOptions.Run(c, s);
            if (res != CheckOptions.Status.Ok)
            {
                Debug.WriteLine(s.ToString());
            }
            Assert.Equal(CheckOptions.Status.Ok, res);
        }

        private static CheckOptions GetValidator(string tmpFile)
        {
            CheckOptions c = new()
            {
                CheckSchema = new[] { "bsFiles\\ids_09.xsd" },
                InputSource = tmpFile
            };
            return c;
        }

        [Fact]
        public void DoubleFileExportTest()
        {
            Xids x = new();
            // at least one specification is needed
            //
            var t = x.PrepareSpecification(IfcSchemaVersion.IFC2X3);
            t.Requirement.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWindow" });
            t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });

            var newGroup = new SpecificationsGroup(x);
            x.SpecificationsGroups.Add(newGroup);

            t = x.PrepareSpecification(newGroup, IfcSchemaVersion.IFC4);
            t.Requirement.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });
            t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWindow" });

            // export
            var tmpFile = Path.GetTempFileName();
            var type = x.ExportBuildingSmartIDS(tmpFile);
            type.Should().Be(ExportedFormat.ZIP, "multiple groups are defined in the file");

            using var archive = ZipFile.OpenRead(tmpFile);
            archive.Entries.Count.Should().Be(2);
        }

        internal ILogger<BuildingSmartIDSLoadTests> GetXunitLogger()
        {
            var services = new ServiceCollection()
                        .AddLogging((builder) => builder.AddXUnit(OutputHelper));
            IServiceProvider provider = services.BuildServiceProvider();
            var logg = provider.GetRequiredService<ILogger<BuildingSmartIDSLoadTests>>();
            Assert.NotNull(logg);
            return logg;
        }

        private const string IdsTestcasesPath = @"..\..\..\..\..\..\BuildingSmart\IDS\Documentation\testcases";

        [Theory]
        [MemberData(nameof(GetIdsFiles))]    
        public void CanReadIdsSamples(string idsFile)
        {
            var d = new DirectoryInfo(IdsTestcasesPath);
            var comb = d.FullName + idsFile;
            FileInfo f = new FileInfo(comb);
            f.Exists.Should().BeTrue("test file must be found");
                
            var loggerMock = new Mock<ILogger<BuildingSmartCompatibilityTests>>(); // this is to check events
            var x = LoadBuildingSmartIDS(f.FullName, loggerMock.Object);
            x.Should().NotBeNull();
            var loggingIssues = loggerMock.Invocations.Where(
                w => w.Arguments[0].ToString() == "Error" || w.Arguments[0].ToString() == "Warning"
                ).Select(s => s.Arguments[2].ToString()).ToArray(); // this creates the array of logging calls
            loggingIssues.Should().BeEmpty();
        }

        public static IEnumerable<object[]> GetIdsFiles()
        {
            // start from current directory and look in relative position for the bs IDS repository
            var d = new DirectoryInfo(IdsTestcasesPath);
            foreach (var f in d.GetFiles("*.ids", SearchOption.AllDirectories))
            {
                yield return new object[] { f.FullName.Replace(d.FullName, "") };
            }
            
        }

        [Theory]
        [InlineData("bsFiles/bsFilesSelf/SimpleValueString.ids")]
        [InlineData("bsFiles/bsFilesSelf/SimpleValueRestriction.ids")]
        [InlineData("bsFiles/bsFilesSelf/TestFile.ids")]
        public void FullSchemaImportTest(string fileName)
        {
            var res = Validate(fileName);
            res.Should().Be(CheckOptions.Status.Ok, "the input file needs to be valid");
            var x = LoadBuildingSmartIDS(fileName);
            var exportedFile = Path.GetTempFileName();

            ILogger<BuildingSmartIDSLoadTests> logg = GetXunitLogger();
            var loggerMock = new Mock<ILogger<BuildingSmartCompatibilityTests>>(); // this is to check events
            _ = x.ExportBuildingSmartIDS(exportedFile, loggerMock.Object);
            _ = x.ExportBuildingSmartIDS(exportedFile, logg);
            var loggingCalls = loggerMock.Invocations.Select(x => x.ToString()).ToArray(); // this creates the array of logging calls
            loggingCalls.Where(x => x.Contains("Error") || x.Contains("Warning")).Should().BeEmpty("no calls to errors or warnings are expected");
            res = Validate(exportedFile);
            res.Should().Be(CheckOptions.Status.Ok , "the generated file needs to be valid");

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

        private static IdsLib.CheckOptions.Status Validate(string fileName)
        {
            var c = GetValidator(fileName);
            StringWriter debugOutputWriter = new();
            var validationResult = CheckOptions.Run(c, debugOutputWriter);
            if (validationResult != CheckOptions.Status.Ok)
            {
                Debug.WriteLine(debugOutputWriter.ToString());
            }
            
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
            public XmlElementSummary parent;
            public int attributes = 0;
            public List<XmlElementSummary> Subs = new();

            public XmlElementSummary(XElement main, XmlElementSummary parent)
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

            //public bool Equals([AllowNull] XmlElementSummary other)
            //{
            //    if (other == null)
            //        return false;
            //    if (ReferenceEquals(this, other))
            //        return true;
            //    if (this.type != other.type)
            //        return false;
            //    if (this.attributes != other.attributes)
            //        return false; 
            //}
        }

        //[Fact]
        //public void NotifiesErrorOnCompatibilityExport()
        //{
        //    var tpFacet = new IfcTypeFacet() { IfcType = "IfcWall" };
        //    var partFacet = new PartOfFacet();
        //    partFacet.SetEntity(PartOfFacet.Container.IfcGroup);
        //    partFacet.EntityType = "SomeName";
        //    Xids x = GetSpec(tpFacet, partFacet);
        //    RequiresErrors(x, 1);
        //    partFacet.EntityType = null;
        //    RequiresErrors(x, 0);
        //    x = GetSpec(partFacet, tpFacet);
        //    RequiresErrors(x, 1);
        //}

        private static Xids GetSpec(IFacet tpFacet, IFacet partFacet)
        {
            var x = new Xids();
            var t = x.PrepareSpecification(IfcSchemaVersion.IFC4);
            t.Applicability.Facets.Add(tpFacet);
            t.Requirement.Facets.Add(partFacet);
            return x;
        }

        static private void RequiresErrors(Xids x, int v)
        {
            var loggerMock = new Mock<ILogger<BuildingSmartCompatibilityTests>>();
            var file = Path.GetTempFileName();
            x.ExportBuildingSmartIDS(file, loggerMock.Object);
            var loggingCalls = loggerMock.Invocations.Select(x => x.ToString()).ToArray(); // this creates the array of logging calls
            var errorAndWarnings = loggingCalls.Where(x => x.Contains("Error") || x.Contains("Warning"));
            errorAndWarnings.Count().Should().Be(v, $"{nameof(PartOfFacet.EntityType)} is not exportable to bS IDS in this scenario");
            File.Delete(file);
        }
    }
}
