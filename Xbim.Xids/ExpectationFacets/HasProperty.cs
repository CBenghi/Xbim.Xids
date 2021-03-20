using System;
using System.Diagnostics;

namespace Xbim.Xids
{
    public partial class HasProperty : ExpectationFacet, IEquatable<HasProperty>
	{
		public string PropertySetName { get; set; } = "";

		public string PropertyName { get; set; } = "";

		public string PropertyType { get; set; } = "";

		public IValueConstraint PropertyConstraint { get; set; } = null;

		public bool Equals(HasProperty other)
		{
			if (other == null)
				return false;
			if (PropertySetName.ToLowerInvariant() != other.PropertySetName.ToLowerInvariant())
				return false;
			if (PropertyName.ToLowerInvariant() != other.PropertyName.ToLowerInvariant())
				return false;
			if (PropertyType != other.PropertyType)
				return false;
			if (PropertyConstraint != null && !PropertyConstraint.Equals(other.PropertyConstraint))
				return false;
			else if (PropertyConstraint == null && other.PropertyConstraint != null)
				return false;
			return base.Equals(other as ExpectationFacet);
		}

		public override string Short()
		{
			return ToString();
		}
		public override bool Equals(object obj)
		{
			return this.Equals(obj as HasProperty);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override bool Validate()
		{
			// Strictly speaking we only need property name
			if (string.IsNullOrWhiteSpace(PropertyName))
				return false;
			//if (Guid == Guid.Empty)
			//	Guid = Guid.NewGuid();
			return true;
		}

		public override string ToString()
		{
			return $"{PropertySetName}-{PropertyName}-{PropertyType}-{PropertyConstraint}";
		}

	}
}