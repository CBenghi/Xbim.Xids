using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.Xids
{
	public enum Location
	{
		any,
		instance,
		type,
	}

	public enum Use
	{
		undefined,
		required,
		optional
	}


	public abstract class FacetBase : IEquatable<FacetBase>
	{
		public string Location { get; set; } = null; // attribute location
		public string Uri { get; set; } = null; // attribute href
		public string Use { get; set; } = null; // attribute use
		public string Instructions { get; set; } // element

		public override string ToString()
		{
			return $"{Uri?.ToString() ?? ""}-{Location}-{Use}-{Instructions}";
		}

		public bool Equals(FacetBase other)
		{
			if (other == null)
				return false;
			if (!IFacetExtensions.NullableStringCaseInsensitiveEquals(Location, other.Location))
				return false;
			if (!IFacetExtensions.NullableStringCaseInsensitiveEquals(Use, other.Use))
				return false;
			if (!IFacetExtensions.NullableStringCaseInsensitiveEquals(Instructions, other.Instructions))
				return false;
			if (!IFacetExtensions.NullEquals(Uri, other.Uri))
				return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as FacetBase);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}
}
