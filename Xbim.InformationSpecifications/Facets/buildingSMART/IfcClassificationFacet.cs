using System;
using System.Collections.Generic;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Constrain model parts on the ground of how classifications are applied to them
    /// Either directly or via a type relation.
    /// </summary>
    public partial class IfcClassificationFacet : FacetBase, IFacet, IEquatable<IfcClassificationFacet>
	{
		/// <summary>
		/// A string identifying the relevant classification system
		/// </summary>
		public ValueConstraint? ClassificationSystem { get; set; }

		/// <summary>
		/// The specific class element within the tree of the <see cref="ClassificationSystem"/>.
		/// </summary>
		public ValueConstraint? Identification { get; set; }

		/// <summary>
		/// Includes hierarchical values below the <see cref="Identification"/> element.
		/// Defaults to false on newly created class.
		/// </summary>
		public bool IncludeSubClasses { get; set; }

        /// <inheritdoc />
		public string Short()
		{
			if (
				ClassificationSystem == null
				&& Identification == null
				)
				return "Any valid classification";
			var desc = new List<string>();

			if (ClassificationSystem != null)
			{
				desc.Add($"classification system {ClassificationSystem.Short()}");
			}
			if (Identification != null)
			{
				desc.Add($"identification {Identification.Short()}");
			}		
			var tmp = string.Join(" and ", desc.ToArray()) + ".";
			return tmp.FirstCharToUpper();
		}


        /// <inheritdoc />
        public bool Equals(IfcClassificationFacet? other)
		{
			if (other == null)
				return false;

			if (IncludeSubClasses != other.IncludeSubClasses)
				return false;

			if (!IFacetExtensions.NullEquals(
				ClassificationSystem, other.ClassificationSystem)
				)
				return false;
			if (!IFacetExtensions.NullEquals(
				Identification, other.Identification)
				)
				return false;
			return base.Equals(other);
		}
        /// <inheritdoc />
		public override bool Equals(object? obj)
		{
			return this.Equals(obj as IfcClassificationFacet);
		}
        /// <inheritdoc />
		public override string ToString()
		{
			return $"{ClassificationSystem}-{Identification}-{IncludeSubClasses}-{base.ToString()}";
		}
        /// <inheritdoc />
		public override int GetHashCode() => 23 + 31 * (ClassificationSystem, Identification, IncludeSubClasses).GetHashCode() + 53 * base.GetHashCode();

        /// <inheritdoc />
        public bool IsValid()
		{
			// if we assume that all empty means that it's enough to have any classification
			// then the facet is always valid.
			return true;
		}
	}
}