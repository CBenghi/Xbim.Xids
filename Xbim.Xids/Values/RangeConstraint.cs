using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Xids
{
	public class RangeConstraint : IValueConstraint, IEquatable<RangeConstraint>
	{
		public IComparable MinValue { get; set; }
		public bool MinInclusive { get; set; }

		public IComparable MaxValue { get; set; }
		public bool MaxInclusive { get; set; }

		public bool Equals(RangeConstraint other)
		{
			if (other == null)
				return false;
			if (MinValue != null)
			{
				if (!MinValue.Equals(other.MinValue))
					return false;
			}
			else if (other.MinValue == null)
					return false;

			if (MaxValue != null)
			{
				if (!MaxValue.Equals(other.MaxValue))
					return false;
			}
			else if (other.MaxValue == null)
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

		public bool IsSatisfiedBy(object testObject)
		{
			var compe = testObject as IComparable;
			if (compe == null)
				return false;
			return true;
		}
	}
}
