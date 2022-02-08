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
    public class ValueConstraintEqualityTests
	{
		[Theory]
		[MemberData(nameof(GetSingleAttributes))]
		public void AttributeEqualMatchImplementation(ValueConstraint t, ValueConstraint tSame)
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
		public void AttributeEqualNotMatchImplementation(ValueConstraint one, ValueConstraint other)
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

		[Fact]
		public void OrderIrrelevant()
        {
			var t1 = new ValueConstraint(TypeName.String)
			{
				AcceptedValues = new List<IValueConstraint>()
				{
					new ExactConstraint("2"),
					new ExactConstraint("3")
				}
			};

			var t2 = new ValueConstraint(TypeName.String)
			{
				AcceptedValues = new List<IValueConstraint>()
				{
					new ExactConstraint("3"),
					new ExactConstraint("2")
				}
			};

			var areEqual = t1.Equals(t2);
			areEqual.Should().BeTrue();
		}

		public static IEnumerable<ValueConstraint> GetDifferentAttributes()
		{
			yield return new ValueConstraint();
			yield return new ValueConstraint("2");
			yield return new ValueConstraint("3");
			yield return new ValueConstraint(2.0d);
			yield return new ValueConstraint(2.0m);
			yield return new ValueConstraint(2);
			yield return new ValueConstraint(TypeName.Boolean);

			yield return new ValueConstraint(TypeName.String)
			{
				AcceptedValues = new List<IValueConstraint>()
				{
					new ExactConstraint("2"),
					new ExactConstraint("3")
				}
			};
		}
	}
}
