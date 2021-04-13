using System;
namespace Xbim.InformationSpecifications
{
	public partial class MaterialFacet : FacetBase,  IFacet, IEquatable<MaterialFacet>
	{
		public ValueConstraint Value { get; set; } = null;

		public string Short()
		{
			return ToString();
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
			return ((FacetBase)this).Equals((FacetBase)other);
		}
		public override bool Equals(object obj)
		{
			return this.Equals(obj as MaterialFacet);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public bool IsValid()
		{
			// an empty one just means should have a material
			return true;
		}
	}
}