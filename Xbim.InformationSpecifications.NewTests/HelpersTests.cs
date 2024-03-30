using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xbim.InformationSpecifications.Helpers;
using Xbim.InformationSpecifications.Tests.Helpers;
using Xunit;

namespace Xbim.InformationSpecifications.Test.Helpers
{

    public class HelpersTests
    {
      
        [Fact]
        public void FacetGroupUse()
        {
            var x = XidsTestHelpers.GetSimpleXids();

            var usedForApplicability = x.FacetGroups(FacetGroup.FacetUse.Applicability);
            usedForApplicability.Should().NotBeNull();
            usedForApplicability.Should().ContainSingle();

            var usedForRequirement = x.FacetGroups(FacetGroup.FacetUse.Requirement);
            usedForRequirement.Should().NotBeNull();
            usedForRequirement.Should().ContainSingle();

            var all = x.FacetGroups(FacetGroup.FacetUse.All);
            all.Count().Should().Be(2);
        }

        [Fact]
        public void CanEnumerateFacetGroupsByUse()
        {
            var fSpec = @"bsFiles/IDS_wooden-windows.ids";
            // open the specs
            var tempXids = Xids.LoadBuildingSmartIDS(fSpec);
            tempXids.Should().NotBeNull("file should be able to load");
            Assert.NotNull(tempXids);

            var tmpFile = Path.GetTempFileName();
            tempXids.SaveAsJson(tmpFile);
            // can select all elements
            var all = tempXids.FacetGroups(FacetGroup.FacetUse.Applicability);
            all.Count().Should().BeGreaterThan(0);

            File.Delete(tmpFile);
        }


        [Fact]
        public void EnumCompatibilityTests()
        {
            PartOfFacet.Container.IfcAsset.IsCompatibleSchema(IfcSchemaVersion.Undefined).Should().BeFalse();
            PartOfFacet.Container.IfcAsset.IsCompatibleSchema(IfcSchemaVersion.IFC2X3).Should().BeTrue();
            PartOfFacet.Container.Undefined.IsCompatibleSchema(IfcSchemaVersion.IFC2X3).Should().BeFalse();

            var schemas = new[]
            {
                (IfcSchemaVersion.IFC2X3, 10),
                (IfcSchemaVersion.IFC4, 13),
                (IfcSchemaVersion.IFC4X3, 14)
            };

            foreach (var schema in schemas)
            {
                var schemaName = schema.Item1;
                var expected = schema.Item2;
                var cnt = 0;
                foreach (var val in Enum.GetValues<PartOfFacet.Container>())
                {
                    if (val.IsCompatibleSchema(schemaName))
                    {
                        Debug.WriteLine(val);
                        cnt++;
                    }
                }
                cnt.Should().Be(expected, $"there's an error on {schemaName}");
            }
        }
    }
}
