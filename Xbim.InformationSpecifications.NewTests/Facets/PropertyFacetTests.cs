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
    public class PropertyFacetTests
    {
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
			yield return new IfcPropertyFacet() { Location = "2" };
			yield return new IfcPropertyFacet() { Measure = "3" };
			yield return new IfcPropertyFacet() { PropertyName = "4" };
			yield return new IfcPropertyFacet() { PropertySetName = "5" };
			yield return new IfcPropertyFacet() { PropertyValue = "6" };
			yield return new IfcPropertyFacet() { Uri = "7" };
			yield return new IfcPropertyFacet() { Use = "8" };
			yield return new IfcPropertyFacet()
			{
				Instructions = "Some",
				Location = "SomeLoc",
				Measure = "meas",
				PropertyName = "name",
				PropertySetName = "psetname",
				PropertyValue = "pvalue",
				Uri = "uri",
				Use = "use",
			};


		}
	}
}
