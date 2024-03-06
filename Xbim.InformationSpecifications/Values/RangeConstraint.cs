using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
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
                if (MinInclusive) minimum = ApplyRealTolerances(minimum, false);
                minOk = MinInclusive
                    ? valueToCompare.CompareTo(minimum) >= 0
                    : valueToCompare.CompareTo(minimum) > 0;
            }
            if (MaxValue is not null && !string.IsNullOrEmpty(MaxValue))
            {
                var maximum = ValueConstraint.ParseValue(MaxValue, context.BaseType);
                if (MaxInclusive) maximum = ApplyRealTolerances(maximum, true);
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
        /// <param name="isMax">Indicates if the expected value is a maxima, or minima</param>
        /// <param name="tolerance">The floating point tolerance. Defaults to 1e-06</param>
        /// <returns>The value with tolerance applied</returns>
        private object? ApplyRealTolerances(object? expectedValue, bool isMax, double tolerance = RealHelper.DefaultRealPrecision)
        {
            // To support 1e-6 tolerance on Reals we increase/decrease the magnitude of the appropriate end of the range,
            // depending on whether it's upper (Max) or lower (Min).
            // This follows the same pattern used in IDS to account for magnitude of the value. https://github.com/buildingSMART/IDS/issues/78#issuecomment-1976197561
            // But for ranges also need to reverse the logic when the value is -ve
            //  [123.45   <= ---- => 678.90]    For +ve range values
            // [ 123.449  <= ---- => 678.901]   Min decreases in magnitude, while Max increases
            // But for negative we reverse that
            //  [-678.90  <= ---- => -123.45]  For -ve range values
            // [ -678.901 <= ---- => -123.449] Min increases in magnitude, while Max decreases

            var applyFactor = (double value) =>
            {
                var increaseFactor = isMax ? (1 + tolerance) : (1 - tolerance);
                var decreaseFactor = isMax ? (1 - tolerance) : (1 + tolerance);
                var fixedFactor = (isMax ? +tolerance : -tolerance);
                if ((value >= 0))
                {
                    return (value * increaseFactor) + fixedFactor;
                }
                else
                {
                    return (value * decreaseFactor) + fixedFactor;
                }
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

            // values that exist must be comparable
            if (min is not IComparable minCmp)
                return false;
            if (max is not IComparable maxCmp)
                return false;

            if (min is not null && max is not null)
            {
                // if both values are available they need to be meaningful (max > min)
                if (!MinInclusive && !MaxInclusive)
                {
                    return maxCmp.CompareTo(minCmp) > 0;
                }
                return maxCmp.CompareTo(minCmp) >= 0;
            }

            return true;
        }
    }
}
