using AwesomeAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xbim.InformationSpecifications.Values;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Constraints;

public class ValueConstraintHelpersTests
{
	public ValueConstraintHelpersTests(ITestOutputHelper output)
	{
		this.outputHelper = output;
	}
	private readonly ITestOutputHelper outputHelper;

	[Fact(DisplayName = nameof(SingleConversionDebugHelper))]
	public void SingleConversionDebugHelper()
	{
		bool[] testingValues = [
			true,
			false,
			];
		foreach (var testingValue in testingValues)
		{
			var looptp = NetTypeName.Boolean;
			outputHelper.WriteLine($"Original: {testingValue} -> {testingValue}");
			var cnv = ValueConstraint.PersistValue(testingValue, looptp)!;
			outputHelper.WriteLine($"Persisted: `{cnv}`");
			var back = ValueConstraint.ParseValue(cnv, looptp);
			if (back is bool reloaded)
			{
				outputHelper.WriteLine($"reloaded: {reloaded} -> {reloaded}");
				outputHelper.WriteLine($"Equal: {reloaded.Equals(testingValue)}");
			}
			outputHelper.WriteLine("---");
		}


		var tp = NetTypeName.DateTime;
		var expectedValue = new DateTime(2026, 05, 09, 0, 0, 0, DateTimeKind.Utc);
		var stringRepresentation = ValueConstraint.PersistValue(expectedValue, tp);

		PerformSingleExactTypedTest(tp, stringRepresentation, expectedValue);
	}


	/// <summary>
	/// This test only converts in one direction from a variety ot tolerant string representations 
	/// to the expected value, and checks that the value is recognised as an exact match for the 
	/// type and value constraint. 
	/// 
	/// We want to be sure that we can recognise a variety of string representations as valid for a type, 
	/// and that they are correctly converted to the expected value, and that the IsSingleExactTyped 
	/// method works as expected.
	/// 
	/// THIS DOES NOT CHECK THAT THE ROUND TRIP IS EXACT
	/// 
	/// </summary>

	[Theory(DisplayName = nameof(SingleExactTypedTest))]
	[InlineData(NetTypeName.String, "", "")]
	[InlineData(NetTypeName.String, "A", "A")]
	[InlineData(NetTypeName.Boolean, "true", true)]
	[InlineData(NetTypeName.Boolean, "false", false)]
	[InlineData(NetTypeName.Boolean, "True", true)] // xids is more tolerant than xml
	[InlineData(NetTypeName.Boolean, "False", false)]
	[InlineData(NetTypeName.Date, "2006-12-24", "2006-12-24")]
	[MemberData(nameof(SingleExactTypedTestData))]
	public void SingleExactTypedTest(NetTypeName tp, string? stringRepresentation, object? expectedValue = null)
	{
		if (tp == NetTypeName.Date && expectedValue is string strVal)
		{
			expectedValue = DateTime.Parse(strVal);
		}
		PerformSingleExactTypedTest(tp, stringRepresentation, expectedValue);
	}

	// isolated because we wanto to be able to run a single one in a fact to help debugging if we have a problem with the data
	private void PerformSingleExactTypedTest(NetTypeName tp, string? stringRepresentation, object? expectedValue)
	{
		// creates a constraint by data type and a VALID STRING REPRESENTATION and checks that the value
		// is recognised and parsed as expected
		ArgumentNullException.ThrowIfNull(stringRepresentation, nameof(stringRepresentation));

		var valueConstraint = new ValueConstraint(tp, stringRepresentation);
		var exactC = valueConstraint.AcceptedValues?.FirstOrDefault() as ExactConstraint;
		exactC.Should().NotBeNull();
		outputHelper.WriteLine($"Testing `{tp}` with ");
		outputHelper.WriteLine($"  string representation '{stringRepresentation}' and");
		outputHelper.WriteLine($"  expected value '{expectedValue}'");
		outputHelper.WriteLine($"  expected type is '{expectedValue!.GetType().Name}'");
		outputHelper.WriteLine($"String value in Facet's ExactConstraint is '{exactC.Value}'");

		var convertedBackWithParseValue = ValueConstraint.ParseValue(stringRepresentation, tp);
		if (tp == NetTypeName.Duration && expectedValue != convertedBackWithParseValue)
		{
		}
		convertedBackWithParseValue.Should().NotBeNull();
		outputHelper.WriteLine($"{nameof(convertedBackWithParseValue)} for `{tp}` is '{convertedBackWithParseValue}' {convertedBackWithParseValue.GetType().Name}");
		outputHelper.WriteLine($"Repersisted is: {ValueConstraint.PersistValue(convertedBackWithParseValue, tp)}");

		// we check the primitive first
		convertedBackWithParseValue.Should().BeOfType(expectedValue.GetType());
		// convertedBackWithParseValue.Should().BeEquivalentTo(expectedValue, "we check the primitive conversion first for {0}", convertedBackWithParseValue.GetType().Name);

		var isSingleMatchWorks = valueConstraint.IsSingleExactTyped(out var matchedVal, out var matchedTp);
		isSingleMatchWorks.Should().BeTrue();
		matchedTp.Should().Be(tp);
		matchedVal.Should().Be(expectedValue);
	}

	/// <summary>
	/// gets test data in a format that should be unequivocally recognised when interpreted as a value constraint
	/// </summary>
	public static TheoryData<NetTypeName, string?, object?> SingleExactTypedTestData()
	{
		var data = new TheoryData<NetTypeName, string?, object?>();
		DateTime[] dates = [
			new DateTime(2026, 05, 09),
			new DateTime(2026, 12, 14),
			new DateTime(2026, 12, 14, 11, 34, 23, DateTimeKind.Utc),
			new DateTime(2026, 12, 14, 11, 34, 23, DateTimeKind.Local),
			new DateTime(2026, 12, 14, 11, 34, 23, DateTimeKind.Unspecified),
			DateTime.MinValue,
			DateTime.MaxValue,
			];
		foreach (DateTime dt in dates)
		{
			data.Add(NetTypeName.Date,  ValueConstraint.PersistValue(dt, NetTypeName.Date), dt.Date);
			data.Add(NetTypeName.DateTime, ValueConstraint.PersistValue(dt, NetTypeName.DateTime), dt);
		}

		Uri[] uris = [
			new Uri("http://example.com"),
			new Uri("https://example.com/resource"),
			new Uri("ftp://example.com/file.txt"),
			new Uri("mailto:")];
		foreach (var entry in uris)
			data.Add(NetTypeName.Uri, ValueConstraint.PersistValue(entry, NetTypeName.Uri), entry);

		// taking extra cases from other tests, so that we are compatible with strings
		// that are ok for XML persistence
		foreach (var entry in IoTests.IoTests.GetTypeParsingTestData())
		{
			if (entry[1] is NetTypeName tp)
			{
				var str = entry[0] as string;
				var objVal = entry[2];
				data.Add(tp, ValueConstraint.PersistValue(objVal, tp), objVal);
				data.Add(tp, str, objVal);
			}
		}

		(string pers, double val)[] reals = [
			("0.0", 0.0),
			("2.0", 2.0),
			("2", 2.0),
			("0", 0.0),
			("-0", 0.0),
			("-0.0", 0.0),
			("-3", -3.0)];
		foreach (var entry in reals)
		{
			data.Add(NetTypeName.Floating, ValueConstraint.PersistValue(entry.val, NetTypeName.Floating), (float)entry.val);
			data.Add(NetTypeName.Double, ValueConstraint.PersistValue(entry.val, NetTypeName.Double), entry.val);
		}
		foreach (var entry in reals)
		{
			data.Add(NetTypeName.Floating, entry.pers, (float)entry.val);
			data.Add(NetTypeName.Double, entry.pers, entry.val);
		}

		(string pers, TimeSpan val)[] durations = [
			("0.0", TimeSpan.Zero),
			("-3", TimeSpan.FromSeconds(-3)),
			];

		return data;
	}

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
