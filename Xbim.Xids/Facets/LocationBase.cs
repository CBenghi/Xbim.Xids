using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.Xids
{
	public abstract class LocationBase : IEquatable<LocationBase>
	{
		public string Location { get; set; } = "";

		public Uri Uri { get; set; } = null;

		public override string ToString()
		{
			return $"{Uri?.ToString() ?? ""}-{Location}";
		}

		public bool Equals(LocationBase other)
		{
			if (other == null)
				return false;
			if (Location.ToLowerInvariant() != other.Location.ToLowerInvariant())
				return false;
			if (!IFacetExtensions.NullEquals(Uri, other.Uri))
				return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as LocationBase);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}
}
