using System;
namespace Xbim.Xids
{
	public partial class MaterialFacet : IFacet, IEquatable<MaterialFacet>
	{
		public string MaterialName { get; set; } = "";

		public string Short()
		{
			return ToString();
		}

		public override string ToString()
		{
			return $"{MaterialName}";
		}

		public bool Equals(MaterialFacet other)
		{
			if (other == null)
				return false;
			if (MaterialName.ToLowerInvariant() != other.MaterialName.ToLowerInvariant())
				return false;
			return true;
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