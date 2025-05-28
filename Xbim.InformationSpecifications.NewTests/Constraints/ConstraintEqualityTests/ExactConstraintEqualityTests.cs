#pragma warning disable xUnit1042
using FluentAssertions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Facets
{
	public class ExactConstraintEqualityTests
	{
		[Theory]
		[MemberData(nameof(GetSingleAttributes))]
		public void AttributeEqualMatchImplementation(ExactConstraint t, ExactConstraint tSame)
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
		public void AttributeEqualNotMatchImplementation(ExactConstraint one, ExactConstraint other)
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

		public static IEnumerable<ExactConstraint> GetDifferentAttributes()
		{
			yield return new ExactConstraint("2");
			yield return new ExactConstraint(string.Empty);
			yield return new ExactConstraint(string.Empty) { Value = "3" };
		}
	}
}
