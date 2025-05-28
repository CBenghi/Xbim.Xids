using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// A constraint component based on a range, identified by minimum and maximum values.
	/// </summary>
	public class RangeConstraint : IValueConstraintComponent, IEquatable<RangeConstraint>
	{
		/// <summary>
		/// Default empty contructor
		/// </summary>
		public RangeConstraint()
		{

		}

		/// <summary>
		/// Fully specified constructor
		/// </summary>
		/// <param name="minValue">The optional minimum value</param>
		/// <param name="minInclusive">Is the minimum value included in the range</param>
		/// <param name="maxValue">The optional maximum value</param>
		/// <param name="maxInclusive">Is the maximum value included in the range</param>
		public RangeConstraint(string? minValue, bool minInclusive, string? maxValue, bool maxInclusive)
		{
			MinValue = minValue;
			MinInclusive = minInclusive;
			MaxValue = maxValue;
			MaxInclusive = maxInclusive;
		}

		/// <summary>
		/// String representation of the minimum value
		/// </summary>
		public string? MinValue { get; set; }
		/// <summary>
		/// boolean option, is the minimum value inclusive?
		/// </summary>
		public bool MinInclusive { get; set; }
		/// String representation of the maximum value
		public string? MaxValue { get; set; }
		/// <summary>
		/// boolean option, is the maximum value inclusive?
		/// </summary>
		public bool MaxInclusive { get; set; }

		/// <inheritdoc />
		public bool Equals(RangeConstraint? other)
		{
			if (other == null)
				return false;
			if (!IFacetExtensions.NullEquals(MinValue, other.MinValue))
				return false;
			if (!IFacetExtensions.NullEquals(MaxValue, other.MaxValue))
				return false;
			return MinInclusive == other.MinInclusive &&
				MaxInclusive == other.MaxInclusive;
		}

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			return Equals(obj as RangeConstraint);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			var minV = MinValue ?? "undefined";
			var min = MinInclusive ? "<=" : "<";
			var maxV = MaxValue ?? "undefined";
			var max = MaxInclusive ? "<=" : "<";
			return $"Range: {minV} {min} .. {max} {maxV}";
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		/// <inheritdoc />
		public bool IsSatisfiedBy(object candidateValue, ValueConstraint context, bool ignoreCase, ILogger? logger = null)
		{
			if (context is null)
				return false;
			if (candidateValue is not IComparable valueToCompare)
			{
				logger?.LogError("Failed to create a comparable value from {candidateValueType} '{candidateValue}'", candidateValue.GetType().Name, candidateValue);
				return false;
			}
			var minOk = true;
			var maxOk = true;

			if (MinValue is not null && !string.IsNullOrEmpty(MinValue))
			{
				var minimum = ValueConstraint.ParseValue(MinValue, context.BaseType);
				// Range tolerances removed as part of https://github.com/buildingSMART/IDS/pull/318

				//var rangeType = MinInclusive ? RangeType.Inclusive : RangeType.Exclusive;
				//minimum = ApplyRealTolerances(minimum, rangeType, false);
				minOk = MinInclusive
					? valueToCompare.CompareTo(minimum) >= 0
					: valueToCompare.CompareTo(minimum) > 0;
			}
			if (MaxValue is not null && !string.IsNullOrEmpty(MaxValue))
			{
				var maximum = ValueConstraint.ParseValue(MaxValue, context.BaseType);
				//var rangeType = MaxInclusive ? RangeType.Inclusive : RangeType.Exclusive;
				//maximum = ApplyRealTolerances(maximum, rangeType, true);
				maxOk = MaxInclusive
					? valueToCompare.CompareTo(maximum) <= 0
					: valueToCompare.CompareTo(maximum) < 0;
			}
			return minOk && maxOk;
		}

		/// <summary>
		/// Applies a tolerance factor to Real constraint values to support small floating point imprecisions
		/// </summary>
		/// <param name="expectedValue"></param>
		/// <param name="rangeType">The type of range</param>
		/// <param name="isMax">Indicates if the expected value is a maxima, or minima</param>
		/// <param name="tolerance">The floating point tolerance. Defaults to 1e-06</param>
		/// <returns>The value with tolerance applied</returns>
		private object? ApplyRealTolerances(object? expectedValue, RangeType rangeType, bool isMax, double tolerance = RealHelper.DefaultRealPrecision)
		{
			// To support 1e-6 tolerance on Reals we increase/decrease the magnitude of the appropriate end of the range,
			// depending on the end of the range and whether it's an inclusive or exclusive range.
			// For inclusive ranges we expand the overall range slightly, while for exclusive ranges we shrink the raange

			// This follows the same pattern used in IDS to account for magnitude of the value. https://github.com/buildingSMART/IDS/issues/78#issuecomment-1976197561
			// Inclusive ranges:

			// For Inclusive ranges we expand the range at each end
			//         [0   <= ---- => 100]
			// [ -0.000001  <= ---- => 100.0000101]
			// While for Exclusive ranges we shrink the ends
			//  [        0 <= ---- => 273        ] 
			//   [0.000001 <= ---- => 273.000019] 

			var applyFactor = (double value) =>
			{
				// The high/low bounds for the given value and tolerance. e.g. 49.999 < 50 < 50.001
				// This already accounts for -ve values
				var (low, high) = RealHelper.GetPrecisionBounds(value, tolerance);
				// based on the range type and 'end' we adjust to the appropriate value
				return rangeType switch
				{
					RangeType.Inclusive => isMax ? high : low,  // Expand range for Inclusive. i.e. 41.99999 is Between 42 and 100 (inclusive) while 41.995 is not
					RangeType.Exclusive => isMax ? low : high,  // Shrink range for Exclusive i.e 0.1 is between 0 and 100 (exclusive) while 0.000001 is not 
					_ => isMax ? low : high,
				};
			};
			return expectedValue switch
			{
				float f => Convert.ToSingle(applyFactor(f)),
				double d => applyFactor(d),
				_ => expectedValue  // Unchanged
			}; ;
		}

		/// <inheritdoc />
		public string Short()
		{
			var ret = new List<string>();
			if (!string.IsNullOrEmpty(MinValue))
				ret.Add(MinInclusive
					? $">={MinValue}"
					: $">{MinValue}");
			if (!string.IsNullOrEmpty(MaxValue))
				ret.Add(MaxInclusive
						? $"<={MaxValue}"
						: $"<{MaxValue}");

			return string.Join(" and ", ret.ToArray());
		}


		/// <inheritdoc />
		public bool IsValid(ValueConstraint context)
		{
			var min = ValueConstraint.ParseValue(MinValue, context.BaseType);
			var max = ValueConstraint.ParseValue(MaxValue, context.BaseType);

			// values need to be succesfully converted
			if (min is null && !string.IsNullOrEmpty(MinValue))
				return false;
			if (max is null && !string.IsNullOrEmpty(MaxValue))
				return false;
			if (min is null && max is null)
				return false;

			var minCmp = min as IComparable;
			var maxCmp = max as IComparable;
			// values that exist must be comparable
			if (min is not null && minCmp is null)
				return false;
			if (max is not null && maxCmp is null)
				return false;

			if (minCmp is not null && maxCmp is not null)
			{
				// if both values are available they need to be meaningful (max > min)
				var cmp = maxCmp.CompareTo(minCmp);
				if (!MinInclusive && !MaxInclusive)
				{
					return cmp > 0;
				}
				return cmp >= 0;
			}
			return true;
		}
		private enum RangeType
		{
			Inclusive,
			Exclusive
		}
	}

}
