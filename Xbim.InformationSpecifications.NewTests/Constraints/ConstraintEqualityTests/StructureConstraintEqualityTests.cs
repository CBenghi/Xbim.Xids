#pragma warning disable xUnit1042
using FluentAssertions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Facets
{
	public class StructureConstraintEqualityTests
	{
		[Theory]
		[MemberData(nameof(GetSingleAttributes))]
		public void AttributeEqualMatchImplementation(StructureConstraint t, StructureConstraint tSame)
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
		public void AttributeEqualNotMatchImplementation(StructureConstraint one, StructureConstraint other)
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

		public static IEnumerable<StructureConstraint> GetDifferentAttributes()
		{
			yield return new StructureConstraint();
			yield return new StructureConstraint() { FractionDigits = 3 };
			yield return new StructureConstraint() { Length = 3 };
			yield return new StructureConstraint() { MaxLength = 3 };
			yield return new StructureConstraint() { MinLength = 3 };
			yield return new StructureConstraint() { TotalDigits = 3 };
			yield return new StructureConstraint()
			{
				FractionDigits = 1,
				TotalDigits = 3
			};
			yield return new StructureConstraint()
			{
				MinLength = 1,
				MaxLength = 3
			};
			yield return new StructureConstraint()
			{
				TotalDigits = 5,
			};
		}
	}
}
