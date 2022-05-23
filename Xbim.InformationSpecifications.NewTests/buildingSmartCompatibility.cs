using FluentAssertions;
using IdsLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;
using static Xbim.InformationSpecifications.Xids;

namespace Xbim.InformationSpecifications.Tests
{
    public class buildingSmartCompatibilityTests
    {
        private ITestOutputHelper OutputHelper;

        public buildingSmartCompatibilityTests(ITestOutputHelper outputHelper)
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

            StringWriter s = new StringWriter();
            var res = IdsLib.CheckOptions.Run(c, s);
            if (res != IdsLib.CheckOptions.Status.Ok)
            {
                Debug.WriteLine(s.ToString());
            }
            Assert.Equal(IdsLib.CheckOptions.Status.Ok, res);
        }

        private static CheckOptions GetValidator(string tmpFile)
        {
            CheckOptions c = new CheckOptions();
            c.CheckSchema = new[] { "bsFiles\\ids_05.xsd" };
            c.InputSource = tmpFile;
            return c;
        }

        [Fact]
        public void DoubleFileExportTest()
        {
            Xids x = new Xids();
            // at least one specification is needed
            //
            var t = x.PrepareSpecification(IfcSchemaVersion.IFC2X3);
            t.Requirement.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWindow" });
            t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });

            var newGroup = new SpecificationsGroup();
            x.SpecificationsGroups.Add(newGroup);

            t = x.PrepareSpecification(newGroup, IfcSchemaVersion.IFC4);
            t.Requirement.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });
            t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWindow" });

            // export
            var tmpFile = Path.GetTempFileName();
            var type = x.ExportBuildingSmartIDS(tmpFile);
            type.Should().Be(ExportedFormat.ZIP, "multiple groups are defined in the file");

            // todo: check that the file is a valid zip.
        }

        internal ILogger<buildingSmartIDSLoadTests> GetXunitLogger()
        {
            var services = new ServiceCollection()
                        .AddLogging((builder) => builder.AddXUnit(OutputHelper));
            IServiceProvider provider = services.BuildServiceProvider();
            var logg = provider.GetRequiredService<ILogger<buildingSmartIDSLoadTests>>();
            Assert.NotNull(logg);
            return logg;
        }

        [Theory]
        [InlineData("bsFiles/bsFilesSelf/SimpleValueString.xml")]
        [InlineData("bsFiles/bsFilesSelf/SimpleValueRestriction.xml")]
        public void FullSchemaImportTest(string fileName)
        {
            Validate(fileName);
            var x = Xids.ImportBuildingSmartIDS(fileName);
            var exportedFile = Path.GetTempFileName();

            ILogger<buildingSmartIDSLoadTests> logg = GetXunitLogger();
            var loggerMock = new Mock<ILogger<buildingSmartCompatibilityTests>>(); // this is to check events
            _ = x.ExportBuildingSmartIDS(exportedFile, loggerMock.Object);
            _ = x.ExportBuildingSmartIDS(exportedFile, logg);
            var loggingCalls = loggerMock.Invocations.Select(x => x.ToString()).ToArray(); // this creates the array of logging calls
            loggingCalls.Where(x => x.Contains("Error") || x.Contains("Warning")).Should().BeEmpty("no calls to errors or warnings are expected");
            Validate(exportedFile);

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

        private static void Validate(string fileName)
        {
            var c = GetValidator(fileName);
            StringWriter debugOutputWriter = new StringWriter();
            var validationResult = IdsLib.CheckOptions.Run(c, debugOutputWriter);
            if (validationResult != IdsLib.CheckOptions.Status.Ok)
            { 
                Debug.WriteLine(debugOutputWriter.ToString());
            }
            validationResult.Should().Be(IdsLib.CheckOptions.Status.Ok);
        }

        private XmlElementSummary XmlReport(string tmpFile)
        {
            var main = XElement.Parse(File.ReadAllText(tmpFile));
            XmlElementSummary summary = new XmlElementSummary(main, null);
            return summary;
        }

        private (int elements, int attributes) Count(string tmpFile)
        {
            var main = XElement.Parse(File.ReadAllText(tmpFile));
            return Count(main);
        }

        private static (int elements, int attributes) Count(XElement main)
        {
            var t = (elements: main.Elements().Count(), attributes: main.Attributes().Count());
            foreach (var sub in main.Elements())
            {
                var subCount = Count(sub);
                t.elements += subCount.elements;
                t.attributes += subCount.attributes;
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
                Subs = main.Elements().Select(x=>new XmlElementSummary(x, this)).ToList();
            }

            public string FirstDifference(XmlElementSummary other)
            {
                if (other == null)
                    return ReportDifference("Other is null");
                if (this.attributes != other.attributes)
                    return ReportDifference("Different attributes count");
                if (this.Subs.Count() != other.Subs.Count())
                    return ReportDifference("Different elements count");
                for (int i = 0; i< Subs.Count(); i++ )
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
                StringBuilder sb = new StringBuilder(); 
                
                Stack<XmlElementSummary> parents = new Stack<XmlElementSummary>();
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


    }
}
