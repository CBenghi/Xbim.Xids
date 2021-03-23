using System;

namespace Xbim.Xids
{
	public partial class IfcClassificationFacet : LocationBase, IFacet, IEquatable<IfcClassificationFacet>
	{
		/// <summary>
		/// A string identifying the relevant classification system
		/// </summary>
		public string ClassificationSystem { get; set; }

		/// <summary>
		/// The specific class element within the tree of the <see cref="ClassificationSystem"/> 
		/// </summary>
		public string Node { get; set; }

		public bool Equals(IfcClassificationFacet other)
		{
			if (other == null)
				return false;

			if (!IFacetExtensions.NullableStringCaseInsensitiveEquals(
				ClassificationSystem, other.ClassificationSystem)
				)
				return false;
			if (!IFacetExtensions.NullableStringCaseInsensitiveEquals(
				Node, other.Node)
				)
				return false;
			return ((LocationBase)this).Equals((LocationBase)other);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as IfcClassificationFacet);
		}

		public override string ToString()
		{
			return $"{ClassificationSystem}-{Node}-{base.ToString()}";
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public string Short()
		{
			return ToString();
		}
	}
}