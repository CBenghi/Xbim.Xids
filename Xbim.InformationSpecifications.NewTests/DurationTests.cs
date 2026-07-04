using System;
using Xbim.InformationSpecifications.Values;
using Xunit;
using AwesomeAssertions;
using Microsoft.VisualBasic;

namespace Xbim.InformationSpecifications.Tests
{
	public class DurationTests
	{
		[Fact]
		public void Constructor_SetsPropertiesCorrectly()
		{
			var duration = new Duration(true, 1, 2, 3, 4, 5, 6.78m);

			duration.IsNegative.Should().BeTrue();
			duration.Years.Should().Be(1);
			duration.Months.Should().Be(2);
			duration.Days.Should().Be(3);
			duration.Hours.Should().Be(4);
			duration.Minutes.Should().Be(5);
			duration.Seconds.Should().Be(6.78m);
			duration.IsZero.Should().BeFalse();
		}

		[Fact]
		public void Constructor_Throws_OnNegativeComponents()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new Duration(false, -1, 0, 0));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Duration(false, 0, -1, 0));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Duration(false, 0, 0, -1));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Duration(false, 0, 0, 0, -1));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Duration(false, 0, 0, 0, 0, -1));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Duration(false, 0, 0, 0, 0, 0, -1m));
		}

		[Theory]
		[InlineData("P1Y", false, 1, 0, 0, 0, 0, 0)]
		[InlineData("P2M", false, 0, 2, 0, 0, 0, 0)]
		[InlineData("P3D", false, 0, 0, 3, 0, 0, 0)]
		[InlineData("PT4H", false, 0, 0, 0, 4, 0, 0)]
		[InlineData("PT5M", false, 0, 0, 0, 0, 5, 0)]
		[InlineData("PT6S", false, 0, 0, 0, 0, 0, 6)]
		[InlineData("PT6.78S", false, 0, 0, 0, 0, 0, 6.78)]
		[InlineData("-P1Y2M3DT4H5M6.78S", true, 1, 2, 3, 4, 5, 6.78)]
		[InlineData("P0D", false, 0, 0, 0, 0, 0, 0)]
		public void TryParse_ValidInputs_Succeeds(string input, bool isNegative, int years, int months, int days, int hours, int minutes, double seconds)
		{
			var success = Duration.TryParse(input, out var result);

			success.Should().BeTrue();
			result.IsNegative.Should().Be(isNegative);
			result.Years.Should().Be(years);
			result.Months.Should().Be(months);
			result.Days.Should().Be(days);
			result.Hours.Should().Be(hours);
			result.Minutes.Should().Be(minutes);
			result.Seconds.Should().Be((decimal)seconds);
		}

		[Theory]
		[InlineData("")]
		[InlineData("   ")]
		[InlineData("P")]
		[InlineData("-P")]
		[InlineData("PT")]
		[InlineData("P1YT")]
		[InlineData("1Y2M")]
		[InlineData("P1Y2MT")]
		public void TryParse_InvalidInputs_ReturnsFalse(string input)
		{
			var success = Duration.TryParse(input, out _);

			success.Should().BeFalse();
		}

		[Fact]
		public void ToString_FormatsCorrectly()
		{
			var d1 = new Duration(false, 1, 2, 3, 4, 5, 6.78m);
			d1.ToString().Should().Be("P1Y2M3DT4H5M6.78S");

			var d2 = new Duration(true, 0, 0, 0, 0, 0, 0m);
			d2.ToString().Should().Be("P0D");

			var d3 = new Duration(false, 0, 0, 10, 0, 0, 0m);
			d3.ToString().Should().Be("P10D");
		}

		[Fact]
		public void TimeSpanConversions_Work()
		{
			var ts = new TimeSpan(5, 4, 3, 2, 100);
			Duration duration = ts;

			duration.Days.Should().Be(5);
			duration.Hours.Should().Be(4);
			duration.Minutes.Should().Be(3);
			duration.Seconds.Should().Be(2.1m);

			TimeSpan back = (TimeSpan)duration;
			back.Should().Be(ts);
		}

		[Fact]
		public void EqualityAndComparison_Work()
		{
			var d1 = new Duration(false, 1, 0, 0);
			var d2 = new Duration(false, 1, 0, 0);
			var d3 = new Duration(false, 0, 12, 0); // 12 months approximated to 360 days in ToTimeSpan
			var d4 = new Duration(false, 0, 0, 365); // 365 days in ToTimeSpan

			(d1 == d2).Should().BeTrue();
			d1.Equals(d2).Should().BeTrue();
			d1.CompareTo(d2).Should().Be(0);

			// Structural inequality but same approximated timespan
			(d1 == d4).Should().BeFalse();
			d1.CompareTo(d4).Should().Be(0); // P1Y (365 days) and P365D have same approximated TimeSpan

			(d3 < d4).Should().BeTrue(); // 360 days < 365 days
			(d4 > d3).Should().BeTrue();
		}

		[Fact]
		public void AddAndSubtract_DateTime_AccountsForCalendarContext()
		{
			// Test adding/subtracting 1 month to/from different dates (irregular month lengths)
			var OneMonth = new Duration(false, 0, 1, 0); // P1M

			var jan31 = new DateTime(2026, 1, 31);
			(jan31 + OneMonth).Should().Be(new DateTime(2026, 2, 28)); // Jan 31 + 1 month = Feb 28

			var feb28_Leap = new DateTime(2024, 2, 28);
			(feb28_Leap + OneMonth).Should().Be(new DateTime(2024, 3, 28));

			var feb29_Leap = new DateTime(2024, 2, 29);
			(feb29_Leap + OneMonth).Should().Be(new DateTime(2024, 3, 29));

			var mar31_Leap = new DateTime(2024, 3, 31);
			(mar31_Leap - OneMonth).Should().Be(new DateTime(2024, 2, 29));

			var mar31_NoLeap = new DateTime(2026, 3, 31);
			(mar31_NoLeap - OneMonth).Should().Be(new DateTime(2026, 2, 28));

			// Test subtracting
			var mar31 = new DateTime(2026, 3, 31);
			(mar31 - OneMonth).Should().Be(new DateTime(2026, 2, 28));

			// Test negative duration addition
			var sub1Month = new Duration(true, 0, 1, 0); // -P1M
			(mar31 + sub1Month).Should().Be(new DateTime(2026, 2, 28));
		}

		[Fact]
		public void AddAndSubtract_DateTimeOffset_AccountsForCalendarContext()
		{
			var add1Year = new Duration(false, 1, 0, 0); // P1Y
			var leapDay = new DateTimeOffset(2024, 2, 29, 12, 0, 0, TimeSpan.FromHours(1));
			var leapDayDate = new DateTime(2024, 2, 29, 12, 0, 0);

			// 2024 is leap year, 2025 is not. Adding 1 year to Feb 29 2024 should yield Feb 28 2025
			(leapDay + add1Year).Should().Be(new DateTimeOffset(2025, 2, 28, 12, 0, 0, TimeSpan.FromHours(1)));
			(leapDay - add1Year).Should().Be(new DateTimeOffset(2023, 2, 28, 12, 0, 0, TimeSpan.FromHours(1)));

			// 2024 is leap year, 2025 is not. Adding 1 year to Feb 29 2024 should yield Feb 28 2025
			(leapDayDate + add1Year).Should().Be(new DateTime(2025, 2, 28, 12, 0, 0));
			(leapDayDate - add1Year).Should().Be(new DateTime(2023, 2, 28, 12, 0, 0));



		}
	}
}
