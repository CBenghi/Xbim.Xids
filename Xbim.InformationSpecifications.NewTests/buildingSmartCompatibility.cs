using FluentAssertions;
using IdsLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xunit;
using static Xbim.InformationSpecifications.Xids;

namespace Xbim.InformationSpecifications.NewTests
{
    public class buildingSmartCompatibility
    {


        [Fact]
        public void MinimalFileExportTest()
        {
            Xids x = new Xids();
            // at least one specification is needed
            //
            var t = x.PrepareSpecification("IFC2X3");
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
            var t = x.PrepareSpecification("IFC2X3");
            t.Requirement.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWindow" });
            t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });

            t = x.PrepareSpecification("IFC4");
            t.Requirement.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });
            t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWindow" });

            // export
            var tmpFile = Path.GetTempFileName();
            var type = x.ExportBuildingSmartIDS(tmpFile);
            type.Should().Be(ExportedFormat.ZIP, "multiple groups are defined in the file");

            // todo: check that the file is a valid zip.
        }

        [Theory]
        [InlineData("bsFiles/bsFilesSelf/SimpleValueString.xml")]
        [InlineData("bsFiles/bsFilesSelf/SimpleValueRestriction.xml")]
        public void FullSchemaImportTest(string fileName)
        {
            Validate(fileName);
            var x = Xids.ImportBuildingSmartIDS(fileName);
            var exportedFile = Path.GetTempFileName();
            var _ = x.ExportBuildingSmartIDS(exportedFile);
            Validate(exportedFile);

            // more checks
            var outputCount = XmlReport(exportedFile);
            var inputCount = XmlReport(fileName);
            outputCount.Should().Be(inputCount, "everything should be exported");

        }

        private static void Validate(string fileName)
        {
            var c = GetValidator(fileName);
            StringWriter s = new StringWriter();
            var res = IdsLib.CheckOptions.Run(c, s);
            if (res != IdsLib.CheckOptions.Status.Ok)
            { 
                Debug.WriteLine(s.ToString());
            }
            res.Should().Be(IdsLib.CheckOptions.Status.Ok);
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
            public List<XmlElementSummary> Subs = new List<XmlElementSummary>();

            public XmlElementSummary(XElement main, XmlElementSummary parent)
            {
                type = main.Name.LocalName;
                this.parent = parent;
                attributes = main.Attributes().Count(); 
                Subs = main.Elements().Select(x=>new XmlElementSummary(x, this)).ToList();
            }
        }


    }
}
