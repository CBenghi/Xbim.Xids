using System;
namespace Xbim.Xids
{
    public partial class HasQuantity : ExpectationFacet, IEquatable<HasQuantity>
	{
		public override string Short()
		{
			return ToString();
		}
		public string PropertySetName { get; set; } = "";

		public string QuantityName { get; set; } = "";

		public string QuantityType { get; set; } = "";

		public bool Equals(HasQuantity other)
		{
			if (other == null)
				return false;
			if (PropertySetName.ToLowerInvariant() != other.PropertySetName.ToLowerInvariant())
				return false;
			if (QuantityName.ToLowerInvariant() != other.QuantityName.ToLowerInvariant())
				return false;
			if (QuantityType.ToLowerInvariant() != other.QuantityType.ToLowerInvariant())
				return false;
			return base.Equals(other as ExpectationFacet);
		}
		public override bool Equals(object obj)
		{
			return this.Equals(obj as HasQuantity);
		}

		public override int GetHashCode()
		{
			return $"{PropertySetName}-{QuantityName}-{QuantityType}".GetHashCode();
		}
		public override bool Validate()
		{
			// Strictly speaking we only need QuantityName
			if (string.IsNullOrWhiteSpace(QuantityName))
				return false;
			//if (Guid == Guid.Empty)
			//	Guid = Guid.NewGuid();
			return true;
		}
	}
}
