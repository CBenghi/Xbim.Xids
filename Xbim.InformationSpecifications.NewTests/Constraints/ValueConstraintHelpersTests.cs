using FluentAssertions;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{
	public class ValueConstraintHelpersTests
	{
		[Fact]
		public void ValueConstraintIsExact()
		{
			ValueConstraint constraint;

			var stringValue = "Gatto";
			constraint = new ValueConstraint(stringValue);
			var test = constraint.IsSingleUndefinedExact(out var someVal);
			test.Should().BeFalse("starting from a string we set the type to string");
			someVal.Should().BeNull();

			test = constraint.IsSingleExact(out string? gattoVal);
			test.Should().BeTrue();
			gattoVal.Should().Be(stringValue);

			test = constraint.IsSingleExact<int>(out int intGattoVal);
			test.Should().BeFalse();
			intGattoVal.Should().Be(default);

			int IntValue = 32;
			constraint = new ValueConstraint(IntValue);
			test = constraint.IsSingleExact(out int tInt);
			test.Should().BeTrue();
			tInt.Should().Be(IntValue);

			test = constraint.IsSingleExact(out string? strVal);
			test.Should().BeFalse();
			strVal.Should().Be(default);
		}

		[Fact]
		public void SingleUndefinedExactTests()
		{
			var stringV = "someValue";
			var val = ValueConstraint.SingleUndefinedExact(stringV);
			var itIs = val.IsSingleUndefinedExact(out var retVal);
			itIs.Should().BeTrue();
			retVal.Should().Be(stringV);


			ValueConstraint t = new(2d);
			t.IsSingleUndefinedExact(out var _).Should().BeFalse();


			t = new ValueConstraint(NetTypeName.Undefined);
			t.AddAccepted(new RangeConstraint());
			t.IsSingleUndefinedExact(out var _).Should().BeFalse();


		}
	}
}
