using System;
using System.Diagnostics.CodeAnalysis;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Base class of all facets.
    /// </summary>
	public abstract class FacetBase : IEquatable<FacetBase>
	{
        internal static bool IsNullOrEmpty([NotNullWhen(false)] ValueConstraint? evaluatingConstraint)
        {
			if (evaluatingConstraint == null)
				return true;
			return evaluatingConstraint.IsEmpty();
        }

        /// <summary>
        /// Optional, but not null, information of any public URI Id of the facet.
        /// </summary>
		public string Uri { get; set; } = string.Empty; 
        /// <summary>
        /// Optional instructions relevant to the facet.
        /// </summary>
		public string Instructions { get; set; } = string.Empty; // element

        /// <inheritdoc />
		public override string ToString()
		{
			return $"{Uri}-{Instructions}";
		}

        /// <inheritdoc />
		public bool Equals(FacetBase? other)
		{
			if (other == null)
				return false;
			if (!IFacetExtensions.CaseInsensitiveEquals(Instructions, other.Instructions))
				return false;
			if (!IFacetExtensions.NullEquals(Uri, other.Uri))
				return false;
			return true;
		}

        /// <inheritdoc />
		public override bool Equals(object? obj)
		{
			return Equals(obj as FacetBase);
		}

        /// <inheritdoc />
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}
}
