using System;
using System.Text;

namespace Xbim.InformationSpecifications
{

	public partial class IfcPropertyFacet : FacetBase, IFacet, IEquatable<IfcPropertyFacet>
    {
        public ValueConstraint PropertySetName { get; set; }
		public ValueConstraint PropertyName { get; set; }
		public string Measure { get; set; }
		public ValueConstraint PropertyValue { get; set; } 

        public string Short()
        {
            StringBuilder sb = new StringBuilder();
            if (!FacetBase.IsNullOrEmpty(PropertyName))
                sb.Append($"Has property '{PropertyName}'");
            else
                sb.Append($"Has any property");
            if (!FacetBase.IsNullOrEmpty(PropertySetName))
                sb.Append($" in property set '{PropertySetName}'");
            if (Measure != null)
                sb.Append($" containing '{Measure}'");
            
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
            if (!IFacetExtensions.CaseInsensitiveEquals(PropertySetName, other.PropertySetName))
                return false;
            if (!IFacetExtensions.CaseInsensitiveEquals(PropertyName, other.PropertyName))
                return false;
            if (!IFacetExtensions.CaseInsensitiveEquals(Measure, other.Measure))
                return false;
            if (!IFacetExtensions.NullEquals(PropertyValue, other.PropertyValue))
                return false;
            return base.Equals(other);
        }

		public bool IsValid()
		{
            if (FacetBase.IsNullOrEmpty(PropertySetName))
                return false;
            if (FacetBase.IsNullOrEmpty(PropertyName))
                return false;
            return true;
		}
	}
}
