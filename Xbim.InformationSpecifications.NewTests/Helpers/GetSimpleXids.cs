using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.InformationSpecifications.NewTests.Helpers
{
    internal class XidsTestHelpers
    {
        internal static Xids GetSimpleXids()
        {
            var x = new Xids();
            var newspec = x.PrepareSpecification(IfcSchemaVersion.IFC2X3);
            newspec.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });

            newspec.Requirement.Facets.Add(
                new IfcPropertyFacet()
                {
                    PropertySetName = "Pset",
                    PropertyName = "Prop"
                }
                );
            return x;
        }
    }
}
