using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xbim.InformationSpecifications
{
	public class RangeConstraint : IValueConstraint, IEquatable<RangeConstraint>
	{
		public string? MinValue { get; set; }
		public bool MinInclusive { get; set; }
		public string? MaxValue { get; set; }
		public bool MaxInclusive { get; set; }

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

		public override bool Equals(object? obj)
		{
			return this.Equals(obj as RangeConstraint);
		}

		public override string ToString()
		{
			var minV = MinValue ?? "undefined";
			var min = MinInclusive ? "<=" : "<";
			var maxV = MaxValue ?? "undefined";
			var max = MaxInclusive ? "<=" : "<";
			return $"Range: {minV} {min} .. {max} {maxV}";
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public bool IsSatisfiedBy(object candiatateValue, ValueConstraint context, bool ignoreCase, ILogger? logger = null)
        {
			if (context == null)
				return false;
            if (candiatateValue is not IComparable compe)
            {
                logger?.LogError("Failed to create a comparable value from {0} '{1}'", candiatateValue.GetType().Name, candiatateValue);
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

		public string Short()
		{
			var ret = new List<string>();
			if (!string.IsNullOrEmpty(MinValue))
				ret.Add(MinInclusive
					? $"is equal or more than '{MinValue}'"
					: $"is greater than '{MinValue}'");
			if (!string.IsNullOrEmpty(MaxValue))
				ret.Add(MaxInclusive
						? $"is equal or less than '{MaxValue}'"
						: $"is less than '{MaxValue}'");

			return string.Join(" and ", ret.ToArray());
		}
	}
}
