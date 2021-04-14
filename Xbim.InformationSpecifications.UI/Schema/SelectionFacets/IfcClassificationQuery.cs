using System;

namespace Xbim.IDS
{
	public partial class IfcClassificationQuery : IFilter, IEquatable<IfcClassificationQuery>
	{
		/// <summary>
		/// A string identifying the relevant classification system
		/// </summary>
		public string ClassificationSystem { get; set; }

		/// <summary>
		/// The specific class element within the tree of the <see cref="ClassificationSystem"/> 
		/// </summary>
		public string Node { get; set; }

		public bool Equals(IfcClassificationQuery other)
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
			return this.Equals(obj as IfcClassificationQuery);
		}

		public override int GetHashCode()
		{
			return $"{ClassificationSystem}-{Node}".GetHashCode();
		}

		public string Short()
		{
			return "SomeClassificationFilter";
		}
	}
}