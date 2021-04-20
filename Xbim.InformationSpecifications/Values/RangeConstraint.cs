using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xbim.InformationSpecifications
{
	public class RangeConstraint : IValueConstraint, IEquatable<RangeConstraint>
	{
		public string MinValue { get; set; }
		public bool MinInclusive { get; set; }
		public string MaxValue { get; set; }
		public bool MaxInclusive { get; set; }

		public bool Equals(RangeConstraint other)
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

		public override bool Equals(object obj)
		{
			return this.Equals(obj as RangeConstraint);
		}

		public override string ToString()
		{
			var minV = MinValue ?? "undefined";
			var min = MinInclusive ? "<=" : "<";
			var maxV = MaxValue ?? "undefined";
			var max = MaxInclusive ? "<=" : "<";
			return $"{minV} {min} .. {max} {maxV}";
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public bool IsSatisfiedBy(object candiatateValue, ValueConstraint context)
		{
			var compe = candiatateValue as IComparable;
			if (compe == null)
				return false;
			var minOk = true;
			var maxOk = true;
			if (!string.IsNullOrEmpty(MinValue))
			{
				var mn = ValueConstraint.GetObject(MinValue, context.BaseType);
				minOk = MinInclusive
					? compe.CompareTo(mn) >= 0
					: compe.CompareTo(mn) > 0;
			}
			if (!string.IsNullOrEmpty(MaxValue))
			{
				var mx = (IComparable)ValueConstraint.GetObject(MaxValue, context.BaseType);
				maxOk = MaxInclusive
					? mx.CompareTo(compe) >= 0
					: mx.CompareTo(compe) > 0;
			}
			return minOk && maxOk;
		}

		public string Short()
		{
			List<string> ret = new List<string>();
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
