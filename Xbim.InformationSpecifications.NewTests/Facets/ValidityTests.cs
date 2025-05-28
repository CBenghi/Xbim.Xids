#pragma warning disable xUnit1042
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Facets
{
	public class ValidityTests
	{
		[Theory]
		[MemberData(nameof(GetValidFacets))]
		public void EmptyValidityTests(IFacet facet)
		{
			// some empty facets are not valid
			facet.IsValid().Should().BeFalse($"{facet.GetType()} should not be valid if empty");
		}

		public static IEnumerable<object[]> GetValidFacets()
		{
			yield return new object[] { new IfcPropertyFacet() };
			yield return new object[] { new AttributeFacet() };
		}


		[Theory]
		[MemberData(nameof(GetValidConstraints))]
		void Can_tell_valid_constrains(ValueConstraint constraint)
		{
			constraint.IsValid().Should().BeTrue();
		}

		public static IEnumerable<object[]> GetValidConstraints()
		{
			var c1 = new ValueConstraint(NetTypeName.Double);
			c1.AddAccepted(new ExactConstraint("12.0"));
			yield return new object[] { c1 };

			c1 = new ValueConstraint(NetTypeName.String);
			c1.AddAccepted(new ExactConstraint("12.0"));
			yield return new object[] { c1 };

			c1 = new ValueConstraint(NetTypeName.Double);
			c1.AddAccepted(new RangeConstraint("12.0", true, "12.0", false));
			yield return new object[] { c1 };

			c1 = new ValueConstraint(NetTypeName.Double);
			c1.AddAccepted(new RangeConstraint("12.0", false, "12.0", true));
			yield return new object[] { c1 };

			c1 = new ValueConstraint(NetTypeName.Double);
			c1.AddAccepted(new RangeConstraint("12.0", true, "12.0", true));
			yield return new object[] { c1 };

			c1 = new ValueConstraint(NetTypeName.String);
			c1.AddAccepted(new PatternConstraint(""));
			yield return new object[] { c1 };

			c1 = new ValueConstraint(NetTypeName.String);
			c1.AddAccepted(new PatternConstraint("Some"));
			yield return new object[] { c1 };
		}

		[Theory]
		[MemberData(nameof(GetInvalidConstraints))]
		void Can_tell_Invalid_constrains(ValueConstraint constraint)
		{
			constraint.IsValid().Should().BeFalse();
		}
		public static IEnumerable<object[]> GetInvalidConstraints()
		{
			var c1 = new ValueConstraint(NetTypeName.Double);
			c1.AddAccepted(new ExactConstraint("someNoNumberValue"));
			yield return new object[] { c1 };

			c1 = new ValueConstraint(NetTypeName.String);
			c1.AddAccepted(new RangeConstraint("12.0", false, "12.0", false));
			yield return new object[] { c1 };

			c1 = new ValueConstraint(NetTypeName.String);
			c1.AddAccepted(new PatternConstraint("SomeInvalid["));
			yield return new object[] { c1 };

			c1 = new ValueConstraint(NetTypeName.String);
			c1.AddAccepted(new PatternConstraint("?"));
			yield return new object[] { c1 };
		}
	}
}
