#pragma warning disable xUnit1042
using FluentAssertions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Facets
{
	public class PropertyFacetTests
	{
		[Theory]
		[InlineData("0", "", true)]
		[InlineData("", "12", true)]
		[InlineData("0", "12", true)]
		[InlineData("12", "0", false)]
		[InlineData("", "", false)]
		public void RangeValidityPass(string min, string max, bool outcome)
		{
			var facet = new IfcPropertyFacet()
			{
				PropertyName = "Name",
				PropertySetName = "Name",
				DataType = "IFCLENGHTMEASURE",
				PropertyValue = new ValueConstraint()
				{
					BaseType = NetTypeName.Double,
					AcceptedValues = [new RangeConstraint(min, true, max, false)]
				}
			};
			facet.IsValid().Should().Be(outcome);
		}


		[Theory]
		[MemberData(nameof(GetSingleAttributes))]
		public void AttributeEqualMatchImplementation(IfcPropertyFacet t, IfcPropertyFacet tSame)
		{
			FacetImplementationTests.TestAddRemove(t);
			var aresame = t.Equals(tSame);
			if (!aresame)
			{
				Debug.WriteLine(t);
			}
			aresame.Should().BeTrue();
			t.Equals(null).Should().BeFalse();
		}


		[Theory]
		[InlineData("Name", true)]
		[InlineData("NAME", true)]
		[InlineData("name", true)]
		public void StringsCanBeComparedCaseInsensitively(string input, bool expectedResult)
		{

			var facet = new IfcPropertyFacet()
			{
				PropertyName = "Name",
			};
			facet.PropertyName.BaseType.Should().Be(NetTypeName.Undefined); // Why isn't this String type?

			facet.PropertyName.IsSatisfiedBy(input, ignoreCase: true).Should().Be(expectedResult, "we're ignoring case");
		}

		[Fact]
		public void EqualityWorksWhenBaseTypeSet()
		{
			// Equality testing when Basetype string with ExactConstraints was wrong
			var facet = new IfcPropertyFacet()
			{
				PropertyName = "Name",
			};
			facet.PropertyName.BaseType = NetTypeName.String;
			var copy = facet;

			facet.Equals(copy).Should().BeTrue("It's the same object!");
		}

		[Theory]
		[MemberData(nameof(GetDifferentAttributesPairs))]
		public void AttributeEqualNotMatchImplementation(IfcPropertyFacet one, IfcPropertyFacet other)
		{
			var result = one.Equals(other);
			if (result == true)
			{
				Debug.WriteLine($"{one} vs {other}");
			}
			result.Should().BeFalse();
		}

		public static IEnumerable<object[]> GetDifferentAttributesPairs()
		{
			var source = GetDifferentAttributes().ToArray();
			for (int i = 0; i < source.Length; i++)
			{
				for (int t = i + 1; t < source.Length; t++)
				{
					yield return new object[] { source[i], source[t] };
				}
			}
		}

		public static IEnumerable<object[]> GetSingleAttributes()
		{
			var set1 = GetDifferentAttributes().ToList();
			var set2 = GetDifferentAttributes().ToList();
			for (int i = 0; i < set1.Count; i++)
			{
				yield return new object[]
				{
					set1[i],
					set2[i],
				};
			}
		}

		public static IEnumerable<IfcPropertyFacet> GetDifferentAttributes()
		{
			yield return new IfcPropertyFacet() { };
			yield return new IfcPropertyFacet() { Instructions = "1" };
			yield return new IfcPropertyFacet() { DataType = "3" };
			yield return new IfcPropertyFacet() { PropertyName = "4" };
			yield return new IfcPropertyFacet() { PropertySetName = "5" };
			yield return new IfcPropertyFacet() { PropertyValue = "6" };
			yield return new IfcPropertyFacet() { Uri = "7" };
			yield return new IfcPropertyFacet()
			{
				Instructions = "Some",
				DataType = "meas",
				PropertyName = "name",
				PropertySetName = "psetname",
				PropertyValue = "pvalue",
				Uri = "uri",
			};
		}
	}
}
