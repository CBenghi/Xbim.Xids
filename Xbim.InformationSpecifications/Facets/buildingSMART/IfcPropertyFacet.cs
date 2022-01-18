using System;
using System.Text;

namespace Xbim.InformationSpecifications
{

	public partial class IfcPropertyFacet : FacetBase, IFacet, IEquatable<IfcPropertyFacet>
    {
        public string PropertySetName { get; set; }
		public string PropertyName { get; set; }
		public string PropertyValueType { get; set; }
		public ValueConstraint PropertyValue { get; set; } 

        public string Short()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(PropertyName))
                sb.Append($"Has property '{PropertyName}'");
            else
                sb.Append($"Has any property");
            if (!string.IsNullOrEmpty(PropertySetName))
                sb.Append($" in property set '{PropertySetName}'");
            if (PropertyValueType != null)
                sb.Append($" of type '{PropertyValueType}'");
            
            if (PropertyValue != null)
                sb.Append($" {PropertyValue.Short()}");
            
            sb.Append(".");
            return sb.ToString();
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
            return base.Equals(other);
        }

		public bool IsValid()
		{
            // I suppose that at least PropertySetName should be defined.
            if (string.IsNullOrWhiteSpace(PropertySetName))
                return false;
            return true;
		}
	}
}
