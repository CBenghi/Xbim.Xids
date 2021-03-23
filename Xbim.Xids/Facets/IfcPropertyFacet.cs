using System;

namespace Xbim.Xids
{

	public partial class IfcPropertyFacet : LocationBase, IFacet, IEquatable<IfcPropertyFacet>
    {
        public string PropertySetName { get; set; }
		public string PropertyName { get; set; }
		
        public IValueConstraint PropertyValue { get; set; } 

        public string Short()
        {
            return ToString();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as IfcPropertyFacet);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

		public override string ToString()
		{
            return $"{PropertySetName}-{PropertyName}-{PropertyValue?.ToString()??""}-{base.ToString()}";
        }

		public bool Equals(IfcPropertyFacet other)
        {
            if (other == null)
                return false;

            if (!IFacetExtensions.NullableStringCaseInsensitiveEquals(PropertySetName, other.PropertySetName))
                return false;
            if (!IFacetExtensions.NullableStringCaseInsensitiveEquals(PropertyName, other.PropertyName))
                return false;
            if (!IFacetExtensions.NullEquals(PropertyValue, other.PropertyValue))
                return false;
            return ((LocationBase)this).Equals((LocationBase)other);
        }
    }
}
