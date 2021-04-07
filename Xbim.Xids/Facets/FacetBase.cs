using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.Xids
{
	[Flags]
	public enum Location
	{
		any = 3,
		instance = 1,
		type = 2,
	}

	public enum Use
	{
		undefined,
		required,
		optional
	}


	public abstract class FacetBase : IEquatable<FacetBase>
	{
		private string location = Xbim.Xids.Location.any.ToString();

		public Location GetLocation()
		{
			if (Enum.TryParse<Location>(location, out var loc))
			{
				return loc;
			}
			return Xbim.Xids.Location.any;
		}
		public void SetLocation(Location loc)
		{
			location = loc.ToString();
		}



		/// <summary>
		/// String value of location, use <see cref="GetLocation()"/> for the enum.
		/// Setting an invalid string will ignore the change.
		/// </summary>
		public string Location
		{
			get => location;
			set
			{
				if (Enum.TryParse<Location>(location, out _))
					location = value;
			}
		}

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
