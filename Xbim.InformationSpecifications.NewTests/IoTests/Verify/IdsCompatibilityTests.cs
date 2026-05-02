using FluentAssertions;
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
		var x = Xids.LoadFromJson(originalFileName)!;

		var idsfile = originalFileName.Replace(".1.json", ".ids");
		x.ExportBuildingSmartIDS(idsfile);

		// ensure that the exported file is valid ids
		var opts = new IdsLib.SingleAuditOptions() {
			XmlWarningAction = IdsLib.AuditProcessOptions.XmlWarningBehaviour.ReportAsError,
			OmitIdsContentAudit = true
		};

		using (var reader = File.OpenRead(idsfile))
		{
			var auditResult = IdsLib.Audit.Run(reader, opts, log);
			auditResult.Should().Be(IdsLib.Audit.Status.Ok);
		}

		var reloaded = Xids.LoadBuildingSmartIDS(idsfile)!;
		var reloadedName = originalFileName.Replace(".1.json", ".2.json");
		reloaded.SaveAsJson(reloadedName);

		var reloadedJson = File.ReadAllText(reloadedName);
		var reloadedVerified = await Verifier.VerifyJson(reloadedJson)
			.UseFileName($"{originalFileName}_reloaded")
			.IgnoreMembers("ApplicabilityDescription", "RequirementDescription")
			.ScrubNumericIds()
			.AutoVerify();

		var originalJson = File.ReadAllText(originalFileName);
		var originalVerified = await Verifier.VerifyJson(originalJson)
			.UseFileName($"{originalFileName}_original")
			.IgnoreMembers("ApplicabilityDescription", "RequirementDescription")
			.ScrubNumericIds()
			.AutoVerify();
		reloadedVerified.Text.Should().BeEquivalentTo(originalVerified.Text, "The reloaded JSON should be equivalent to the original JSON");

		foreach (var item in reloadedVerified.Files)
		{
			File.Delete(item);
		}
		foreach (var item in originalVerified.Files)
		{
			File.Delete(item);
		}
	}

	public static IEnumerable<object[]> GetSimpleXids()
	{
		IFacet? prev = null;
		int index = 0;
		foreach (var item in IdsCompatibleFacets())
		{
			if (prev != null)
			{
				var x = new Xids();
				var newspec = x.PrepareSpecification(IfcSchemaVersion.IFC2X3);
				newspec.Applicability.Facets.Add(prev);
				newspec.Requirement!.Facets.Add(item);
				string displayName = $"{index++:D4}_{GetName(prev)}_{GetName(item)}.1.json";
				x.SaveAsJson(displayName);
				yield return [displayName];
			}
			prev = item;
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
		yield return new IfcTypeFacet() { IfcType = "IFCWALL", IncludeSubtypes = true, PredefinedType = "PARAPET" };
		// yield return new IfcTypeFacet() { IfcType = new ValueConstraint(["IFCWALL", "IFCWINDOW"]), IncludeSubtypes = true };
		yield return new IfcTypeFacet() { IfcType = new ValueConstraint(["IFCWALL", "IFCWINDOW"]), IncludeSubtypes = false };
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
