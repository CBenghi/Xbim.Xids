using System;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{

	/// <summary>
	/// Constrain model parts on the ground of class attributes.
	/// Either directly or via a type relation (see <see cref="Location"/>).
	/// </summary>
	public partial class AttributeFacet : LocatedFacet, IEquatable<AttributeFacet>, IFacet
	{
		/// <summary>
		/// Constraint that is applied to the value of the attribute (required).
		/// </summary>
		public ValueConstraint AttributeName { get; set; } = "";


		// todo: IDSTALK: Left empty means any class that could have the attribute of the given match?
		//       is this a valid way of identifying classes?

		/// <summary>
		/// Constraint that is applied to the value of the attribute (optional).
		/// </summary>
		public ValueConstraint AttributeValue { get; set; } 

		public bool Equals(AttributeFacet other)
		{
			if (other == null)
				return false;
			var thisEqual = (AttributeName, AttributeValue)
				.Equals((other.AttributeName, other.AttributeValue));
			if (!thisEqual)
				return false;
			return base.Equals(other as LocatedFacet);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as AttributeFacet);
		}

		public override string ToString()
		{
			return $"{AttributeName}-{AttributeValue}-{base.ToString()}";
		}

		public override int GetHashCode() => 23 + 31 *(AttributeName, AttributeValue).GetHashCode() + 31 * base.GetHashCode();

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