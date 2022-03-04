using System;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
	

	public partial class AttributeFacet : LocatedFacet, IEquatable<AttributeFacet>, IFacet
	{
		public ValueConstraint AttributeName { get; set; } = "";

		public ValueConstraint AttributeValue { get; set; } 

		public bool Equals(AttributeFacet other)
		{
			if (other == null)
				return false;
			var thisEqual = (AttributeName, AttributeValue)
				.Equals((other.AttributeName, other.AttributeValue));
			if (!thisEqual)
				return false;
			return base.Equals(other as LocatedFacet);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as AttributeFacet);
		}

		public override string ToString()
		{
			return $"{AttributeName}-{AttributeValue}-{base.ToString()}";
		}

		public override int GetHashCode() => 23 + 31 *(AttributeName, AttributeValue).GetHashCode() + 31 * base.GetHashCode();

		public string Short()
		{
			if (string.IsNullOrEmpty(Location))
				return $"attribute {AttributeName} = {AttributeValue}";
			else
				return $"attribute {AttributeName} @ {Location} = {AttributeValue}";
		}

		public bool IsValid()
		{
			return !FacetBase.IsNullOrEmpty(AttributeName);
		}
	}
}