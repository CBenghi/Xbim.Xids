using System;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
	

	public partial class AttributeFacet : IFacet, IEquatable<AttributeFacet>
	{
		public ValueConstraint AttributeName { get; set; } = "";

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


		public ValueConstraint AttributeValue { get; set; } 

		public bool Equals(AttributeFacet other)
		{
			if (other == null)
				return false;
			var thisEqual = (AttributeName, AttributeValue, Location)
				.Equals((other.AttributeName, other.AttributeValue, other.Location));
			return thisEqual;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as AttributeFacet);
		}

		public override string ToString()
		{
			return $"{AttributeName}-{AttributeValue}-{Location}";
		}

		public override int GetHashCode() => (AttributeName, AttributeValue, Location).GetHashCode();

		public string Short()
		{
			if (string.IsNullOrEmpty(Location))
				return $"attribute {AttributeName} = {AttributeValue}";
			else
				return $"attribute {AttributeName} @ {Location} = {AttributeValue}";
		}

		public bool IsValid()
		{
			return !FacetBase.IsNullOrEmpty(AttributeName);
		}
	}
}