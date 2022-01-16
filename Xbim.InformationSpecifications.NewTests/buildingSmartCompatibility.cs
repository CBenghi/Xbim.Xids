using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Xbim.InformationSpecifications.NewTests
{
    public class buildingSmartCompatibility
    {

        [Fact]
        public void MinimalFileTest()
        {
            Xids x = new Xids();
            x.Initialize("IFC2X3");
            // at least one specification is needed
            //
            var t = x.PrepareSpecification();
            t.Requirement.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });
            t.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });

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

    }
}
