using AwesomeAssertions;
using IdsLib.IdsSchema.XsNodes;
using IdsLib.IfcSchema;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xbim.InformationSpecifications.Helpers;
using Xbim.InformationSpecifications.Tests.Helpers;
using Xbim.InformationSpecifications.Tests.IoTests;
using Xunit;
using static IdsLib.IdsSchema.XsNodes.XsTypes;

namespace Xbim.InformationSpecifications.Tests;


public class HelpersTests
{
	public HelpersTests(ITestOutputHelper hlp)
	{
		this.helper = hlp;
	}
	ITestOutputHelper helper;

	internal ILogger<HelpersTests> GetXunitLogger()
	{
		var services = new ServiceCollection()
					.AddLogging((builder) => builder.AddXUnit(helper));
		IServiceProvider provider = services.BuildServiceProvider();
		var logg = provider.GetRequiredService<ILogger<HelpersTests>>();
		Assert.NotNull(logg);
		return logg;
	}

	public static IEnumerable<object[]> XsdFacetTestData => Enum.GetValues<IdsLib.IdsSchema.XsNodes.XsTypes.XsdAllowedFacets>().Select(v => new object[] { v }).ToArray();

	[Fact]
	public void RandomCreationPassesAudit()
	{
		var x = SampleXidsFactory.CreateXids()
			.WithRandomSpecifications(100, true)
			.Build();
		using var memoryStream = new System.IO.MemoryStream();
		x.ExportBuildingSmartIDS(memoryStream);
		memoryStream.Seek(0, SeekOrigin.Begin);
		var logger = GetXunitLogger();

		// ensure that the exported file is valid ids
		var opts = new IdsLib.SingleAuditOptions()
		{
			XmlWarningAction = IdsLib.AuditProcessOptions.XmlWarningBehaviour.ReportAsError,
			OmitIdsContentAudit = false
		};
		var auditResult = IdsLib.Audit.Run(memoryStream, opts, logger);
		auditResult.Should().Be(IdsLib.Audit.Status.Ok);
	}

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

	[Theory]
	[InlineData(IfcSchemaVersions.Ifc2x3, 0, false, false, true, false, 77)]
	[InlineData(IfcSchemaVersions.Ifc2x3, 0, false, true, false, false, 4)]
	[InlineData(IfcSchemaVersions.Ifc2x3, 0, true, false, false, false, 77)]
	[InlineData(IfcSchemaVersions.Ifc2x3, 5, false, false, false, false, 5)]

	[InlineData(IfcSchemaVersions.Ifc4, 0, false, false, true, false, 82)]
	[InlineData(IfcSchemaVersions.Ifc4, 0, false, true, false, false, 9)]
	[InlineData(IfcSchemaVersions.Ifc4, 0, true, false, false, false, 54)]
	[InlineData(IfcSchemaVersions.Ifc4, 5, false, false, false, false, 5)]

	[InlineData(IfcSchemaVersions.Ifc4x3, 0, false, false, true, false, 82)]
	[InlineData(IfcSchemaVersions.Ifc4x3, 0, false, true, false, false, 9)]
	[InlineData(IfcSchemaVersions.Ifc4x3, 0, true, false, false, false, 57)]
	[InlineData(IfcSchemaVersions.Ifc4x3, 5, false, false, false, false, 5)]
	public void SampleCountIsOk(IfcSchemaVersions sv,
		int makeSampleCount,
		bool makeSampleAttributes,
		bool makeDataTypes,
		bool makeMeasures,
		bool buildingSmartOnly,
		int expected)
	{
		// makeSampleAttributes ifc2x3 - 77
		// makeSampleAttributes ifc4 - 54
		// makeSampleAttributes ifc4x3 - 54

		var tm = new object[]
		{
				sv,
				makeSampleCount,
				makeSampleAttributes,
				makeDataTypes,
				makeMeasures,
				buildingSmartOnly,
				expected
		};
		var vals = tm.Select(x => x.ToString()).ToArray();

		helper.WriteLine($"Testing with parameters: {string.Join(", ", vals)}");

		var xds = SampleXidsFactory.CreateXids();
		if (makeSampleCount > 0)
		{
			helper.WriteLine($"Generating WithRandomSpecifications sample specifications.");
			xds = xds.WithRandomSpecifications(makeSampleCount, buildingSmartOnly);
		}
		if (makeSampleAttributes)
		{
			helper.WriteLine($"Generating WithDistinctAttributeTypeSpecifications sample specifications.");
			xds = xds.WithDistinctAttributeTypeSpecifications(sv);
		}
		if (makeDataTypes)
		{
			helper.WriteLine($"Generating WithPropertiesOfIfcTypes sample specifications.");
			xds = xds.WithPropertiesOfIfcTypes(sv);
		}
		if (makeMeasures)
		{
			helper.WriteLine($"Generating WithPropertiesOfMeasures sample specifications.");
			xds = xds.WithPropertiesOfMeasures(sv);
		}
		var t = xds.Build();
		var actual = t.AllSpecifications().Count();

		foreach (var x in t.AllSpecifications())
		{
			helper.WriteLine($"Specification: {x.Name}");
		}

		actual.Should().Be(expected);
	}


	[Fact]
	public void FacetGroupUse()
	{
		var x = XidsTestHelpers.GetSimpleXids();

		var usedForApplicability = x.GetSelectorsBy(FacetGroup.FacetUse.Applicability);
		usedForApplicability.Should().NotBeNull();
		usedForApplicability.Should().ContainSingle();

		var usedForRequirement = x.GetSelectorsBy(FacetGroup.FacetUse.Requirement);
		usedForRequirement.Should().NotBeNull();
		usedForRequirement.Should().ContainSingle();

		var all = x.GetSelectorsBy(FacetGroup.FacetUse.All);
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
		var all = tempXids.GetSelectorsBy(FacetGroup.FacetUse.Applicability);
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
