using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Xbim.InformationSpecifications.Values
{
	/// <summary>
	/// Represents a duration of time as defined by XML Schema xs:duration.
	/// This class supports years and months components which cannot be fully expressed by .NET TimeSpan.
	/// </summary>
	public readonly struct Duration : IEquatable<Duration>, IComparable<Duration>, IComparable
	{
		private static readonly Regex DurationRegex = new Regex(
			@"^(?<sign>-)?P(?:(?<years>\d+)Y)?(?:(?<months>\d+)M)?(?:(?<days>\d+)D)?(?<time>T(?:(?<hours>\d+)H)?(?:(?<minutes>\d+)M)?(?:(?<seconds>\d+(?:\.\d+)?)S)?)?$",
			RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>
		/// Gets a value indicating whether the duration is negative.
		/// </summary>
		public bool IsNegative { get; }

		/// <summary>
		/// Gets the years component of the duration.
		/// </summary>
		public int Years { get; }

		/// <summary>
		/// Gets the months component of the duration.
		/// </summary>
		public int Months { get; }

		/// <summary>
		/// Gets the days component of the duration.
		/// </summary>
		public int Days { get; }

		/// <summary>
		/// Gets the hours component of the duration.
		/// </summary>
		public int Hours { get; }

		/// <summary>
		/// Gets the minutes component of the duration.
		/// </summary>
		public int Minutes { get; }

		/// <summary>
		/// Gets the seconds component of the duration (including fractional seconds).
		/// </summary>
		public decimal Seconds { get; }

		/// <summary>
		/// Gets a value indicating whether the duration is zero.
		/// </summary>
		public bool IsZero => Years == 0 && Months == 0 && Days == 0 && Hours == 0 && Minutes == 0 && Seconds == 0m;

		/// <summary>
		/// Initializes a new instance of the <see cref="Duration"/> struct with individual components.
		/// </summary>
		public Duration(bool makeNegative, int years, int months, int days, int hours = 0, int minutes = 0, decimal seconds = 0m)
		{
			if (years < 0) throw new ArgumentOutOfRangeException(nameof(years));
			if (months < 0) throw new ArgumentOutOfRangeException(nameof(months));
			if (days < 0) throw new ArgumentOutOfRangeException(nameof(days));
			if (hours < 0) throw new ArgumentOutOfRangeException(nameof(hours));
			if (minutes < 0) throw new ArgumentOutOfRangeException(nameof(minutes));
			if (seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));

			IsNegative = makeNegative; // a duration IS negative if you MAKE it negative
			Years = years;
			Months = months;
			Days = days;
			Hours = hours;
			Minutes = minutes;
			Seconds = seconds;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Duration"/> struct from a <see cref="TimeSpan"/>.
		/// </summary>
		public Duration(TimeSpan timeSpan)
		{
			if (timeSpan == TimeSpan.MinValue)
			{
				IsNegative = true;
				Years = 0;
				Months = 0;
				Days = 10675199;
				Hours = 2;
				Minutes = 48;
				Seconds = 54.775808m; // TimeSpan.MinValue is -10675199.02:48:54.7758080
				return;
			}

			IsNegative = timeSpan < TimeSpan.Zero;
			if (IsNegative)
			{
				timeSpan = timeSpan.Negate();
			}

			Years = 0;
			Months = 0;
			Days = timeSpan.Days;
			Hours = timeSpan.Hours;
			Minutes = timeSpan.Minutes;
			Seconds = (decimal)(timeSpan.Ticks % TimeSpan.TicksPerMinute) / TimeSpan.TicksPerSecond;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Duration"/> struct from a string in XML xs:duration format.
		/// </summary>
		public Duration(string xmlValue)
		{
			if (TryParse(xmlValue, out var val))
			{
				IsNegative = val.IsNegative;
				Years = val.Years;
				Months = val.Months;
				Days = val.Days;
				Hours = val.Hours;
				Minutes = val.Minutes;
				Seconds = val.Seconds;
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(xmlValue), "Invalid XML xs:duration format.");
			}
		}

		/// <summary>
		/// Attempts to parse a string in XML xs:duration format into a <see cref="Duration"/> value.
		/// </summary>
		public static bool TryParse(string? input, out Duration result)
		{
			result = default;
			if (string.IsNullOrWhiteSpace(input))
				return false;

			var match = DurationRegex.Match(input);
			if (!match.Success)
				return false;

			var hasTimeGroup = match.Groups["time"].Success;
			var hasHours = match.Groups["hours"].Success;
			var hasMinutes = match.Groups["minutes"].Success;
			var hasSeconds = match.Groups["seconds"].Success;

			if (hasTimeGroup && !hasHours && !hasMinutes && !hasSeconds)
				return false; // Invalid: T designator present but no time components

			var hasYears = match.Groups["years"].Success;
			var hasMonths = match.Groups["months"].Success;
			var hasDays = match.Groups["days"].Success;

			if (!hasYears && !hasMonths && !hasDays && !hasHours && !hasMinutes && !hasSeconds)
				return false; // Invalid: at least one component must be present

			var isNegative = match.Groups["sign"].Success;

			int years = 0;
			if (hasYears && !int.TryParse(match.Groups["years"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out years))
				return false;

			int months = 0;
			if (hasMonths && !int.TryParse(match.Groups["months"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out months))
				return false;

			int days = 0;
			if (hasDays && !int.TryParse(match.Groups["days"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out days))
				return false;

			int hours = 0;
			if (hasHours && !int.TryParse(match.Groups["hours"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out hours))
				return false;

			int minutes = 0;
			if (hasMinutes && !int.TryParse(match.Groups["minutes"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out minutes))
				return false;

			decimal seconds = 0m;
			if (hasSeconds && !decimal.TryParse(match.Groups["seconds"].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out seconds))
				return false;

			result = new Duration(isNegative, years, months, days, hours, minutes, seconds);
			return true;
		}

		/// <summary>
		/// Approximates this duration as a <see cref="TimeSpan"/>, treating a year as 365 days and a month as 30 days.
		/// </summary>
		public TimeSpan ToTimeSpan()
		{
			long days = (long)Years * 365 + (long)Months * 30 + Days;
			long hours = days * 24 + Hours;
			long minutes = hours * 60 + Minutes;
			decimal seconds = (decimal)minutes * 60 + Seconds;
			long ticks = (long)(seconds * TimeSpan.TicksPerSecond);
			return new TimeSpan(IsNegative ? -ticks : ticks);
		}

		/// <summary>
		/// Adds this duration to the specified <see cref="DateTime"/> value, taking into account irregular year/month lengths and leap years.
		/// </summary>
		public DateTime AddTo(DateTime dateTime)
		{
			int sign = IsNegative ? -1 : 1;
			var result = dateTime.AddYears(Years * sign);
			result = result.AddMonths(Months * sign);
			result = result.AddDays(Days * sign);
			result = result.AddHours(Hours * sign);
			result = result.AddMinutes(Minutes * sign);
			result = result.AddSeconds((double)Seconds * sign);
			return result;
		}

		/// <summary>
		/// Subtracts this duration from the specified <see cref="DateTime"/> value, taking into account irregular year/month lengths and leap years.
		/// </summary>
		public DateTime SubtractFrom(DateTime dateTime)
		{
			int sign = IsNegative ? 1 : -1;
			var result = dateTime.AddYears(Years * sign);
			result = result.AddMonths(Months * sign);
			result = result.AddDays(Days * sign);
			result = result.AddHours(Hours * sign);
			result = result.AddMinutes(Minutes * sign);
			result = result.AddSeconds((double)Seconds * sign);
			return result;
		}

		/// <summary>
		/// Adds this duration to the specified <see cref="DateTimeOffset"/> value, taking into account irregular year/month lengths and leap years.
		/// </summary>
		public DateTimeOffset AddTo(DateTimeOffset dateTimeOffset)
		{
			int sign = IsNegative ? -1 : 1;
			var result = dateTimeOffset.AddYears(Years * sign);
			result = result.AddMonths(Months * sign);
			result = result.AddDays(Days * sign);
			result = result.AddHours(Hours * sign);
			result = result.AddMinutes(Minutes * sign);
			result = result.AddSeconds((double)Seconds * sign);
			return result;
		}

		/// <summary>
		/// Subtracts this duration from the specified <see cref="DateTimeOffset"/> value, taking into account irregular year/month lengths and leap years.
		/// </summary>
		public DateTimeOffset SubtractFrom(DateTimeOffset dateTimeOffset)
		{
			int sign = IsNegative ? 1 : -1;
			var result = dateTimeOffset.AddYears(Years * sign);
			result = result.AddMonths(Months * sign);
			result = result.AddDays(Days * sign);
			result = result.AddHours(Hours * sign);
			result = result.AddMinutes(Minutes * sign);
			result = result.AddSeconds((double)Seconds * sign);
			return result;
		}

		/// <summary>
		/// Adds a <see cref="Duration"/> to a <see cref="DateTime"/>.
		/// </summary>
		public static DateTime operator +(DateTime dateTime, Duration duration) => duration.AddTo(dateTime);

		/// <summary>
		/// Subtracts a <see cref="Duration"/> from a <see cref="DateTime"/>.
		/// </summary>
		public static DateTime operator -(DateTime dateTime, Duration duration) => duration.SubtractFrom(dateTime);

		/// <summary>
		/// Adds a <see cref="Duration"/> to a <see cref="DateTimeOffset"/>.
		/// </summary>
		public static DateTimeOffset operator +(DateTimeOffset dateTimeOffset, Duration duration) => duration.AddTo(dateTimeOffset);

		/// <summary>
		/// Subtracts a <see cref="Duration"/> from a <see cref="DateTimeOffset"/>.
		/// </summary>
		public static DateTimeOffset operator -(DateTimeOffset dateTimeOffset, Duration duration) => duration.SubtractFrom(dateTimeOffset);

		/// <summary>
		/// Returns the XML representation (xs:duration) of this duration.
		/// </summary>
		public override string ToString()
		{
			if (IsZero)
			{
				return "P0D";
			}

			var sb = new StringBuilder();

			if (IsNegative)
			{
				sb.Append('-');
			}

			sb.Append('P');

			if (Years > 0)
			{
				sb.Append(Years).Append('Y');
			}
			if (Months > 0)
			{
				sb.Append(Months).Append('M');
			}
			if (Days > 0)
			{
				sb.Append(Days).Append('D');
			}

			if (Hours > 0 || Minutes > 0 || Seconds > 0m)
			{
				sb.Append('T');

				if (Hours > 0)
				{
					sb.Append(Hours).Append('H');
				}
				if (Minutes > 0)
				{
					sb.Append(Minutes).Append('M');
				}
				if (Seconds > 0m)
				{
					var secStr = Seconds.ToString("0.######", CultureInfo.InvariantCulture);
					sb.Append(secStr).Append('S');
				}
			}

			// Handle zero duration - return "P0D" as a valid minimal duration
			if (sb.Length == 1 || (sb.Length == 2 && sb[0] == '-'))
			{
				sb.Append("0D");
			}

			return sb.ToString();
		}

		/// <summary>
		/// Converts the current object to its XML string representation.
		/// </summary>
		public string ToXml() => ToString();

		/// <summary>
		/// Explicit conversion from Duration to TimeSpan.
		/// </summary>
		public static explicit operator TimeSpan(Duration value) => value.ToTimeSpan();

		/// <summary>
		/// Implicit conversion from TimeSpan to Duration.
		/// </summary>
		public static implicit operator Duration(TimeSpan value) => new Duration(value);

		/// <inheritdoc/>
		public bool Equals(Duration other)
		{
			return IsNegative == other.IsNegative &&
				   Years == other.Years &&
				   Months == other.Months &&
				   Days == other.Days &&
				   Hours == other.Hours &&
				   Minutes == other.Minutes &&
				   Seconds == other.Seconds;
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Duration other && Equals(other);

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = IsNegative.GetHashCode();
				hashCode = (hashCode * 397) ^ Years;
				hashCode = (hashCode * 397) ^ Months;
				hashCode = (hashCode * 397) ^ Days;
				hashCode = (hashCode * 397) ^ Hours;
				hashCode = (hashCode * 397) ^ Minutes;
				hashCode = (hashCode * 397) ^ Seconds.GetHashCode();
				return hashCode;
			}
		}

		/// <inheritdoc/>
		public int CompareTo(Duration other)
		{
			if (Equals(other)) return 0;
			return ToTimeSpan().CompareTo(other.ToTimeSpan());
		}

		/// <inheritdoc/>
		public int CompareTo(object? obj)
		{
			if (obj is null) return 1;
			if (obj is Duration other) return CompareTo(other);
			throw new ArgumentException("Object must be of type Duration", nameof(obj));
		}

		/// <summary>
		/// Determines whether two specified instances of <see cref="Duration"/> are equal.
		/// </summary>
		public static bool operator ==(Duration left, Duration right) => left.Equals(right);

		/// <summary>
		/// Determines whether two specified instances of <see cref="Duration"/> are not equal.
		/// </summary>
		public static bool operator !=(Duration left, Duration right) => !left.Equals(right);

		/// <summary>
		/// Determines whether one specified <see cref="Duration"/> is less than another.
		/// </summary>
		public static bool operator <(Duration left, Duration right) => left.CompareTo(right) < 0;

		/// <summary>
		/// Determines whether one specified <see cref="Duration"/> is greater than another.
		/// </summary>
		public static bool operator >(Duration left, Duration right) => left.CompareTo(right) > 0;

		/// <summary>
		/// Determines whether one specified <see cref="Duration"/> is less than or equal to another.
		/// </summary>
		public static bool operator <=(Duration left, Duration right) => left.CompareTo(right) <= 0;

		/// <summary>
		/// Determines whether one specified <see cref="Duration"/> is greater than or equal to another.
		/// </summary>
		public static bool operator >=(Duration left, Duration right) => left.CompareTo(right) >= 0;
	}
}
