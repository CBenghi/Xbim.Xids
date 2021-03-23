using System;
namespace Xbim.Xids
{
	public partial class MaterialFacet : LocationBase,  IFacet, IEquatable<MaterialFacet>
	{
		public Value Value { get; set; } = null;

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
			return ((LocationBase)this).Equals((LocationBase)other);
		}
		public override bool Equals(object obj)
		{
			return this.Equals(obj as MaterialFacet);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}
}