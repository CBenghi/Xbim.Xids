using System;

namespace Xbim.InformationSpecifications
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
		private string location = InformationSpecifications.Location.any.ToString();

		public Location GetLocation()
		{
			if (Enum.TryParse<Location>(location, out var loc))
			{
				return loc;
			}
			return InformationSpecifications.Location.any;
		}

        internal static bool IsNullOrEmpty(ValueConstraint evaluatingConstraint)
        {
			if (evaluatingConstraint == null)
				return true;
			return evaluatingConstraint.IsEmpty();
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
				if (Enum.TryParse<Location>(value, out _))
					location = value;
			}
		}

		public string Uri { get; set; } = null; // attribute uri
		public string Use { get; set; } = null; // attribute use
		public string Instructions { get; set; } // element

		public override string ToString()
		{
			return $"{Uri}-{Location}-{Use}-{Instructions}";
		}

		public bool Equals(FacetBase other)
		{
			if (other == null)
				return false;
			if (!IFacetExtensions.CaseInsensitiveEquals(Location, other.Location))
				return false;
			if (!IFacetExtensions.CaseInsensitiveEquals(Use, other.Use))
				return false;
			if (!IFacetExtensions.CaseInsensitiveEquals(Instructions, other.Instructions))
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
