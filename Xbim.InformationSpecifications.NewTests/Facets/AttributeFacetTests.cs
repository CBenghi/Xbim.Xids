using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.InformationSpecifications.Tests;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Facets
{
    public class AttributeFacetTests
    {
		[Theory]
		[MemberData(nameof(GetSingleAttributes))]
		public void AttributeEqualMatchImplementation(AttributeFacet t, AttributeFacet tSame)
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
		[MemberData(nameof(GetDifferentAttributesPairs))]
		public void AttributeEqualNotMatchImplementation(AttributeFacet one, AttributeFacet other)
		{
			one.Equals(other).Should().BeFalse();
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

		public static IEnumerable<AttributeFacet> GetDifferentAttributes()
		{
			// this enumeration is all facets that are not consiered equal
			yield return new AttributeFacet();
			yield return new AttributeFacet() { AttributeName = "One" };
			yield return new AttributeFacet() { AttributeName = "Two" };
			yield return new AttributeFacet() { AttributeValue = "One" };
			yield return new AttributeFacet() { AttributeValue = "Two" };
			yield return new AttributeFacet() { Instructions = "Two" };
			yield return new AttributeFacet() { Use = "optional" };
			yield return new AttributeFacet() { Use = "required" };
			yield return new AttributeFacet()
			{
				AttributeName = "One",
				AttributeValue = "Two",
				Instructions = "Some instructions",
				Uri = "http://www.google.com",
				Use = "required"
			};
		}
	}
}
