#pragma warning disable xUnit1042
using FluentAssertions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Facets
{
	public class DocumentFacetTests
	{
		[Theory]
		[MemberData(nameof(GetSingleAttributes))]
		public void AttributeEqualMatchImplementation(DocumentFacet t, DocumentFacet tSame)
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
		public void AttributeEqualNotMatchImplementation(DocumentFacet one, DocumentFacet other)
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

		public static IEnumerable<DocumentFacet> GetDifferentAttributes()
		{
			// this enumeration is all facets that are not consiered equal
			yield return new DocumentFacet();
			yield return new DocumentFacet() { DocId = "1", };
			yield return new DocumentFacet() { DocIntendedUse = "2", };
			yield return new DocumentFacet() { DocLocation = "3", };
			yield return new DocumentFacet() { DocName = "4", };
			yield return new DocumentFacet() { DocPurpose = "5", };
			yield return new DocumentFacet() { Instructions = "6", };
			yield return new DocumentFacet() { Uri = "8", };
			yield return new DocumentFacet()
			{
				Uri = "11",
				Instructions = "13",
				DocPurpose = "purp",
				DocName = "name",
				DocLocation = "docloc",
				DocIntendedUse = "intuse",
				DocId = "id",
			};

		}
	}
}
