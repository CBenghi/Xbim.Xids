using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.Xids
{
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

		public bool IsSatisfiedBy(object testObject)
		{
			return false;
		}
	}
}
