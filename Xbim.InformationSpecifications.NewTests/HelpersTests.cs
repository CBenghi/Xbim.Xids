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
        public void HasPropV4()
        {
            PropertySetInfo.SchemaIfc4.Any().Should().BeTrue();
            PropertySetInfo.SchemaIfc4.Count.Should().NotBe(1);
        }

        [Fact]
        public void HasPropV2x3()
        {
            PropertySetInfo.SchemaIfc2x3.Any().Should().BeTrue();
            PropertySetInfo.SchemaIfc2x3.Count.Should().NotBe(1);
        }

        [Fact]
        public void HasClassV4()
        {
            SchemaInfo.SchemaIfc4.Any().Should().BeTrue();
            SchemaInfo.SchemaIfc4.Count().Should().NotBe(2);
            SchemaInfo.SchemaIfc4["IfcProduct"].Parent.Name.Should().Be("IfcObject");
            SchemaInfo.SchemaIfc4["IfcFeatureElement"].SubClasses.Count.Should().Be(3);
            SchemaInfo.SchemaIfc4["IfcFeatureElement"].MatchingConcreteClasses.Count().Should().Be(5);
            SchemaInfo.SchemaIfc4["IfcWall"].Is("IfcWall").Should().BeTrue();
            SchemaInfo.SchemaIfc4["IfcWallStandardCase"].Is("IfcWall").Should().BeTrue();
            SchemaInfo.SchemaIfc4["IfcWall"].Is("IfcWallStandardCase").Should().BeFalse();
            SchemaInfo.SchemaIfc4["IfcWall"].DirectAttributes.Should().NotBeNull();
            SchemaInfo.SchemaIfc4["IfcWall"].DirectAttributes.Count().Should().Be(9);
        }

        [Fact]
        public void HasClassV2x3()
        {
            SchemaInfo.SchemaIfc2x3.Any().Should().BeTrue();
            SchemaInfo.SchemaIfc2x3.Count().Should().NotBe(2);
        }

        [Fact]
        public void HasAttributesV2x3()
        {
            var attribs = SchemaInfo.SchemaIfc2x3.GetAttributeClasses("NotExisting");
            attribs.Should().BeEmpty();

            attribs = SchemaInfo.SchemaIfc2x3.GetAttributeClasses("ID");
            attribs.Length.Should().Be(2);


            var attribNames = SchemaInfo.SchemaIfc2x3.GetAttributeNames();
            attribNames.Count().Should().Be(179);
        }

        [Fact]
        public void HasAttributesV4()
        {
            var attribs = SchemaInfo.SchemaIfc4.GetAttributeClasses("NotExisting");
            attribs.Should().BeEmpty();

            attribs = SchemaInfo.SchemaIfc4.GetAttributeClasses("UserDefinedOperationType");
            attribs.Length.Should().Be(3);

            var attribNames = SchemaInfo.SchemaIfc4.GetAttributeNames();
            attribNames.Count().Should().Be(128);
        }


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
            var fSpec = @"bsFiles\IDS_wooden-windows.ids";
            // open the specs
            var t = Xids.LoadBuildingSmartIDS(fSpec);
            t.Should().NotBeNull("file should be able to load");

            var tmpFile = Path.GetTempFileName();
            t.SaveAsJson(tmpFile);

            // can select all elements
            var all = t.FacetGroups(FacetGroup.FacetUse.Applicability);
            all.Count().Should().BeGreaterThan(0);
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

        [Fact]
        public void PropMeasureTest()
        {
            var prop = PropertySetInfo.Get(IfcSchemaVersion.IFC2X3, "Pset_DuctFittingTypeCommon", "SubType");
            prop.Should().NotBeNull();

            var t = prop.IsMeasureProperty(out var measure);
            t.Should().BeTrue();
            measure.Should().Be(IfcMeasures.String);

            foreach (var item in FindMeasureTypes())
            {
                item.IsMeasureProperty(out _);
            }

        }

        static private IEnumerable<IPropertyTypeInfo> FindMeasureTypes()
        {
            var done = new HashSet<string>();
            foreach (var item in new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4 })
            {
                var schema = PropertySetInfo.GetSchema(item);
                schema.Should().NotBeNull();
                foreach (var propSet in schema)
                {
                    foreach (var prop in propSet.Properties.OfType<SingleValuePropertyType>().Where(x => x.DataType != null))
                    {
                        if (done.Contains(prop.DataType))
                            continue;
                        done.Add(prop.DataType);
                        yield return prop;
                    }
                }
            }
        }

        [Fact]
        public void MeasureInfoNeverNull()
        {
            foreach (var m in Enum.GetValues<IfcMeasures>())
            {
                SchemaInfo.GetMeasure(m).Should().NotBeNull($"{m} is needed.");
            }
        }


    }
}
