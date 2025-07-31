using FluentAssertions;
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

namespace Xbim.InformationSpecifications.Tests.DescriptionTests
{
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

			var facet = spec!.Applicability!.Facets[0];   // A long Applicability enumeration

			var description = facet.ApplicabilityDescription;
			description.Should().StartWith("of entity 'actuator' or 'airterminal' or ");
			description.Length.Should().BeLessThan(250);
			description.Should().EndWith("or 117 others and of predefined type <any>");
		}
	}
}
