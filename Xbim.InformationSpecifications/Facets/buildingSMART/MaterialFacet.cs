using System;
namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// Constrain model parts on the ground of a material identified through a relation.
	/// </summary>
	public partial class MaterialFacet : LocatedFacet,  IFacet, IEquatable<MaterialFacet>
	{
		/// <summary>
		/// Constraint on the material's name
		/// </summary>
		public ValueConstraint Value { get; set; } = null;

		public string Short()
		{
			if (Value == null)
			{
				return "Specifies any valid material.";
			}
			return $"Has a material's name {Value.Short()}";
		}

		public override string ToString()
		{
			return $"{Value}-{base.ToString()}";
		}

		public bool Equals(MaterialFacet other)
		{
			if (other == null)
				return false;
			if (!IFacetExtensions.NullEquals(Value, other.Value))
				return false;
			return base.Equals(other);
		}
		public override bool Equals(object obj)
		{
			return this.Equals(obj as MaterialFacet);
		}

		public override int GetHashCode() => 23 + 31 * (Value, true).GetHashCode() + 31 * base.GetHashCode();


		public bool IsValid()
		{
			// an empty one just means should have a material
			return true;
		}
	}
}