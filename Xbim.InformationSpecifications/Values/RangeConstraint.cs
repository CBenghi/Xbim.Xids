using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;

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
        public bool IsSatisfiedBy(object candiatateValue, ValueConstraint context, bool ignoreCase, ILogger? logger = null)
        {
            if (context is null)
                return false;
            if (candiatateValue is not IComparable compe)
            {
                logger?.LogError("Failed to create a comparable value from {candidateValueType} '{candiatateValue}'", candiatateValue.GetType().Name, candiatateValue);
                return false;
            }
            var minOk = true;
            var maxOk = true;
            if (MinValue is not null && !string.IsNullOrEmpty(MinValue))
            {
                var mn = ValueConstraint.GetObject(MinValue, context.BaseType);
                minOk = MinInclusive
                    ? compe.CompareTo(mn) >= 0
                    : compe.CompareTo(mn) > 0;
            }
            if (MaxValue is not null && !string.IsNullOrEmpty(MaxValue))
            {
                var mx = ValueConstraint.GetObject(MaxValue, context.BaseType);
                maxOk = MaxInclusive
                    ? compe.CompareTo(mx) <= 0
                    : compe.CompareTo(mx) < 0;
            }
            return minOk && maxOk;
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
            var min = ValueConstraint.GetObject(MinValue, context.BaseType);
            var max = ValueConstraint.GetObject(MaxValue, context.BaseType);

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
