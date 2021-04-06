using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Xids
{
	public class RangeConstraint : IValueConstraint, IEquatable<RangeConstraint>
	{
		public IComparable MinValue { get; set; }
		public bool MinInclusive { get; set; }

		private IComparable actualMinValue = null;

		private IComparable ActualMinValue
		{
			get
			{
				if (actualMinValue == null)
				{
					actualMinValue = GetCompareValue(MinValue);
				}
				return actualMinValue;
			}
		}

		private IComparable actualMaxValue = null;

		private IComparable ActualMaxValue
		{
			get
			{
				if (actualMaxValue == null)
				{
					actualMaxValue = GetCompareValue(MaxValue);
				}
				return actualMaxValue;
			}
		}

		private IComparable GetCompareValue(IComparable valueIn)
		{
			if (valueIn == null)
				return null;
			Debug.WriteLine(valueIn.GetType().ToString());
			switch (valueIn.GetType().ToString())
			{
				case "System.Int32":
				case "System.Single":
				case "System.Decimal":
					return Convert.ToDouble(valueIn);
				default:
					return valueIn;
			}
		}

		public IComparable MaxValue { get; set; }
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

		public bool IsSatisfiedBy(object testObject)
		{
			var compe = testObject as IComparable;
			if (compe == null)
				return false;
			compe = GetCompareValue(compe);
			//var minOk = MinValue == null
			//	? true 
			var minOk = MinInclusive
				? compe.CompareTo(ActualMinValue) >= 0
				: compe.CompareTo(ActualMinValue) > 0;
			var maxOk = MaxInclusive
				? ActualMaxValue.CompareTo(compe) >= 0
				: ActualMaxValue.CompareTo(compe) > 0;
			return minOk && maxOk;
		}
	}
}
