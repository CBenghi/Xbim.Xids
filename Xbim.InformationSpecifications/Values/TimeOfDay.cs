using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Xbim.InformationSpecifications.Values
{
	/// <summary>
	/// Convenience class to map to the XML xs:time type.
	/// It represents a time of day with an optional time zone offset.
	/// 
	/// For persistence, use ToString() and TryParseXml().
	/// </summary>
	public readonly struct TimeOfDay : IEquatable<TimeOfDay>, IComparable<TimeOfDay>
	{
		private readonly TimeSpan _value;
		private readonly TimeSpan? _offset;

		/// <summary>
		/// Initializes a new instance of the TimeOfDay class with the specified hour, minute, second, and optional time
		/// offset.
		/// </summary>
		/// <param name="hour">The hour component of the time. Must be in the range 0 through 23.</param>
		/// <param name="minute">The minute component of the time. Must be in the range 0 through 59.</param>
		/// <param name="second">The second component of the time. Must be in the range 0 through 59. The default is 0.</param>
		/// <param name="offset">An optional time offset from Coordinated Universal Time (UTC). If null, the time is considered unspecified with
		/// respect to time zones.</param>
		public TimeOfDay(int hour, int minute, int second = 0, TimeSpan? offset = null)
			: this(new TimeSpan(hour, minute, second), offset) { }

		/// <summary>
		/// Initializes a new instance of the TimeOfDay class with the specified time and optional offset.
		/// </summary>
		/// <param name="value">The time of day represented as a TimeSpan. Must be greater than or equal to TimeSpan.Zero and less than 24 hours.</param>
		/// <param name="offset">An optional offset from Coordinated Universal Time (UTC), represented as a TimeSpan. If specified, must be between
		/// -14 and +14 hours, inclusive.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if value is less than TimeSpan.Zero or greater than or equal to 24 hours, or if offset is specified and is
		/// less than -14 hours or greater than 14 hours.</exception>
		public TimeOfDay(TimeSpan value, TimeSpan? offset = null)
		{
			if (value < TimeSpan.Zero || value >= TimeSpan.FromDays(1))
				throw new ArgumentOutOfRangeException(nameof(value));

			if (offset.HasValue &&
				(offset.Value < TimeSpan.FromHours(-14) || offset.Value > TimeSpan.FromHours(14)))
				throw new ArgumentOutOfRangeException(nameof(offset));

			_value = value;
			_offset = offset;
		}

		/// <summary>
		/// a constructor that parses a string in XML xs:time format, which may include an optional time zone offset.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the xmlValue is not a valid XML xs:time format.</exception>
		public TimeOfDay(string xmlValue)
		{
			if (TryParseXml(xmlValue, out var valueOk))
			{
				_value = valueOk._value;
				_offset = valueOk._offset;
			}
			else
				throw new ArgumentOutOfRangeException(nameof(xmlValue));
		}

		/// <summary>
		/// Gets the hour component of the represented time interval.
		/// </summary>
		public int Hour => _value.Hours;

		/// <summary>
		/// Gets the minute component of the represented time.
		/// </summary>
		public int Minute => _value.Minutes;
		/// <summary>
		/// Gets the seconds component of the represented time interval.
		/// </summary>
		public int Second => _value.Seconds;

		/// <summary>
		/// Gets the time of day component represented by this instance, ignoring the offset.
		/// For a computed timespan use the explicit conversion available.
		/// </summary>
		public TimeSpan TimeOfDayValue => _value;
		/// <summary>
		/// Gets the time offset from Coordinated Universal Time (UTC), if available.
		/// </summary>
		public TimeSpan? Offset => _offset;
		/// <summary>
		/// Gets a value indicating whether an offset value is present.
		/// </summary>
		public bool HasOffset => _offset.HasValue;

		/// <inheritdoc />
		public bool Equals(TimeOfDay other) =>
			_value == other._value && _offset == other._offset;

		/// <inheritdoc />
		public override bool Equals(object? obj) => obj is TimeOfDay other && Equals(other);

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked
			{
				return (_value.GetHashCode() * 397) ^ _offset.GetHashCode();
			}
		}

		/// <summary>
		/// Determines whether the current instance and the specified <see cref="TimeOfDay"/> represent the same instant in
		/// time, accounting for their respective offsets if present.
		/// </summary>
		/// <param name="other">The <see cref="TimeOfDay"/> instance to compare with the current instance.</param>
		/// <returns>TRUE if both instances represent the same instant in time; otherwise, FALSE.</returns>
		public bool EqualsInstant(TimeOfDay other)
		{
			if (_offset.HasValue && other._offset.HasValue)
			{
				return (_value - _offset.Value) == (other._value - other._offset.Value);
			}
			return _value == other._value && _offset == other._offset;
		}

		/// <inheritdoc />
		public int CompareTo(TimeOfDay other)
		{
			if (_offset.HasValue && other._offset.HasValue)
			{
				var thisUtc = _value - _offset.Value;
				var otherUtc = other._value - other._offset.Value;
				return thisUtc.CompareTo(otherUtc);
			}
			return _value.CompareTo(other._value);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			// todo: microseconds is not defined in netstandard2.0 - for the time being, we will only support milliseconds
			var time = (_value.Milliseconds > 0) ? _value.ToString(@"hh\:mm\:ss\.fff") : _value.ToString(@"hh\:mm\:ss");
			if (!_offset.HasValue) return time;
			if (_offset.Value == TimeSpan.Zero) return time + "Z";
			var sign = _offset.Value < TimeSpan.Zero ? "-" : "+";
			return $"{time}{sign}{_offset.Value:hh\\:mm}";
		}

		/// <summary>
		/// Determines whether two specified TimeOfDay instances represent the same time of day retaining their structure.
		/// </summary>
		/// <returns>TRUE if the two TimeOfDay instances are equal both for time and time offset; otherwise, FALSE.</returns>
		public static bool operator == (TimeOfDay left, TimeOfDay right) => left.Equals(right);

		/// <summary>
		/// Determines whether two specified TimeOfDay instances represent different time of day retaining their structure.
		/// </summary>
		/// <returns>TRUE if the two TimeOfDay instances are NOT EQUAL both for time and time offset; otherwise, FALSE.</returns>
		public static bool operator != (TimeOfDay left, TimeOfDay right) => !left.Equals(right);

		/// <summary>
		/// explicit conversion that discards information structure: loses the offset
		/// </summary>
		public static explicit operator TimeSpan(TimeOfDay value) => value._value + (value._offset ?? TimeSpan.Zero);

		/// <summary>
		/// explicit conversion that adds a phantom date
		/// </summary>
		public static explicit operator DateTime(TimeOfDay value) => DateTime.MinValue + value._value + (value._offset ?? TimeSpan.Zero);

		/// <summary>
		/// Converts the current object to its XML string representation.
		/// </summary>
		public string ToXml() => ToString();

		/// <summary>
		/// Attempts to parse a string in XML xs:time format into a TimeOfDay value.
		/// </summary>
		/// <param name="stringSource">The string representation of the time to parse, in XML xs:time format. May include an optional time zone offset.</param>
		/// <param name="result">When this method returns, contains the parsed TimeOfDay value if parsing succeeded; otherwise, the default value.</param>
		/// <returns>true if the string was successfully parsed; otherwise, false.</returns>
		public static bool TryParseXml(string stringSource, out TimeOfDay result)
		{
			result = default;
			if (string.IsNullOrEmpty(stringSource))
				return false;

			// Split off the offset portion, if any.
			string timePart;
			TimeSpan? offset;

			if (stringSource.EndsWith("Z", StringComparison.Ordinal))
			{
				timePart = stringSource.Substring(0, stringSource.Length - 1);
				offset = TimeSpan.Zero;
			}
			else
			{
				// Find a '+' or a '-' that appears after the time portion.
				// The time part always starts with two digits and a colon, so
				// we only look for the sign from index 8 onwards (hh:mm:ss).
				int signIndex = -1;
				for (int i = 8; i < stringSource.Length; i++)
				{
					if (stringSource[i] == '+' || stringSource[i] == '-')
					{
						signIndex = i;
						break;
					}
				}

				if (signIndex >= 0)
				{
					timePart = stringSource.Substring(0, signIndex);
					var offsetPart = stringSource.Substring(signIndex);
					if (!TimeSpan.TryParseExact(
							offsetPart.TrimStart('+', '-'),
							@"hh\:mm",
							CultureInfo.InvariantCulture,
							out var parsedOffset))
					{
						return false;
					}

					offset = offsetPart[0] == '-' ? -parsedOffset : parsedOffset;
				}
				else
				{
					timePart = stringSource;
					offset = null;
				}
			}

			// Parse the time portion. xs:time allows optional fractional seconds.
			string[] formats =
			{
				@"hh\:mm\:ss",
				@"hh\:mm\:ss\.f",
				@"hh\:mm\:ss\.ff",
				@"hh\:mm\:ss\.fff",
				@"hh\:mm\:ss\.ffff",
				@"hh\:mm\:ss\.fffff",
				@"hh\:mm\:ss\.ffffff",
				@"hh\:mm\:ss\.fffffff",
			};

			if (!TimeSpan.TryParseExact(timePart, formats, CultureInfo.InvariantCulture, out var time))
				return false;

			// Reject 24:00:00 and beyond — xs:time allows 24:00:00 as midnight,
			// but our struct's invariant is [00:00:00, 24:00:00). Decide per your needs.
			if (time < TimeSpan.Zero || time >= TimeSpan.FromDays(1))
				return false;

			if (offset.HasValue &&
				(offset.Value < TimeSpan.FromHours(-14) || offset.Value > TimeSpan.FromHours(14)))
			{
				return false;
			}

			result = new TimeOfDay(time, offset);
			return true;
		}
	}
}
