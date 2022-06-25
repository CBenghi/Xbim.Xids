using System;
namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Constrain model parts on the ground of a material identified through a relation.
    /// </summary>
    public partial class MaterialFacet : FacetBase, IFacet, IEquatable<MaterialFacet>
    {
        /// <summary>
        /// Constraint on the material's name
        /// </summary>
        public ValueConstraint? Value { get; set; } = null;

        /// <inheritdoc />
		public string Short()
        {
            if (Value == null)
            {
                return "Specifies any valid material.";
            }
            return $"Has a material's name {Value.Short()}";
        }

        /// <inheritdoc />
		public override string ToString()
        {
            return $"{Value}-{base.ToString()}";
        }

        /// <inheritdoc />
		public bool Equals(MaterialFacet? other)
        {
            if (other == null)
                return false;
            if (!IFacetExtensions.NullEquals(Value, other.Value))
                return false;
            return base.Equals(other);
        }

        /// <inheritdoc />
		public override bool Equals(object? obj)
        {
            return this.Equals(obj as MaterialFacet);
        }

        /// <inheritdoc />
		public override int GetHashCode() => 23 + 31 * (Value, true).GetHashCode() + 31 * base.GetHashCode();


        /// always valid (see <see cref="IFacet.IsValid"/>).
		public bool IsValid()
        {
            return FacetBase.IsValidButOptional(Value);
        }
    }
}