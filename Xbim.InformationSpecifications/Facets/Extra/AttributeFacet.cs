using System;

namespace Xbim.InformationSpecifications
{
	public partial class AttributeFacet : IFacet, IEquatable<AttributeFacet>
	{
		public string AttributeName { get; set; } = "";

		public ValueConstraint AttributeValue { get; set; } 

		public bool Equals(AttributeFacet other)
		{
			if (other == null)
				return false;
			var thisEqual = (AttributeName, AttributeValue)
				.Equals((other.AttributeName, other.AttributeValue));
			return thisEqual;
		}
		public override bool Equals(object obj)
		{
			return this.Equals(obj as AttributeFacet);
		}

		public override string ToString()
		{
			return $"{AttributeName}-{AttributeValue}";
		}

		public override int GetHashCode() => (AttributeName, AttributeValue).GetHashCode();

		public string Short()
		{
			return ToString();
		}

		public bool IsValid()
		{
			return !string.IsNullOrWhiteSpace(AttributeName);
		}
	}
}