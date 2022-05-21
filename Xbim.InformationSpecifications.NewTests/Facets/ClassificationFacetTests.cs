using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.InformationSpecifications.Tests;
using Xunit;

namespace Xbim.InformationSpecifications.NewTests.Facets
{
    public class ClassificationFacetTests
    {
		[Theory]
		[MemberData(nameof(GetSingleAttributes))]
		public void AttributeEqualMatchImplementation(IfcClassificationFacet t, IfcClassificationFacet tSame)
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
		public void AttributeEqualNotMatchImplementation(IfcClassificationFacet one, IfcClassificationFacet other)
		{
			var result =one.Equals(other);
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

		public static IEnumerable<IfcClassificationFacet> GetDifferentAttributes()
		{
			// this enumeration is all facets that are not consiered equal
			yield return new IfcClassificationFacet() { };
			yield return new IfcClassificationFacet() { ClassificationSystem = "2", };
			yield return new IfcClassificationFacet() { Identification = "2", };
			yield return new IfcClassificationFacet() { IncludeSubClasses = true, };
			yield return new IfcClassificationFacet() { Instructions = "some", };
			yield return new IfcClassificationFacet() { Uri = "some", };
			yield return new IfcClassificationFacet() { Use = "some", };
			yield return new IfcClassificationFacet()
			{
				ClassificationSystem = "A",
				Identification = "href",
				IncludeSubClasses = true,
				Instructions = "some",
				Uri = "some",
				Use = "use",
			};

		}
	}
}
