using System;

namespace Xbim.InformationSpecifications
{
	[Flags]
	public enum Location
	{
		/// <summary>
		/// Values can be found directly in the entity or via the IfcRelDefinesByType logic.
		/// </summary>
		any = 3,
		/// <summary>
		/// Value is defined directly in the entity.
		/// </summary>
		instance = 1,
		/// <summary>
		/// Value is defined via the IfcRelDefinesByType logic only.
		/// </summary>
		type = 2,
	}

	/// <summary>
	/// A class extending the base facet with information about the value retrieval logic via the `IfcRelDefinesByType` logic.
	/// </summary>
	public abstract class LocatedFacet : FacetBase, IEquatable<LocatedFacet>
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

        public void SetLocation(Location loc)
		{
			location = loc.ToString();
		}

		/// <summary>
		/// String value of location, use <see cref="GetLocation()"/> or <see cref="SetLocation(InformationSpecifications.Location)"/> for the enum.
		/// Setting an invalid string will ignore the change.
		/// The default value for location is <see cref="Location.any"/>
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

		public override string ToString()
		{
			return $"{Location}-{base.ToString()}";
		}

		public bool Equals(LocatedFacet other)
		{
			if (other == null)
				return false;
			if (!IFacetExtensions.CaseInsensitiveEquals(Location, other.Location))
				return false;
			return base.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as LocatedFacet);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}
}
