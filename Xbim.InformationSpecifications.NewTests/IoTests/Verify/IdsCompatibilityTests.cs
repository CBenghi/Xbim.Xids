using AwesomeAssertions;
using IdsLib.IfcSchema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xbim.Ifc4x3.ProductExtension;
using Xbim.InformationSpecifications.Tests.Helpers;
using XidsEditing.InformationSpecifications;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.IoTests.Verify;

public class IdsCompatibilityTests
{

	public IdsCompatibilityTests(ITestOutputHelper outputHelper)
	{
		OutputHelper = outputHelper;
		log = LoggingTestHelper.GetXunitLogger<IdsCompatibilityTests>(OutputHelper);
	}
	private ITestOutputHelper OutputHelper { get; }
	private readonly ILogger<IdsCompatibilityTests> log;

	[Theory(DisplayName = "Test IDS roundtrip")]
	[MemberData(nameof(GetSimpleXids))]
	public async Task TestIdsPersistenceAsync(string originalFileName)
	{
		DirectoryInfo d = new DirectoryInfo(".");
		log.LogInformation($"Testing IDS roundtrip for file `{originalFileName}` in directory {Environment.NewLine}`{d.FullName}`");

		var x = Xids.LoadFromJson(originalFileName)!;

		var idsfile = originalFileName.Replace(".1.json", ".ids");
		x.ExportBuildingSmartIDS(idsfile);

		// ensure that the exported file is valid ids
		var opts = new IdsLib.SingleAuditOptions()
		{
			XmlWarningAction = IdsLib.AuditProcessOptions.XmlWarningBehaviour.ReportAsError,
			OmitIdsContentAudit = true
		};

		FileInfo f = new FileInfo(idsfile);
		log.LogInformation($"Auditing IDS file `{f.FullName}`");
		using (var reader = File.OpenRead(idsfile))
		{
			var auditResult = IdsLib.Audit.Run(reader, opts, log);
			auditResult.Should().Be(IdsLib.Audit.Status.Ok);
		}

		var reloadedFromIds = Xids.LoadBuildingSmartIDS(idsfile)!;
		var reloadedName = originalFileName.Replace(".1.json", ".2.json");
		reloadedFromIds.SaveAsJson(reloadedName);

		// facet group names are lost in the roundtrip to IDS
		string[] ignoreNames = ["ApplicabilityDescription", "RequirementDescription", "Name", "Provider", "Consumers"];

		var reloadedJson = File.ReadAllText(reloadedName);
		var reloadedFromIdsVerified = await Verifier.VerifyJson(reloadedJson)
			.UseFileName($"{originalFileName}_reloaded")
			.IgnoreMembers(ignoreNames)
			.ScrubNumericIds()
			.IgnoreTrailingZeros()
			.AutoVerify();

		var originalJson = File.ReadAllText(originalFileName);
		var originalVerified = await Verifier.VerifyJson(originalJson)
			.UseFileName($"{originalFileName}_original")
			.IgnoreMembers(ignoreNames)
			.ScrubNumericIds()
			.IgnoreTrailingZeros()
			.AutoVerify();
		reloadedFromIdsVerified.Text.Should().BeEquivalentTo(originalVerified.Text, "The reloaded JSON should be equivalent to the original JSON");

		foreach (var item in reloadedFromIdsVerified.Files)
		{
			File.Delete(item);
		}
		foreach (var item in originalVerified.Files)
		{
			File.Delete(item);
		}
	}

	[Fact]
	public void CanRandomiseXids()
	{
		var t = GetSimpleXids();
		t.Should().NotBeEmpty("There should be some test XIDs to randomise");
	}

	public static IEnumerable<object[]> GetSimpleXids()
	{
		IFacet? prev = null;
		int idsIndex = 0;
		foreach (var item in IdsCompatibleFacets())
		{
			if (prev != null)
			{
				var x = new Xids();
				var newspec = x.PrepareSpecification(IfcSchemaVersion.IFC4X3);
				newspec.Applicability.Facets.Add(prev);
				newspec.Requirement!.Facets.Add(item);
				string displayName = $"{idsIndex++:D4}_{GetName(prev)}_{GetName(item)}.1.json";
				x.SaveAsJson(displayName);
				yield return [displayName];
			}
			prev = item;
		}
		// also add some with measures and some with odd types
		var schemas = new IfcSchemaVersions[] { IfcSchemaVersions.Ifc4x3, IfcSchemaVersions.Ifc2x3, IfcSchemaVersions.Ifc4 };
		foreach (var version in schemas)
		{
			var opts = new bool[] { true, false };
			foreach (var measuresOrDataTypes in opts)
			{
				var nm = measuresOrDataTypes ? "Measures" : "DataTypes";
				var x = SampleXidsFactory.CreateDataTypes(version, measuresOrDataTypes, !measuresOrDataTypes);
				string displayName = $"{idsIndex++:D4}_{nm}.1.json";
				x.SaveAsJson(displayName);
				yield return [displayName];
			}
		}
	}

	private static string GetName(PartOfFacet prev)
	{
		if (prev.GetRelation() == PartOfFacet.PartOfRelation.Undefined)
		{
			return nameof(PartOfFacet);
		}
		return $"{nameof(PartOfFacet)}_{prev.EntityRelation}";
	}
	private static string GetName(IfcTypeFacet prev)
	{
		if (prev.IfcType == null)
			return nameof(IfcTypeFacet);
		var count = 0;
		if (prev.IfcType.AcceptedValues is not null)
			count = prev.IfcType.AcceptedValues.Count;
		return $"{nameof(IfcTypeFacet)}_{count}";
	}

	private static string GetName(IFacet prev)
	{
		if (prev == null)
			return "Null";
		return prev switch
		{
			PartOfFacet p => GetName(p),
			IfcTypeFacet tf => GetName(tf),
			_ => prev.GetType().Name
		};
	}

	internal static IEnumerable<IfcTypeFacet> IdsCompatibleTypeFacets()
	{
		yield return new IfcTypeFacet() { IfcType = "IFCDEEPFOUNDATION", IncludeSubtypes = true };
		yield return new IfcTypeFacet() { IfcType = new ValueConstraint(["IFCWALL", "IFCROOF"]), IncludeSubtypes = true };
		yield return new IfcTypeFacet() { IfcType = new ValueConstraint(["IFCWALL", "IFCROOF"]), IncludeSubtypes = false };
		yield return new IfcTypeFacet() { IfcType = new ValueConstraint("IFCWINDOW"), PredefinedType = "WINDOW", IncludeSubtypes = true }; // has no sublcasses
		yield return new IfcTypeFacet() { IfcType = PartOfFacet.Container.IfcGroup.ToString() };
		yield return new IfcTypeFacet() { IfcType = PartOfFacet.Container.IfcElementAssembly.ToString() };
	}

	internal static IEnumerable<PartOfFacet> IdsCompatiblePartOfFacets()
	{
		var rels = Enum.GetValues<PartOfFacet.PartOfRelation>();
		foreach (var item in rels)
		{
			foreach (var tp in IdsCompatibleTypeFacets())
			{
				yield return new PartOfFacet()
				{
					EntityType = tp,
					EntityRelation = item.ToString()
				};
			}
		}
	}

	internal static IEnumerable<IFacet> IdsCompatibleFacets()
	{
		foreach (var item in IdsCompatiblePartOfFacets())
			yield return item;
		foreach (var item in IdsCompatibleTypeFacets())
			yield return item;
	}
}
