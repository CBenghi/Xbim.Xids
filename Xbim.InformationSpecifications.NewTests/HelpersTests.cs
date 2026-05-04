using FluentAssertions;
using IdsLib.IdsSchema.XsNodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xbim.InformationSpecifications.Helpers;
using Xbim.InformationSpecifications.Tests.Helpers;
using Xunit;
using static IdsLib.IdsSchema.XsNodes.XsTypes;

namespace Xbim.InformationSpecifications.Tests;


public class HelpersTests
{
	public static IEnumerable<object[]> XsdFacetTestData => Enum.GetValues<IdsLib.IdsSchema.XsNodes.XsTypes.XsdAllowedFacets>().Select(v => new object[] { v }).ToArray();

	[Theory]
	[MemberData(nameof(XsdFacetTestData))]
	public void XsdFacetTestDataTests(XsdAllowedFacets value)
	{
		var some = ValueConstraint.ConstraintFromIds(value);
		some.Should().NotBeNull();
		var back = ValueConstraint.ConstraintToIds(some!.Value);
		back.Should().Be(value);
	}


	public static IEnumerable<object[]> ToIdsTypeConversionTestData => Enum.GetValues<NetTypeName>().Select(v => new object[] { v }).ToArray();

	[Theory]
	[MemberData(nameof(ToIdsTypeConversionTestData))]
	public void ToIdsTypeConversionTests(NetTypeName value)
	{
		var some = ValueConstraint.ConvertToXsType(value);
		some.Should().NotBe(BaseTypes.Invalid);
		var back = ValueConstraint.ConvertFromXsType(some);
		back.Should().Be(value);
	}

	public static IEnumerable<object[]> FromIdsTypeConversionTestData => XsTypes.GetValidBaseTypes().Select(v => new object[] { v }).ToArray();

	[Theory]
	[MemberData(nameof(FromIdsTypeConversionTestData))]
	public void FromIdsTypeConversionTests(BaseTypes value)
	{
		var some = ValueConstraint.ConvertFromXsType(value);
		var back = ValueConstraint.ConvertToXsType(some);
		back.Should().Be(value);
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
