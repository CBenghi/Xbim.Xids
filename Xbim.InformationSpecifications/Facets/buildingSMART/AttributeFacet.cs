using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xbim.InformationSpecifications.Facets.buildingSMART;

namespace Xbim.InformationSpecifications
{

    /// <summary>
    /// Constrain model parts on the ground of class attributes.
    /// </summary>
    public partial class AttributeFacet : FacetBase, IEquatable<AttributeFacet>, IFacet, IBuilsingSmartCardinality
    {
        /// <summary>
        /// Constraint that is applied to the value of the attribute (required).
        /// </summary>
        public ValueConstraint? AttributeName { get; set; }

        // todo: IDSTALK: Left empty means any class that could have the attribute of the given match?
        //       is this a valid way of identifying classes?

        /// <summary>
        /// Constraint that is applied to the value of the attribute (optional).
        /// </summary>
        public ValueConstraint? AttributeValue { get; set; }

        /// <inheritdoc/>
        public string RequirementDescription
        {
            get
            {
                return $"an attribute {AttributeName?.Short() ?? Any} with value {AttributeValue?.Short() ?? Any}";
            }
        }


        /// <inheritdoc/>
        public string ApplicabilityDescription
        {
            get
            {
                return $"with attribute {AttributeName?.Short() ?? Any} with value {AttributeValue?.Short() ?? Any}";
            }
        }


        /// <inheritdoc />
        public bool Equals(AttributeFacet? other)
        {
            if (other == null)
                return false;
            var thisEqual = (AttributeName, AttributeValue)
                .Equals((other.AttributeName, other.AttributeValue));
            if (!thisEqual)
                return false;
            return base.Equals(other);
        }

        /// <inheritdoc />
		public override bool Equals(object? obj)
        {
            return Equals(obj as AttributeFacet);
        }

        /// <inheritdoc />
		public override string ToString()
        {
            return $"{AttributeName}-{AttributeValue}-{base.ToString()}";
        }

        /// <inheritdoc />
		public override int GetHashCode() => 23 + 31 * (AttributeName, AttributeValue).GetHashCode() + 31 * base.GetHashCode();

        /// <inheritdoc />
		public string Short()
        {
            return $"attribute {AttributeName} = {AttributeValue}";
        }

        /// <inheritdoc />
		[MemberNotNullWhen(true, nameof(AttributeName))]
        public bool IsValid()
        {
            return FacetBase.IsValid(AttributeName)
                && FacetBase.IsValidOrNull(AttributeValue)
                && IdsLib.IfcSchema.SchemaInfo.AllAttributes.Any(x => AttributeName.IsSatisfiedBy(x.IfcAttributeName));
        }
    }
}