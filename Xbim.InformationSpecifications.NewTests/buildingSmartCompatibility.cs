using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using static Xbim.InformationSpecifications.Xids;

namespace Xbim.InformationSpecifications.NewTests
{
    public class buildingSmartCompatibility
    {

        [Fact]
        public void MinimalFileTest()
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
            IdsLib.CheckOptions c = new IdsLib.CheckOptions();
            c.CheckSchema = new[] { "bsFiles\\ids_05.xsd" };
            c.InputSource = tmpFile;

            StringWriter s = new StringWriter();
            var res = IdsLib.CheckOptions.Run(c, s);
            if (res != IdsLib.CheckOptions.Status.Ok)
            {
                Debug.WriteLine(s.ToString());
            }
            Assert.Equal(IdsLib.CheckOptions.Status.Ok, res);
        }

        [Fact]
        public void DoubleFileTest()
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

            // the file is actually a zip.
        }

    }
}
