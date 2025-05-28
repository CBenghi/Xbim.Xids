using FluentAssertions;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.DescriptionTests
{
	public class MaterialFacetDescriptionTests : BaseDescriptionTests
	{
		[InlineData("Concrete", "a material Concrete")]
		[InlineData("Concrete,Aggregate", "a material Concrete or Aggregate")]
		[InlineData("Concrete.*", "a material matching 'Concrete.*'", ConstraintType.Pattern)]
		[Theory]
		public void MaterialFacetsRequirementsDescribed(string materialName, string expected, ConstraintType valueConstraint = ConstraintType.Exact)
		{
			MaterialFacet facet;

			facet = BuildFacetFromInputs(materialName, valueConstraint);


			facet.RequirementDescription.Should().Be(expected);
		}

		[InlineData("Concrete", "of material Concrete")]
		[InlineData("Concrete,Aggregate", "of material Concrete or Aggregate")]
		[InlineData("Concrete.*", "of material matching 'Concrete.*'", ConstraintType.Pattern)]
		[Theory]
		public void MaterialFacetsApplicabilityDescribed(string materialName, string expected, ConstraintType valueConstraint = ConstraintType.Exact)
		{
			MaterialFacet facet;

			facet = BuildFacetFromInputs(materialName, valueConstraint);


			facet.ApplicabilityDescription.Should().Be(expected);
		}

		[Fact]
		public void StructureAttributeFacetsWork()
		{
			MaterialFacet facet = new()
			{
				Value = new ValueConstraint(NetTypeName.String),
			};
			var constraint = new StructureConstraint()
			{
				MaxLength = 10,
				MinLength = 4
			};

			facet.Value.AddAccepted(constraint);


			facet.RequirementDescription.Should().Be("a material minimum 4 characters and maximum 10 characters");
		}

		private static MaterialFacet BuildFacetFromInputs(string materialList, ConstraintType identifierConstraint = ConstraintType.Exact)
		{
			MaterialFacet facet = new()
			{
				Value = new ValueConstraint(NetTypeName.String),
			};

			var materials = materialList.Split(',');
			foreach (var material in materials)
			{
				AddConstraint(facet.Value, identifierConstraint, material);
			}
			return facet;
		}
	}
}
