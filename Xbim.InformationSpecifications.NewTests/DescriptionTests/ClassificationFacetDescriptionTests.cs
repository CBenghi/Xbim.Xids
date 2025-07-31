using FluentAssertions;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.DescriptionTests
{
	public class ClassificationFacetDescriptionTests : BaseDescriptionTests
	{

		[InlineData("EF_25", "Uniclass", "a classification EF_25 from system Uniclass")]
		[InlineData("EF_25.*", "Uniclass", "a classification matching 'EF_25.*' from system Uniclass", ConstraintType.Pattern)]
		[InlineData("EF_25", "Uniclass.*", "a classification EF_25 from system matching 'Uniclass.*'", ConstraintType.Exact, ConstraintType.Pattern)]
		[InlineData("EF_25.*", "Uniclass.*", "a classification matching 'EF_25.*' from system matching 'Uniclass.*'", ConstraintType.Pattern, ConstraintType.Pattern)]

		[InlineData("SS_20", null, "a classification SS_20 from system <any>")]
		[InlineData("EF_25,EF_26", "Uniclass 2015", "a classification 'EF_25' or 'EF_26' from system Uniclass 2015")]
		[InlineData("EF_25,EF_26", "Uniclass 2015,UniClass", "a classification 'EF_25' or 'EF_26' from system 'Uniclass 2015' or 'UniClass'")]
		[Theory]
		public void ClassificationFacetsRequirementsDescribed(string identifiers, string? systems, string expected,
			ConstraintType identifierConstraint = ConstraintType.Exact, ConstraintType systemConstraint = ConstraintType.Exact)
		{
			IfcClassificationFacet facet;


			facet = BuildFacetFromInputs(identifiers, systems ?? "", identifierConstraint, systemConstraint);


			facet.RequirementDescription.Should().Be(expected);
		}

		[InlineData("EF_25", "Uniclass", "with classification EF_25 from system Uniclass")]
		[InlineData("EF_25.*", "Uniclass", "with classification matching 'EF_25.*' from system Uniclass", ConstraintType.Pattern)]
		[InlineData("EF_25", "Uniclass.*", "with classification EF_25 from system matching 'Uniclass.*'", ConstraintType.Exact, ConstraintType.Pattern)]
		[InlineData("EF_25.*", "Uniclass.*", "with classification matching 'EF_25.*' from system matching 'Uniclass.*'", ConstraintType.Pattern, ConstraintType.Pattern)]

		[InlineData("SS_20", null, "with classification SS_20 from system <any>")]
		[InlineData("EF_25,EF_26", "Uniclass 2015", "with classification 'EF_25' or 'EF_26' from system Uniclass 2015")]
		[InlineData("EF_25,EF_26", "Uniclass 2015,UniClass", "with classification 'EF_25' or 'EF_26' from system 'Uniclass 2015' or 'UniClass'")]
		[Theory]
		public void ClassificationFacetsApplicabilityDescribed(string identifiers, string? systems, string expected,
			ConstraintType identifierConstraint = ConstraintType.Exact, ConstraintType systemConstraint = ConstraintType.Exact)
		{
			IfcClassificationFacet facet;


			facet = BuildFacetFromInputs(identifiers, systems ?? "", identifierConstraint, systemConstraint);


			facet.ApplicabilityDescription.Should().Be(expected);
		}


		[Fact]
		public void ClassificationFacetsApplicabilityNameSupportsMultiplePatterns()
		{
			IfcClassificationFacet facet = new()
			{
				ClassificationSystem = new ValueConstraint(NetTypeName.String),
				Identification = new ValueConstraint(NetTypeName.String),
			};

			AddConstraint(facet.ClassificationSystem, ConstraintType.Pattern, "Uniclass.*");

			AddConstraint(facet.Identification, ConstraintType.Pattern, "EF_.*");
			AddConstraint(facet.Identification, ConstraintType.Pattern, "Pr_.*");

			// Undesireable but fixing needs thought. Documenting the behaviour for now. E.g. the quoting of 'matching <foo>'
			var expected = "with classification matching 'EF_.*' or matching 'Pr_.*' from system matching 'Uniclass.*'";
			facet.ApplicabilityDescription.Should().Be(expected);
		}

		private static IfcClassificationFacet BuildFacetFromInputs(string classification, string systems = "",
			ConstraintType identifierConstraint = ConstraintType.Exact, ConstraintType systemConstraint = ConstraintType.Exact)
		{
			IfcClassificationFacet facet = new()
			{
				ClassificationSystem = new ValueConstraint(NetTypeName.String),
				Identification = new ValueConstraint(NetTypeName.String),
			};

			var ids = classification.Split(',');
			foreach (var id in ids)
			{
				AddConstraint(facet.Identification, identifierConstraint, id);
			}
			if (systems == null)   // No systems
			{
				facet.ClassificationSystem = null;
				return facet;
			}



			var values = systems.Split(','); // String or strings
			foreach (var val in values)
			{
				AddConstraint(facet.ClassificationSystem, systemConstraint, val);
			}
			return facet;
		}

	}
}
