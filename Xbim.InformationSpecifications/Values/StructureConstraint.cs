using System;

namespace Xbim.InformationSpecifications
{
	internal static class NumberLenExtensions
	{
		// https://stackoverflow.com/questions/4483886/how-can-i-get-a-count-of-the-total-number-of-digits-in-a-number

		public static int Digits(this decimal n)
		{
			var mantissa = decimal.GetBits(n);
			if (mantissa[1] == 0 && mantissa[2] == 0)
				return mantissa[0].Digits();
			var dec = new decimal(
				mantissa[0],
				mantissa[1],
				mantissa[2], false, 0
				);
			return dec.ToString().Length;
		}
		
		// does not count the minus sign
		public static int Digits(this int n)
		{
			if (n >= 0)
			{
				if (n < 10) return 1;
				if (n < 100) return 2;
				if (n < 1000) return 3;
				if (n < 10000) return 4;
				if (n < 100000) return 5;
				if (n < 1000000) return 6;
				if (n < 10000000) return 7;
				if (n < 100000000) return 8;
				if (n < 1000000000) return 9;
				return 10;
			}
			else
			{
				if (n > -10) return 1;
				if (n > -100) return 2;
				if (n > -1000) return 3;
				if (n > -10000) return 4;
				if (n > -100000) return 5;
				if (n > -1000000) return 6;
				if (n > -10000000) return 7;
				if (n > -100000000) return 8;
				if (n > -1000000000) return 9;
				return 10;
			}
		}

		public static int Digits(this long n)
		{
			if (n >= 0)
			{
				if (n < 10L) return 1;
				if (n < 100L) return 2;
				if (n < 1000L) return 3;
				if (n < 10000L) return 4;
				if (n < 100000L) return 5;
				if (n < 1000000L) return 6;
				if (n < 10000000L) return 7;
				if (n < 100000000L) return 8;
				if (n < 1000000000L) return 9;
				if (n < 10000000000L) return 10;
				if (n < 100000000000L) return 11;
				if (n < 1000000000000L) return 12;
				if (n < 10000000000000L) return 13;
				if (n < 100000000000000L) return 14;
				if (n < 1000000000000000L) return 15;
				if (n < 10000000000000000L) return 16;
				if (n < 100000000000000000L) return 17;
				if (n < 1000000000000000000L) return 18;
				return 19;
			}
			else
			{
				if (n > -10L) return 1;
				if (n > -100L) return 2;
				if (n > -1000L) return 3;
				if (n > -10000L) return 4;
				if (n > -100000L) return 5;
				if (n > -1000000L) return 6;
				if (n > -10000000L) return 7;
				if (n > -100000000L) return 8;
				if (n > -1000000000L) return 9;
				if (n > -10000000000L) return 10;
				if (n > -100000000000L) return 11;
				if (n > -1000000000000L) return 12;
				if (n > -10000000000000L) return 13;
				if (n > -100000000000000L) return 14;
				if (n > -1000000000000000L) return 15;
				if (n > -10000000000000000L) return 16;
				if (n > -100000000000000000L) return 17;
				if (n > -1000000000000000000L) return 18;
				return 19;
			}
		}
	}

	public class StructureConstraint : IValueConstraint, IEquatable<StructureConstraint>
	{
		public int? TotalDigits { get; set; }
		public int? FractionDigits { get; set; }
		public int? Length { get; set; }
		public int? MinLength { get; set; }
		public int? MaxLength { get; set; }

		public override bool Equals(object obj)
		{
			return Equals(obj as StructureConstraint);
		}

		public override int GetHashCode() => (TotalDigits, FractionDigits, Length, MinLength, MaxLength).GetHashCode();

		public bool Equals(StructureConstraint other)
		{
			if (other == null)
				return false;
			if (!IFacetExtensions.NullEquals(TotalDigits, other.TotalDigits))
				return false;
			if (!IFacetExtensions.NullEquals(FractionDigits, other.FractionDigits))
				return false;
			if (!IFacetExtensions.NullEquals(Length, other.Length))
				return false;
			if (!IFacetExtensions.NullEquals(MinLength, other.MinLength))
				return false;
			if (!IFacetExtensions.NullEquals(MaxLength, other.MaxLength))
				return false;
			return true;
		}

		public bool IsSatisfiedBy(object candiatateValue, ValueConstraint context)
		{
			if (TotalDigits.HasValue)
			{
				if (candiatateValue is float f)
					candiatateValue = Convert.ToDecimal(f);		
				else if (candiatateValue is double d)
					candiatateValue = Convert.ToDecimal(d);
				if (candiatateValue is decimal dec)
				{
					var count = dec.Digits();
					if (count != TotalDigits.Value)
						return false;
				}
				else if (candiatateValue is int i)
				{
					var count = i.Digits();
					if (count != TotalDigits.Value)
						return false;
				}
				else if (candiatateValue is long l)
				{
					var count = l.Digits();
					if (count != TotalDigits.Value)
						return false;
				}
				else
				{
					return false;
				}
			}
			if (FractionDigits.HasValue)
			{
				if (candiatateValue is float f)
					candiatateValue = Convert.ToDecimal(f);
				else if (candiatateValue is double d)
					candiatateValue = Convert.ToDecimal(d);
				if (candiatateValue is decimal dec)
				{
					var exp = decimal.GetBits(dec)[3];
					int count = BitConverter.GetBytes(exp)[2];
					if (count != FractionDigits.Value)
						return false;
				}
				else if (candiatateValue is int || candiatateValue is long)
				{
					if (FractionDigits.Value != 0)
						return false;
				}
				else
					return false;
			}
			if (Length.HasValue || MinLength.HasValue || MaxLength.HasValue)
			{
				var l = candiatateValue.ToString().Length;
				if (Length.HasValue && l != Length.Value)
					return false;
				if (MinLength.HasValue && l < MinLength.Value)
					return false;
				if (MaxLength.HasValue && l > MaxLength.Value)
					return false;
			}
			return true;
		}
		
	}
}
