using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

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
	/// <summary>
	/// A constraint component based on a structure of the presentation 
	/// </summary>
	public class StructureConstraint : IValueConstraintComponent, IEquatable<StructureConstraint>
	{
		/// <summary>
		/// Optional: Total digits of the presentation
		/// </summary>
		public int? TotalDigits { get; set; }
		/// <summary>
		/// Optional: fractional digits of the presentation
		/// </summary>
		public int? FractionDigits { get; set; }
		/// <summary>
		/// Optional: Total characters of the presentation
		/// </summary>
		public int? Length { get; set; }
		/// <summary>
		/// Optional: minimum characters of the presentation
		/// </summary>
		public int? MinLength { get; set; }
		/// <summary>
		/// Optional: maximum characters of the presentation
		/// </summary>
		public int? MaxLength { get; set; }

		/// <inheritdoc />
		public override string ToString()
		{
			if (
				!TotalDigits.HasValue
				&& FractionDigits.HasValue
				&& Length.HasValue
				&& MinLength.HasValue
				&& MaxLength.HasValue
				)
				return "Structure: <empty>";
			var sb = new StringBuilder();
			sb.Append("Structure:");
			if (TotalDigits.HasValue)
				sb.Append($" digits: {TotalDigits.Value}");
			if (FractionDigits.HasValue)
				sb.Append($" decimals: {FractionDigits.Value}");
			if (Length.HasValue)
				sb.Append($" length: {Length.Value}");
			if (MinLength.HasValue)
				sb.Append($" minlength: {MinLength.Value}");
			if (MaxLength.HasValue)
				sb.Append($" maxlength: {MaxLength.Value}");
			return sb.ToString();
		}

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			return Equals(obj as StructureConstraint);
		}

		/// <inheritdoc />
		public override int GetHashCode() => (TotalDigits, FractionDigits, Length, MinLength, MaxLength).GetHashCode();

		/// <inheritdoc />
		public bool Equals(StructureConstraint? other)
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

		/// <inheritdoc />
		public bool IsSatisfiedBy(object? candidateValue, ValueConstraint context, bool ignoreCase, ILogger? logger = null)
		{
			if (TotalDigits.HasValue)
			{
				if (candidateValue is null)
					return false;
				// first of all, if candidateValue is float or double we convert it to decimal to count the digits.
				switch (candidateValue)
				{
					case float f:
						// todo: should there be a warning for conversion here?
						candidateValue = Convert.ToDecimal(f);
						break;
					case double d:
						// todo: should there be a warning for conversion here?
						candidateValue = Convert.ToDecimal(d);
						break;
				}
				switch (candidateValue)
				{
					case decimal dec:
						{
							var count = dec.Digits();
							if (count != TotalDigits.Value)
								return false;
							break;
						}
					case int i:
						{
							var count = i.Digits();
							if (count != TotalDigits.Value)
								return false;
							break;
						}
					case long l:
						{
							var count = l.Digits();
							if (count != TotalDigits.Value)
								return false;
							break;
						}
					default:
						logger?.LogError("TotalDigits check is not implemented for type '{}'", candidateValue.GetType().Name);
						return false;
				}
			}
			if (FractionDigits.HasValue)
			{
				if (candidateValue is null)
					return false;
				// first of all, if candidateValue is float or double we convert it to decimal to count the digits.
				switch (candidateValue)
				{
					case float f:
						candidateValue = Convert.ToDecimal(f);
						break;
					case double d:
						candidateValue = Convert.ToDecimal(d);
						break;
				}
				switch (candidateValue)
				{
					case decimal dec:
						{
							var exp = decimal.GetBits(dec)[3];
							int count = BitConverter.GetBytes(exp)[2];
							if (count != FractionDigits.Value)
								return false;
							break;
						}
					case int:
					case long:
						if (FractionDigits.Value != 0)
							return false;
						break;
					default:
						logger?.LogError("TotalDigits check is not implemented for type '{}'", candidateValue.GetType().Name);
						return false;
				}
			}
			if (Length.HasValue || MinLength.HasValue || MaxLength.HasValue)
			{
				if (candidateValue is null)
					return false;
				var eval = candidateValue.ToString() ?? string.Empty;
				var l = eval.Length;
				if (Length.HasValue && l != Length.Value)
					return false;
				if (MinLength.HasValue && l < MinLength.Value)
					return false;
				if (MaxLength.HasValue && l > MaxLength.Value)
					return false;
			}
			return true;
		}

		/// <inheritdoc />
		public string Short()
		{
			var ret = new List<string>();
			if (TotalDigits.HasValue)
				ret.Add($"{TotalDigits.Value} digits in total");
			if (FractionDigits.HasValue)
				ret.Add($"{FractionDigits.Value} decimal digits");
			if (Length.HasValue)
				ret.Add($"{Length.Value} characters");
			if (MinLength.HasValue)
				ret.Add($"minimum {MinLength.Value} characters");
			if (MaxLength.HasValue)
				ret.Add($"maximum {MaxLength.Value} characters");
			return string.Join(" and ", ret.ToArray());
		}

		/// <inheritdoc />
		public bool IsValid(ValueConstraint context)
		{
			if (MinLength.HasValue && MaxLength.HasValue // min && max available
				&& !(MaxLength.Value >= MinLength.Value)) // invalid case
			{
				return false;
			}
			if (Length.HasValue && MaxLength.HasValue // len && max available
				&& !(MaxLength.Value == Length.Value)) // invalid case
			{
				return false;
			}
			if (Length.HasValue && MinLength.HasValue // len && min available
				&& !(MinLength.Value == Length.Value)) // invalid case
			{
				return false;
			}
			if (TotalDigits.HasValue && FractionDigits.HasValue // tot && fraction available
				&& !(TotalDigits.Value >= FractionDigits.Value)) // invalid case
			{
				return false;
			}
			return true;
		}
	}
}
