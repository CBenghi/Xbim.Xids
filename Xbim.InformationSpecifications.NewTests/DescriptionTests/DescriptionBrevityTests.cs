using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.DescriptionTests;

public class DescriptionBrevityTests
{
	[Fact]
	public void CanTruncateLongEnumerableListsInRequirements()
	{
		var fileName = "bsFiles/IDS_demo_BIM-basis-ILS.ids";

		var loaded = Xids.LoadBuildingSmartIDS(fileName);

		var spec = loaded!.AllSpecifications().Skip(1).First();

		var facet = spec!.Requirement!.Facets[1];   // A long requirement enumeration

		var description = facet.RequirementDescription;
		description.Should().StartWith("an attribute Name with value '-10 kelder' or ");
		description.Length.Should().BeLessThan(250);
		description.Should().EndWith("or 41 others");
	}

	[Fact]
	public void CanTruncateLongEnumerableListsInApplicability()
	{
		var fileName = "bsFiles/IDS_demo_BIM-basis-ILS.ids";
		var loaded = Xids.LoadBuildingSmartIDS(fileName);
		var spec = loaded!.AllSpecifications().Skip(2).First();
		var facet = spec!.Applicability!.Facets[0] as IfcTypeFacet;   // A long Applicability enumeration
		facet.Should().NotBeNull();
		var description = facet!.ApplicabilityDescription;
		description.Should().StartWith("of entity 'distributionelement' or 'buildingelement' or 'elementcomponent'");
		description.Length.Should().BeLessThan(300);
		description.Should().Contain("or 1 others");
		description.Should().EndWith("and of predefined type <any>");
	}
}
