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
    public class MaterialFacetTests
    {
		[Theory]
		[MemberData(nameof(GetSingleAttributes))]
		public void AttributeEqualMatchImplementation(MaterialFacet t, MaterialFacet tSame)
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
		public void AttributeEqualNotMatchImplementation(MaterialFacet one, MaterialFacet other)
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

		public static IEnumerable<MaterialFacet> GetDifferentAttributes()
		{
			yield return new MaterialFacet();
			yield return new MaterialFacet() { Instructions = "instr", };
			yield return new MaterialFacet() { Location = "type", };
			yield return new MaterialFacet() { Uri = "uri", };
			yield return new MaterialFacet() { Use = "use", };
			yield return new MaterialFacet() { Value = "value", };
			yield return new MaterialFacet()
			{
				Instructions = "instr",
				Location = "loc",
				Uri = "uri",
				Use = "use",
				Value = "value",
			};
		}
	}
}
