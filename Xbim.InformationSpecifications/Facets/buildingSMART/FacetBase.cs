using System;

namespace Xbim.InformationSpecifications
{
	
	public enum Use
	{
		undefined,
		required,
		optional,
		prohibited
	}

	public abstract class FacetBase : IEquatable<FacetBase>
	{
        internal static bool IsNullOrEmpty(ValueConstraint? evaluatingConstraint)
        {
			if (evaluatingConstraint == null)
				return true;
			return evaluatingConstraint.IsEmpty();
        }

		public string Uri { get; set; } = string.Empty; // attribute uri
		public string Use { get; set; } = InformationSpecifications.Use.undefined.ToString();
		public string Instructions { get; set; } = string.Empty; // element

		public override string ToString()
		{
			return $"{Uri}-{Use}-{Instructions}";
		}

		public bool Equals(FacetBase? other)
		{
			if (other == null)
				return false;
			if (!IFacetExtensions.CaseInsensitiveEquals(Use, other.Use))
				return false;
			if (!IFacetExtensions.CaseInsensitiveEquals(Instructions, other.Instructions))
				return false;
			if (!IFacetExtensions.NullEquals(Uri, other.Uri))
				return false;
			return true;
		}

		public override bool Equals(object? obj)
		{
			return this.Equals(obj as FacetBase);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}
}
