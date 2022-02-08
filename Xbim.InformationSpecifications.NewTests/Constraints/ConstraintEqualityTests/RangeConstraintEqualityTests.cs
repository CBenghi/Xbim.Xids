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
    public class RangeConstraintEqualityTests
    {
		[Theory]
		[MemberData(nameof(GetSingleAttributes))]
		public void AttributeEqualMatchImplementation(RangeConstraint t, RangeConstraint tSame)
		{
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
		public void AttributeEqualNotMatchImplementation(RangeConstraint one, RangeConstraint other)
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

		public static IEnumerable<RangeConstraint> GetDifferentAttributes()
		{
			yield return new RangeConstraint();
			yield return new RangeConstraint() { MinValue = "1" };
			yield return new RangeConstraint() {  MinInclusive = true };
			yield return new RangeConstraint() { MaxValue = "5" };
			yield return new RangeConstraint() { MaxInclusive = true };
			yield return new RangeConstraint() { 
				MinInclusive = true,
				MinValue = "1",
				MaxInclusive = true,
				MaxValue = "5"
			};
		}
	}
}
