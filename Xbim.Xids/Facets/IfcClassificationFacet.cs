using System;

namespace Xbim.Xids
{
	public partial class IfcClassificationFacet : IFacet, IEquatable<IfcClassificationFacet>
	{
		/// <summary>
		/// A string identifying the relevant classification system
		/// </summary>
		public string ClassificationSystem { get; set; } = "";

		/// <summary>
		/// The specific class element within the tree of the <see cref="ClassificationSystem"/> 
		/// </summary>
		public string Node { get; set; } = "";

		public bool Equals(IfcClassificationFacet other)
		{
			if (other == null)
				return false;
			// todo: 2021: needs to clarify if the tests are case sensitive or not
			if (ClassificationSystem.ToLowerInvariant() != other.ClassificationSystem.ToLowerInvariant())
				return false;
			// todo: 2021: needs to clarify if the tests are case sensitive or not
			if (Node.ToLowerInvariant() != other.Node.ToLowerInvariant())
				return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as IfcClassificationFacet);
		}

		public override string ToString()
		{
			return $"{ClassificationSystem}-{Node}";
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