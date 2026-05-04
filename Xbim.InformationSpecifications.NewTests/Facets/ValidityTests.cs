#pragma warning disable xUnit1042
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Facets;

public class ValidityTests
{
	public ValidityTests(ITestOutputHelper helper)
	{
		outputHelper = helper;
	}
	ITestOutputHelper outputHelper;

	[Fact]
	public void LoadedFileFacetsAreValid()
	{
		var loaded = Xids.LoadFromJson("Files/ValidFacetEval.json");
		Assert.NotNull(loaded);
		foreach (var facetG in loaded.FacetGroups(FacetGroup.FacetUse.All))
		{
			outputHelper.WriteLine($"Testing facet group {facetG.Name}");
			foreach (var facet in facetG.Facets)
			{
				outputHelper.WriteLine($"Testing facet {facet.GetType()}");
				var verified = facet.IsValid();
				if (!verified)
				{ 
				}
				verified.Should().BeTrue($"{facet.GetType()} should be valid");
			}
		}
	}

	[Theory]
	[MemberData(nameof(GetValidFacets))]
	public void EmptyValidityTests(IFacet facet)
	{
		// some empty facets are not valid
		facet.IsValid().Should().BeFalse($"{facet.GetType()} should not be valid if empty");
	}

	public static IEnumerable<object[]> GetValidFacets()
	{
		yield return new object[] { new IfcPropertyFacet() };
		yield return new object[] { new AttributeFacet() };
	}


	[Theory]
	[MemberData(nameof(GetValidConstraints))]
	public void Can_tell_valid_constrains(ValueConstraint constraint)
	{
		constraint.IsValid().Should().BeTrue();
	}

	public static IEnumerable<object[]> GetValidConstraints()
	{
		var c1 = new ValueConstraint(NetTypeName.Double);
		c1.AddAccepted(new ExactConstraint("12.0"));
		yield return new object[] { c1 };

		c1 = new ValueConstraint(NetTypeName.String);
		c1.AddAccepted(new ExactConstraint("12.0"));
		yield return new object[] { c1 };

		c1 = new ValueConstraint(NetTypeName.Double);
		c1.AddAccepted(new RangeConstraint("12.0", true, "12.0", false));
		yield return new object[] { c1 };

		c1 = new ValueConstraint(NetTypeName.Double);
		c1.AddAccepted(new RangeConstraint("12.0", false, "12.0", true));
		yield return new object[] { c1 };

		c1 = new ValueConstraint(NetTypeName.Double);
		c1.AddAccepted(new RangeConstraint("12.0", true, "12.0", true));
		yield return new object[] { c1 };

		c1 = new ValueConstraint(NetTypeName.String);
		c1.AddAccepted(new PatternConstraint(""));
		yield return new object[] { c1 };

		c1 = new ValueConstraint(NetTypeName.String);
		c1.AddAccepted(new PatternConstraint("Some"));
		yield return new object[] { c1 };
	}

	[Theory]
	[MemberData(nameof(GetInvalidConstraints))]
	void Can_tell_Invalid_constrains(ValueConstraint constraint)
	{
		constraint.IsValid().Should().BeFalse();
	}
	public static IEnumerable<object[]> GetInvalidConstraints()
	{
		var c1 = new ValueConstraint(NetTypeName.Double);
		c1.AddAccepted(new ExactConstraint("someNoNumberValue"));
		yield return new object[] { c1 };

		c1 = new ValueConstraint(NetTypeName.String);
		c1.AddAccepted(new RangeConstraint("12.0", false, "12.0", false));
		yield return new object[] { c1 };

		c1 = new ValueConstraint(NetTypeName.String);
		c1.AddAccepted(new PatternConstraint("SomeInvalid["));
		yield return new object[] { c1 };

		c1 = new ValueConstraint(NetTypeName.String);
		c1.AddAccepted(new PatternConstraint("?"));
		yield return new object[] { c1 };
	}
}
