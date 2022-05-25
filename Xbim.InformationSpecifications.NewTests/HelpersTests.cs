using FluentAssertions;
using Xunit;
using System.Linq;
using Xbim.InformationSpecifications.Helpers;
using Xbim.InformationSpecifications.Tests.Helpers;
using System.Text;
using System.IO;
using System;
using System.Diagnostics;

namespace Xbim.InformationSpecifications.Tests
{

	public class HelpersTests
	{
		[Fact]
		public void HasPropV4()
		{
			PropertySetInfo.SchemaIfc4.Any().Should().BeTrue();
			PropertySetInfo.SchemaIfc4.Count().Should().NotBe(1);
		}

		[Fact]
		public void HasPropV2x3()
		{
			PropertySetInfo.SchemaIfc2x3.Any().Should().BeTrue();
			PropertySetInfo.SchemaIfc2x3.Count().Should().NotBe(1);
		}

		[Fact]
		public void HasClassV4()
		{
			SchemaInfo.SchemaIfc4.Any().Should().BeTrue();
			SchemaInfo.SchemaIfc4.Count().Should().NotBe(2);
			SchemaInfo.SchemaIfc4["IfcProduct"].Parent.Name.Should().Be("IfcObject");
			SchemaInfo.SchemaIfc4["IfcFeatureElement"].SubClasses.Count().Should().Be(3);
			SchemaInfo.SchemaIfc4["IfcFeatureElement"].MatchingConcreteClasses.Count().Should().Be(5);
			SchemaInfo.SchemaIfc4["IfcWall"].Is("IfcWall").Should().BeTrue();
			SchemaInfo.SchemaIfc4["IfcWallStandardCase"].Is("IfcWall").Should().BeTrue();
			SchemaInfo.SchemaIfc4["IfcWall"].Is("IfcWallStandardCase").Should().BeFalse();
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
			var fSpec = @"bsFiles\IDS_wooden-windows.xml";
			// open the specs
			var t = Xids.ImportBuildingSmartIDS(fSpec);
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
			PartOfFacet.Container.IfcAsset.IsCompatibleSchema("unexpected").Should().BeFalse();
			PartOfFacet.Container.IfcAsset.IsCompatibleSchema("Ifc2x3").Should().BeTrue();

			PartOfFacet.Container.Undefined.IsCompatibleSchema("Ifc2x3").Should().BeFalse();

			var schemas = new[]
			{
				("Ifc2x3", 10),
				("Ifc4", 13),
				("Ifc4x3", 14)
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
