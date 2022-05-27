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
    public class IfcTypeFacetTests
    {
		[Theory]
		[MemberData(nameof(GetSingleAttributes))]
		public void AttributeEqualMatchImplementation(IfcTypeFacet t, IfcTypeFacet tSame)
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
		public void AttributeEqualNotMatchImplementation(IfcTypeFacet one, IfcTypeFacet other)
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

		public static IEnumerable<IfcTypeFacet> GetDifferentAttributes()
		{
			yield return new IfcTypeFacet();
			yield return new IfcTypeFacet() { IfcType = "type", };
			yield return new IfcTypeFacet() { PredefinedType = "predefined", };
			yield return new IfcTypeFacet() { IncludeSubtypes = false, };
			yield return new IfcTypeFacet() { Instructions = "someInstructions" };
			yield return new IfcTypeFacet() { Uri = "someInstructions" };
			yield return new IfcTypeFacet() { Uri = "http://www.google.com" };
			yield return new IfcTypeFacet() { 
				IfcType = "type",
				PredefinedType = "predType",
				IncludeSubtypes = false, 
			};
			yield return new IfcTypeFacet()
			{
				IfcType = "type",
				PredefinedType = "predType",
				IncludeSubtypes = true,
			};
			yield return new IfcTypeFacet()
			{
				IfcType = "type",
				PredefinedType = "predType",
				IncludeSubtypes = false,
				Instructions = "SomeInstructions",
				Uri = "http://www.google.com",
			};
		}
	}
}
