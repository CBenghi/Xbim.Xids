using System;
namespace Xbim.Xids
{
    public partial class HasMaterial : ExpectationFacet, IEquatable<HasMaterial>
	{
		public string MaterialName { get; set; }

		public override string Short()
		{
			return ToString();
		}
		public bool Equals(HasMaterial other)
		{
			if (other == null)
				return false;
			if (MaterialName.ToLowerInvariant() != other.MaterialName.ToLowerInvariant())
				return false;
			return base.Equals(other as ExpectationFacet);
		}
		public override bool Equals(object obj)
		{
			return this.Equals(obj as HasMaterial);
		}

		public override int GetHashCode()
		{
			return $"{MaterialName}".GetHashCode();
		}

		public override bool Validate()
		{
			// Strictly speaking we only need property name
			if (string.IsNullOrWhiteSpace(MaterialName))
				return false;
			//if (Guid == Guid.Empty)
			//	Guid = Guid.NewGuid();
			return true;
		}
	}
}