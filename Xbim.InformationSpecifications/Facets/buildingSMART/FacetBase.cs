using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Base class of all facets.
    /// </summary>
	public abstract class FacetBase : IEquatable<FacetBase> // , IFacet
    {
        /// <summary>
        /// Any text label
        /// </summary>
        protected const string Any = "<any>";
        internal static bool IsNullOrEmpty([NotNullWhen(false)] ValueConstraint? evaluatingConstraint)
        {
            if (evaluatingConstraint == null)
                return true;
            return evaluatingConstraint.IsEmpty() || (evaluatingConstraint.IsSingleExact(out var res) && string.IsNullOrEmpty(res.ToString()));
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

        /// <summary>
        /// Requires the passed <paramref name="constraint"/> to be not null and valid
        /// </summary>
        /// <param name="constraint">The constraint to test</param>
        /// <returns>True if constraint is not null and valid</returns>
        protected static bool IsValid([NotNullWhen(true)]ValueConstraint? constraint)
        {
            if (constraint is null)
                return false;
            return constraint.IsValid();
        }

        /// <summary>
        /// Requires the passed <paramref name="constraint"/> to be not null, not empty and valid
        /// </summary>
        /// <param name="constraint">The constraint to test</param>
        /// <returns>True if constraint is not null and valid</returns>
        protected static bool IsValidAndNotEmpty(ValueConstraint? constraint)
        {
            if (constraint is null)
                return false;
            return constraint.IsValid();
        }

        /// <summary>
        /// Requires the passed <paramref name="constraint"/> to be either null or valid
        /// </summary>
        /// <param name="constraint">The constraint to test</param>
        /// <returns>True if constraint is either null or valid</returns>
        protected static bool IsValidOrNull(ValueConstraint? constraint)
        {
            if (constraint is null)
                return true;
            return constraint.IsValid();
        }
    }
}
