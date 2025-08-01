#pragma warning disable IDE0017, IDE0090
using FluentAssertions;
using System;
using System.Globalization;
using Xbim.InformationSpecifications.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Xbim.InformationSpecifications.Tests
{

	public class ValueContraintTests
	{
		private readonly ITestOutputHelper output;

		public ValueContraintTests(ITestOutputHelper output)
		{
			this.output = output;
			//CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("it");  // Formats strings as 98.765,43
		}

		[Fact]
		public void XbimIfcElementsHandled()
		{
			var id = "2O2Fr$t4X7Zf8NOew3FLOH";
			Ifc2x3.UtilityResource.IfcGloballyUniqueId ifcGuid = new(id);
			var vc = new ValueConstraint(id); // this is a string constraint
			vc.IsSatisfiedBy(ifcGuid).Should().BeTrue("string == IfcGuid");

			var undefinedString = ValueConstraint.SingleUndefinedExact(id);
			undefinedString.IsSatisfiedBy(ifcGuid).Should().BeTrue("object == IfcGuid");

			var differentId = "2O2Fr$t4X7Zf8NOew3FLOh";    // different due to lower-case 'h'
			var anotherString = new ValueConstraint(differentId);
			anotherString.IsSatisfiedBy(ifcGuid).Should().BeFalse("IfcGuids are different");
		}

		[Fact]
		public void ExactContraintSatisfactionTest()
		{
			var vc = new ValueConstraint(NetTypeName.String, "2");
			vc.IsSatisfiedBy("2").Should().BeTrue();
			vc.IsSatisfiedBy("1").Should().BeFalse();
			vc.IsSatisfiedBy(1).Should().BeFalse();
			vc.IsSatisfiedBy(2).Should().BeTrue(); // conversion ToString is valid match

			vc = new ValueConstraint(375.230);
			vc.IsSatisfiedBy(375.23).Should().BeTrue();
			vc.IsSatisfiedBy("375.230").Should().BeTrue("String literals are cast");
			vc.IsSatisfiedBy(375.230000).Should().BeTrue();
			vc.IsSatisfiedBy(375.230001).Should().BeTrue();     // within 1e-6 tolerance
			vc.IsSatisfiedBy(375.229999).Should().BeTrue();     // ""
			vc.IsSatisfiedBy(375.230500).Should().BeFalse();    // not within 1e-6 tolerance
			vc.IsSatisfiedBy(375.229500).Should().BeFalse();    // ""

			vc = new ValueConstraint(375.230m);
			vc.IsSatisfiedBy(375.230m).Should().BeTrue();
			vc.IsSatisfiedBy(375.23m).Should().BeFalse();
			vc.IsSatisfiedBy("375.230").Should().BeTrue("String literals are cast");


			vc = new ValueConstraint("red");
			vc.IsSatisfiedBy("red").Should().BeTrue();
			vc.IsSatisfiedBy("blue").Should().BeFalse();

			vc = new ValueConstraint(NetTypeName.Floating);
			vc.IsSatisfiedBy(2f).Should().BeTrue();
			vc.IsSatisfiedBy("blue").Should().BeFalse();
			vc.IsSatisfiedBy(2d).Should().BeTrue();
			vc.IsSatisfiedBy("2.0").Should().BeTrue();

			vc = new ValueConstraint(12345678.910);
			vc.BaseType = NetTypeName.Undefined;
			vc.IsSatisfiedBy(12345678.910).Should().BeTrue();
			vc.IsSatisfiedBy("12345678.91").Should().BeTrue("String equivalent when decimal normalised");
			vc.IsSatisfiedBy("12345678.910").Should().BeFalse("Extra zero on decimal");
		}

		[Fact]
		public void BuildingSmartConstraintValues()
		{
			// in case where the type is not defined,
			// we cast the value of the string to the type provided to see if they match
			//
			var vc = new ValueConstraint(NetTypeName.Undefined, "42.0");
			vc.IsSatisfiedBy(42d).Should().BeTrue();
		}

		[Fact]
		public void ConstraintFromBasicString()
		{
			ValueConstraint vc = "Some";
			vc.IsSatisfiedBy("Some").Should().BeTrue();
			vc.IsSatisfiedBy("SomeOther").Should().BeFalse();

			vc.IsEmpty().Should().BeFalse();
			vc.IsSingleExact(out var _).Should().BeTrue();
		}

		[Fact]
		public void ConstraintWithSpecificallyFormattedNumbers()
		{
			// from IDS test attributes/fail-only_specifically_formatted_numbers_are_allowed_2_4
			ValueConstraint vc = "123,4.5";
			vc.IsSatisfiedBy("1234.5").Should().BeFalse();
			vc.IsSatisfiedBy(1234.5d).Should().BeFalse();
		}

		[Fact]
		public void ConstraintWithSpecificallyFormattedNumbers2()
		{
			// from IDS test attributes/fail-only_specifically_formatted_numbers_are_allowed_2_4
			ValueConstraint vc = "43,3";
			vc.IsSatisfiedBy("42.3").Should().BeFalse();
			vc.IsSatisfiedBy(42.3d).Should().BeFalse();
		}

		[Fact]
		public void LongValueTypeIsInferred()
		{
			// from IDS test    property\pass-integer_values_are_checked_using_type_casting_1_4.ifc
			// and              attribute\pass-integers_follow_the_same_rules_as_numbers.ids
			ValueConstraint vc = new ValueConstraint(42);
			vc.BaseType = NetTypeName.Undefined;    // E.g. on Attribute values we won't have a BaseType on a simpleValue constraint
			vc.IsSatisfiedBy(42L).Should().BeTrue("42==42");
		}

		[Fact]
		public void IntValueTypeIsInferred()
		{
			ValueConstraint vc = new ValueConstraint(42);
			vc.BaseType = NetTypeName.Undefined;
			vc.IsSatisfiedBy(42).Should().BeTrue("42==42");
		}

		[Theory(DisplayName = nameof(UnSpeficiedBoolValue))]
		[InlineData("false", false, true)]
		[InlineData("FALSE", false, false)]
		[InlineData("0", false, true)]
		[InlineData("true", true, true)]
		[InlineData("TRUE", true, false)]
		[InlineData("1", true, true)]
		public void UnSpeficiedBoolValue(string value, bool comparison, bool satisfied)
		{
			var vc = ValueConstraint.SingleUndefinedExact(value);
			vc.BaseType.Should().Be(NetTypeName.Undefined, "BaseType should be Undefined for unspecified bool value");
			vc.IsSatisfiedBy(comparison).Should().Be(satisfied);
		}

		[Fact]
		public void DecimalValueTypeIsInferred()
		{
			ValueConstraint vc = new ValueConstraint(42m);
			vc.BaseType = NetTypeName.Undefined;
			vc.IsSatisfiedBy(42m).Should().BeTrue("42m==42m");
		}

		[Fact]
		public void FloatValueTypeIsInferred()
		{
			ValueConstraint vc = new ValueConstraint(42f);
			vc.BaseType = NetTypeName.Undefined;
			vc.IsSatisfiedBy(42d).Should().BeTrue("42f==42d");

		}

		[Fact]
		public void DoubleValueTypeIsInferred()
		{
			ValueConstraint vc = new ValueConstraint(42d);
			vc.BaseType = NetTypeName.Undefined;
			vc.IsSatisfiedBy(42f).Should().BeTrue("42d==42f");

		}

		// For Completeness
		[Fact]
		public void TimeSpanValueTypeIsInferred()
		{
			ValueConstraint vc = new ValueConstraint(TimeSpan.FromDays(42).ToString());
			vc.BaseType = NetTypeName.Undefined;
			vc.IsSatisfiedBy(TimeSpan.FromDays(42)).Should().BeTrue("42==42");

		}

		[Fact]
		public void CaseSensitiviyTests()
		{
			ValueConstraint t2 = "ABC";
			t2.BaseType = NetTypeName.String;
			t2.IsSatisfiedBy("abc").Should().BeFalse();
			t2.IsSatisfiedIgnoringCaseBy("abc").Should().BeTrue();

			var t = ValueConstraint.CreatePattern("CDE");
			t.IsSatisfiedBy("cde").Should().BeFalse();
			t.IsSatisfiedIgnoringCaseBy("cde").Should().BeTrue();
		}


		[Fact]
		public void RangeConstraintSatisfactionTest()
		{
			var vc = new ValueConstraint(NetTypeName.Double);
			var t = new RangeConstraint()
			{
				MinValue = 2.ToString(),
				MinInclusive = true,
				MaxValue = 4.ToString(),
				MaxInclusive = true,
			};
			vc.AddAccepted(t);

			vc.IsSatisfiedBy(1d).Should().BeFalse();
			vc.IsSatisfiedBy(2d).Should().BeTrue();
			vc.IsSatisfiedBy(2.01).Should().BeTrue();
			vc.IsSatisfiedBy(3.99).Should().BeTrue();
			vc.IsSatisfiedBy(4d).Should().BeTrue();
			vc.IsSatisfiedBy(4.01).Should().BeFalse();

			t.MinInclusive = false;
			t.MaxInclusive = false;

			vc.IsSatisfiedBy(1d).Should().BeFalse();
			vc.IsSatisfiedBy(2d).Should().BeFalse();
			vc.IsSatisfiedBy(2.01d).Should().BeTrue();
			vc.IsSatisfiedBy(3.99d).Should().BeTrue();
			vc.IsSatisfiedBy(4d).Should().BeFalse();
			vc.IsSatisfiedBy(4.01d).Should().BeFalse();
		}

		[InlineData(NetTypeName.Integer)]
		//[InlineData(NetTypeName.Undefined)]   // TODO: Notsupported. Range Checks require BaseType, 
		[Theory]
		public void RangeConstraintSupportsNegative(NetTypeName type)
		{
			var vc = new ValueConstraint(type);
			var t = new RangeConstraint()
			{
				MinValue = (-32).ToString(),
				MinInclusive = true,
				MaxValue = (-1).ToString(),
				MaxInclusive = true,
			};
			vc.AddAccepted(t);
			vc.IsValid().Should().BeTrue();

			vc.IsSatisfiedBy(-32).Should().BeTrue();
			vc.IsSatisfiedBy(-31).Should().BeTrue();
			vc.IsSatisfiedBy(-1).Should().BeTrue();
			// Fails
			vc.IsSatisfiedBy(0).Should().BeFalse();
			vc.IsSatisfiedBy(-33).Should().BeFalse();
		}

		[InlineData("42", 42)]
		[InlineData("1", 1.000002d)]
		[InlineData("1", 0.999998d)]
		[Theory]
		public void ExactContraintSupportsRealTolerances(string constraint, double value, bool expectSatisfied = true)
		{
			var vc = new ValueConstraint(NetTypeName.Double, constraint);

			vc.IsSatisfiedBy(value).Should().Be(expectSatisfied);
		}

		[Fact]
		public void RangeConstraintSupportsNegativeFloats()
		{
			var vc = new ValueConstraint(NetTypeName.Floating);
			var t = new RangeConstraint()
			{
				MinValue = (-32).ToString(),
				MinInclusive = true,
				MaxValue = (-1).ToString(),
				MaxInclusive = true,
			};
			vc.AddAccepted(t);
			vc.IsValid().Should().BeTrue();

			vc.IsSatisfiedBy(-32f).Should().BeTrue();
			vc.IsSatisfiedBy(-31f).Should().BeTrue();
			vc.IsSatisfiedBy(-1f).Should().BeTrue();
			// Fails
			vc.IsSatisfiedBy(0f).Should().BeFalse();
			vc.IsSatisfiedBy(-33f).Should().BeFalse();
		}

		[Fact]
		public void ExclusiveRangeConstraintShrinkTolerance()
		{
			var vc = new ValueConstraint(NetTypeName.Floating);

			// 0 < x < 100
			var t = new RangeConstraint()
			{
				MinValue = (0).ToString(),
				MinInclusive = false,
				MaxValue = (100).ToString(),
				MaxInclusive = false,
			};
			vc.AddAccepted(t);
			vc.IsValid().Should().BeTrue();

			vc.IsSatisfiedBy(100d).Should().BeFalse();
			vc.IsSatisfiedBy(0d).Should().BeFalse();

			vc.IsSatisfiedBy(99.999999d).Should().BeFalse();
			vc.IsSatisfiedBy(-1e-6d).Should().BeFalse();


			vc.IsSatisfiedBy(1e-6d).Should().BeTrue(); // Was False in Initial IDS1.0 now True
													   // True
			vc.IsSatisfiedBy(2e-6d).Should().BeTrue();
			vc.IsSatisfiedBy(99.999d).Should().BeTrue();
		}

		[Fact]
		public void DecimalRangesAreInclusive()
		{
			var vc = new ValueConstraint(NetTypeName.Decimal);
			var t = new RangeConstraint()
			{
				MinValue = 42.ToString(),
				MinInclusive = true,
				MaxValue = 42.ToString(),
				MaxInclusive = true,
			};
			vc.AddAccepted(t);

			vc.IsSatisfiedBy(42.1).Should().BeFalse("40.1 failure");
			vc.IsSatisfiedBy(41.99999f).Should().BeFalse("41.9999 failure");    // Note 41.99999f rounds to 42.0 so will satisfy
			vc.IsSatisfiedBy(42L).Should().BeTrue("42L failure");
			vc.IsSatisfiedBy(42M).Should().BeTrue("42M failure");
			vc.IsSatisfiedBy(42d).Should().BeTrue("42d failure");
		}

		[Fact]
		public void EnumConstraintSatisfactionTest()
		{
			var vc = new ValueConstraint(NetTypeName.Integer);
			vc.AddAccepted(new ExactConstraint(30.ToString()));
			vc.AddAccepted(new ExactConstraint(60.ToString()));
			vc.AddAccepted(new ExactConstraint(90.ToString()));

			vc.IsSatisfiedBy(1d).Should().BeFalse("1d failure");
			vc.IsSatisfiedBy(30L).Should().BeTrue("30L failure");
			vc.IsSatisfiedBy(60).Should().BeTrue("60 failure");
			vc.IsSatisfiedBy(60L).Should().BeTrue("60L failure");
		}


		[InlineData("text", "Text")]
		[InlineData("à rénover", "A RENOVER")]
		[InlineData("abîmer", "Abimer")]
		[InlineData("tårn", "Tarn")]

		[Theory]
		public void CanCompareCaseInsensitivelyIgnoringAccents(string input, string constraint)
		{

			ValueConstraint vc = constraint;
			vc.IsSatisfiedBy(input, true).Should().BeTrue();

		}

		[InlineData(true)]
		[InlineData(false)]
		[Theory(Skip = "Not supported after IDS 1.0 Spec Change (IDS#318)")]
		public void DoubleValueSupports1e6Tolerances(bool setBaseType)
		{
			// from IDS test property/pass-floating_point_numbers_are_compared_with_a_1e_6_tolerance_1_*.ifc && attribute equivalents
			ValueConstraint vc = new ValueConstraint(42d);
			if (!setBaseType)
				vc.BaseType = NetTypeName.Undefined;

			vc.IsSatisfiedBy(42.000042d).Should().BeTrue("Within 1e-6 tolerances - high");
			vc.IsSatisfiedBy(41.999958d).Should().BeTrue("Within 1e-6 tolerances - low");

			vc.IsSatisfiedBy(42.000084d).Should().BeFalse("Outside 1e-6 tolerances - high");
			vc.IsSatisfiedBy(41.999916d).Should().BeFalse("Outside 1e-6 tolerances - low");
		}

		[InlineData(true)]
		[InlineData(false)]
		[Theory(Skip = "Not supported after IDS 1.0 Spec Change (IDS#318)")]
		public void NegativeDoubleValueSupports1e6Tolerances(bool setBaseType)
		{
			// from IDS test property/pass-floating_point_numbers_are_compared_with_a_1e_6_tolerance_1_*.ifc && attribute equivalents
			ValueConstraint vc = new ValueConstraint(-1);
			if (setBaseType)
			{
				vc.BaseType = NetTypeName.Double; // Set explicitly since BaseType defaults implicitly to Integer
			}
			else
			{
				vc.BaseType = NetTypeName.Undefined;
			}

			vc.IsSatisfiedBy(-1.000002d).Should().BeTrue("Within 1e-6 tolerances - low");
			vc.IsSatisfiedBy(-0.999998d).Should().BeTrue("Within 1e-6 tolerances - high");

			vc.IsSatisfiedBy(-1.000003d).Should().BeFalse("Outside 1e-6 tolerances - low");
			vc.IsSatisfiedBy(-0.999997d).Should().BeFalse("Outside 1e-6 tolerances - high");
		}


		// Previously considered in range (true)
		[InlineData(41.999958d, false, "originally within 1e-6 min tolerances - inclusive")]
		[InlineData(50.000042d, false, "originally within 1e-6 max tolerances - inclusive")]

		[InlineData(41.999916d, false, "outside 1e-6 min tolerances - inclusive")]
		[InlineData(50.000084d, false, "outside 1e-6 max tolerances - inclusive")]
		[Theory]
		public void DoubleValueInclusiveRangesSupportTolerance(double input, bool expectedToSatisfy, string reason)
		{
			// i.e. testing whether we're within the range 42-50 'for all intents and purposes'
			// e.g. Height must be <= 50m. 50.00000000001 is just an error introduced by FP precision

			var vc = new ValueConstraint(NetTypeName.Double);
			var t = new RangeConstraint()
			{
				MinValue = 42.ToString(),
				MinInclusive = true,
				MaxValue = 50.ToString(),
				MaxInclusive = true,
			};
			vc.AddAccepted(t);

			vc.IsSatisfiedBy(input).Should().Be(expectedToSatisfy, $"{input} is {reason}");
		}

		// Previously considered in range 
		[InlineData(-41.999958d, false, "within 1e-6 min tolerances - inclusive")]
		[InlineData(-50.000042d, false, "within 1e-6 max tolerances - inclusive")]

		[InlineData(-45d, true, "well within")]
		[InlineData(-41.999916d, false, "outside 1e-6 min tolerances - inclusive")]
		[InlineData(-50.000084d, false, "outside 1e-6 max tolerances - inclusive")]
		[Theory]
		public void DoubleNegativeValueInclusiveRangesSupportTolerance(double input, bool expectedToSatisfy, string reason)
		{


			var vc = new ValueConstraint(NetTypeName.Double);
			var t = new RangeConstraint()
			{
				MinValue = (-50).ToString(),
				MinInclusive = true,
				MaxValue = (-42).ToString(),
				MaxInclusive = true,
			};

			vc.AddAccepted(t);
			vc.IsValid().Should().BeTrue();

			vc.IsSatisfiedBy(input).Should().Be(expectedToSatisfy, $"{input} is {reason}");
		}

		[InlineData(41.999958d, false, "outside min tolerances for exclusive ranges")]
		[InlineData(41.999916d, false, "outside min tolerances for exclusive ranges")]
		[InlineData(50.000042d, false, "outside max tolerances for exclusive ranges")]
		[InlineData(50.000084d, false, "outside max tolerances for exclusive ranges")]
		[InlineData(42.000044d, true, "inside min tolerances for exclusive ranges")]
		[InlineData(49.999948, true, "inside max tolerances for exclusive ranges")]

		// Following were false in IDS1.0 but now inside
		[InlineData(42.000001d, true, "Inside but outside min tolerances for exclusive ranges")]
		[InlineData(49.999999d, true, "Inside but outside max tolerances for exclusive ranges")]
		[Theory]
		public void DoubleValueExclusiveRangesDoNotSupportTolerance(double input, bool expectedToSatisfy, string reason)
		{
			// Tolerences on Exclusive ranges make no sense
			// E.g. Height must be < 50m. Assuming 50 fails, so must 50.0000000000001
			// This does mean 49.9999999999998 < 50 succeeds - despite this potentially being an artefact of FP imprecision
			var vc = new ValueConstraint(NetTypeName.Double);

			// 42 < x < 50
			var t = new RangeConstraint()
			{
				MinValue = 42.ToString(),
				MinInclusive = false,
				MaxValue = 50.ToString(),
				MaxInclusive = false,
			};
			vc.AddAccepted(t);

			vc.IsSatisfiedBy(input).Should().Be(expectedToSatisfy, $"{input} is {reason}");

			// TODO: Consider semantic paradox where:
			// 41.999958d satisfies being in the range 42-50 inclusive, but also satisfies being < 42 exclusive
		}

		[Fact]
		public void ExclusiveRangeDoNotSupportToleranceMax()
		{
			var vc = new ValueConstraint(NetTypeName.Double);

			// x < 0
			var exclusive = new RangeConstraint()
			{
				MaxValue = 0.ToString(),
				MaxInclusive = false,

			};
			vc.AddAccepted(exclusive);

			vc.IsSatisfiedBy(-0.0000009d).Should().BeTrue();


		}

		[Fact]
		public void ExclusiveRangeDoNotSupportToleranceMin()
		{
			var vc = new ValueConstraint(NetTypeName.Double);

			// x > 0
			var exclusive = new RangeConstraint()
			{
				MinValue = 0.ToString(),
				MinInclusive = false,

			};
			vc.AddAccepted(exclusive);

			vc.IsSatisfiedBy(0.0000009d).Should().BeTrue();

		}

		[Fact]
		public void InclusiveRangeDoNotSupportToleranceMax()
		{
			var vc = new ValueConstraint(NetTypeName.Double);
			var exclusive = new RangeConstraint()
			{
				MaxValue = 0.ToString(),
				MaxInclusive = true,

			};
			vc.AddAccepted(exclusive);

			vc.IsSatisfiedBy(1e-6).Should().BeFalse();  // Was True in initial 1.0 IDS but late change removed support

		}

		[InlineData(1.2345678919873e-22d)]    // Not Supported as we Round to 6 DP
		[InlineData(1.2345678919873e-6d)]
		[InlineData(1.2345678919873e22d)]
		[InlineData(1234567891.9873d)]
		[InlineData(0d)]
		[InlineData(-1d)] // Edge-case where FP tolerance bounds reduce to zero
		[InlineData(-1e10d)]
		[InlineData(-1e-5d)]
		[InlineData(-1e-6d)]
		[InlineData(-1.2345678919873e22d)]
		[Theory]
		public void ExtremeRealValuesCanBeTested(double value)
		{
			var vc = new ValueConstraint(value);

			vc.IsSatisfiedBy(value).Should().BeTrue();

		}

		[InlineData(NetTypeName.Decimal)]
		[InlineData(NetTypeName.Floating)]
		[InlineData(NetTypeName.Double)]
		[InlineData(NetTypeName.Integer)]
		[InlineData(NetTypeName.Undefined)]
		[Theory]
		public void CultureShouldNotAffectResults(NetTypeName type)
		{
			var constraint = new ValueConstraint(type);
			//var amnt = 98765.43d;
			object amnt = type switch
			{
				NetTypeName.Decimal => 98765.432m,
				NetTypeName.Floating => 98765.432f,
				NetTypeName.Double => 98765.432d,
				NetTypeName.Integer => 98765L,
				_ => 98765.432d
			};

			var formatable = amnt as IFormattable;
			var exact = new ExactConstraint(formatable!.ToString(null, CultureInfo.InvariantCulture)!);
			constraint.AddAccepted(exact);

			// Act
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("it");  // Formats strings as 98.765,43
			constraint.IsSatisfiedBy(amnt).Should().BeTrue();
		}

		[InlineData(100000d, 99999.899999d, 100000.100001d)]
		[InlineData(10000d, 9999.989999d, 10000.010001d)]
		[InlineData(1000d, 999.998999d, 1000.001001d)]
		[InlineData(100d, 99.999899d, 100.000101d)]
		[InlineData(10d, 9.999989d, 10.000011d)]
		[InlineData(1d, 0.999998d, 1.000002d)]
		[InlineData(0.1d, 0.0999989d, 0.1000011d)]
		[InlineData(0.01d, 0.00999899d, 0.01000101d)]
		[InlineData(0.001d, 0.000998999d, 0.001001001d)]
		[InlineData(1e-06, 0d, 2e-06)]
		[InlineData(0.0000001d, -0.0000009000001d, 0.0000011000001d)]

		[InlineData(0d, -1e-06, 1e-06)]

		[InlineData(-0.0000001, -0.0000011000001, 0.0000009000001)]
		[InlineData(-1e-06, -2e-06, 0d)]
		[InlineData(-0.1d, -0.1000011000d, -0.0999989d)]
		[InlineData(-1d, -1.000002d, -0.999998d)]
		[InlineData(-10d, -10.0000110000d, -9.999989d)]
		[InlineData(-100d, -100.0001010000d, -99.999899d)]
		[InlineData(-1000000d, -1000001.0000010000d, -999998.999999d)]
		[Theory]
		public void RealHelperBoundsPrecisionTests(double value, double expectedLower, double expectedUpper)
		{
			(var lower, var upper) = RealHelper.GetPrecisionBounds(value);

			lower.Should().BeApproximately(expectedLower, 1e-10);
			upper.Should().BeApproximately(expectedUpper, 1e-10);
		}

		[InlineData("42")]
		[InlineData("42.0")]
		[InlineData("42.")]
		[InlineData("-42", -42)]
		[InlineData("-42.0", -42)]
		[InlineData("-42.", -42)]
		[InlineData("0", 0)]
		[InlineData("0.", 0)]
		[InlineData("0.0", 0)]
		[Theory]
		public void UndefinedIntegerParsingSupportsRedundantDecimals(string intText, int expected = 42)
		{
			// Integer Parsing should permit specification of a zero decimal

			// from attribute/pass-integers_follow_the_same_rules_as_numbers_2_2
			//      /property/pass-integer_values_are_checked_using_type_casting_3_4.ifc
			var constraint = new ValueConstraint(NetTypeName.Undefined);
			var exact = new ExactConstraint(intText);
			constraint.AddAccepted(exact);

			constraint.IsSatisfiedBy(expected).Should().BeTrue();
		}

		[Fact]
		public void TypedIntegerRangesAreSatisfiedByReals()
		{
			// E.g. Integer Range Constraint 0 <= x <= 10 can be satified by 5d
			// from restriction/pass-a_bound_can_be_inclusive_2_3.ifc etc

			var constraint = new ValueConstraint(NetTypeName.Integer);
			var exact = new RangeConstraint("0", true, "10", true);
			constraint.AddAccepted(exact);

			constraint.IsSatisfiedBy(5d).Should().BeTrue("Double");
			constraint.IsSatisfiedBy(5f).Should().BeTrue("Float");
			constraint.IsSatisfiedBy(5.0m).Should().BeTrue("Decimal");
		}

		[Fact]
		public void TypedRealRangesAreSatisfiedByIntegers()
		{
			// E.g. Real Range Constraint 0d <= x <= 10d can be satified by 5L

			var constraint = new ValueConstraint(NetTypeName.Floating);
			var exact = new RangeConstraint("0", true, "10", true);
			constraint.AddAccepted(exact);

			constraint.IsSatisfiedBy(5L).Should().BeTrue("Long");
			constraint.IsSatisfiedBy(5).Should().BeTrue("Int");
			// And other reals
			constraint.IsSatisfiedBy(5f).Should().BeTrue("Float");
			constraint.IsSatisfiedBy(5.0m).Should().BeTrue("Decimal");
		}

		[InlineData("0.0")]
		[InlineData("0.1")]
		[InlineData("0")]
		[InlineData("100")]
		[InlineData("1,000")]
		[InlineData("1e3")]
		[InlineData("-1", false)]
		[InlineData("-1e3", false)]
		[InlineData("", false)]
		[InlineData("invalid", false)]
		[InlineData("0.0.1", false)]
		[InlineData("£1000.00", false)]
		[Theory]
		public void StringFormatedNumericsCanSatisfyNumericRangeConstraints(string value, bool expected = true)
		{
			// Can be typical to receive numeric values in IfcText fields. While purist view may be that the datatype should be fixed
			// it's beneficial to be able to very the contents numerically. E.g. ReplacementCost must be > 0.0
			var constraint = new ValueConstraint(NetTypeName.Double);
			var exact = new RangeConstraint("0", true, null, true);
			constraint.AddAccepted(exact);

			constraint.IsSatisfiedBy(value).Should().Be(expected, $"{value} >= 0");
		}


		[Fact]
		public void RealTolerancesAnalysis()
		{
			for (int i = 5; i > -8; i--)
			{
				var value = Math.Pow(10, i);
				DoTest(value);
			}
			DoTest(0);
			for (int i = -7; i < 6; i++)
			{
				var value = 0 - Math.Pow(10, i);
				DoTest(value);
			}

			void DoTest(double value)
			{
				(var lower, var upper) = RealHelper.GetPrecisionBounds(value);
				var spread = upper - lower;

				output.WriteLine($"{value,12} = {lower,24:R} >= x <= {upper,24:R} Δ {spread:R}");
				//output.WriteLine($"{value,12},{lower,24:R},{upper,24:R},{spread:R}");
			}
		}
	}
}
