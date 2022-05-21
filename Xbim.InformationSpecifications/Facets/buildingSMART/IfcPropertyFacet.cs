using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Constrain model parts on the ground of properties (of type IfcSingleProperty) associated via PropertySets
    /// Either directly or via a type relation.
    /// </summary>
	public partial class IfcPropertyFacet : FacetBase, IFacet, IEquatable<IfcPropertyFacet>
    {

        /// <summary>
        /// Constraint that is applied to the name of the PropertySet.
        /// </summary>
        public ValueConstraint? PropertySetName { get; set; }

        /// <summary>
        /// Constraint that is applied to the name of the Property.
        /// </summary>
        public ValueConstraint? PropertyName { get; set; }
		
        /// <summary>
        /// Constrained type of the identified property value.
        /// </summary>
        public string? Measure { get; set; }

        /// <summary>
        /// Constraint that is applied to the value of the Property.
        /// </summary>
        public ValueConstraint? PropertyValue { get; set; } 

        public string Short()
        {
            var sb = new StringBuilder();
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

        public override bool Equals(object? obj)
        {
            return this.Equals(obj as IfcPropertyFacet);
        }

        public override int GetHashCode() => 23 + 31 * (PropertySetName, PropertyName, PropertyValue, Measure).GetHashCode() + 31 * base.GetHashCode();

        public override string ToString() => $"{PropertySetName}-{PropertyName}-{PropertyValue?.ToString()??""}-{Measure}-{base.ToString()}";
        
		public bool Equals(IfcPropertyFacet? other)
        {
            if (other == null)
                return false;
            if (!IFacetExtensions.CaseInsensitiveEquals(PropertySetName, other.PropertySetName))
                return false;
            if (!IFacetExtensions.CaseInsensitiveEquals(PropertyName, other.PropertyName))
                return false;
            if (!IFacetExtensions.NullEquals(PropertyValue, other.PropertyValue))
                return false;
            if (!IFacetExtensions.CaseInsensitiveEquals(Measure, other.Measure))
                return false;
            return base.Equals(other);
        }

        [MemberNotNullWhen(true, nameof(PropertySetName))]
        [MemberNotNullWhen(true, nameof(PropertyName))]
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
