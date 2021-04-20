using System;
using System.Collections.Generic;
using System.Text;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
	public partial class IfcClassificationFacet : FacetBase, IFacet, IEquatable<IfcClassificationFacet>
	{
		/// <summary>
		/// A string identifying the relevant classification system
		/// </summary>
		public ValueConstraint ClassificationSystem { get; set; }
		
		/// <summary>
		/// Uri of the classification system
		/// </summary>
		public string ClassificationSystemHref { get; set; }

		/// <summary>
		/// The specific class element within the tree of the <see cref="ClassificationSystem"/>.
		/// </summary>
		public ValueConstraint Identification { get; set; }

		public string Short()
		{
			if (ClassificationSystem == null
				&& Identification == null
				&& string.IsNullOrEmpty(ClassificationSystemHref))
				return "Any valid classification";
			List<string> desc = new List<string>();

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

		/// <summary>
		/// Includes hierarchical values below the <see cref="Identification"/> element.
		/// </summary>
		public bool IncludeSubClasses { get; set; }

		public bool Equals(IfcClassificationFacet other)
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

		public override bool Equals(object obj)
		{
			return this.Equals(obj as IfcClassificationFacet);
		}

		public override string ToString()
		{
			return $"{ClassificationSystem}-{Identification}-{IncludeSubClasses}-{base.ToString()}";
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		

		public bool IsValid()
		{
			// if we assume that all empty means that it's enough to have any classification
			// then the facet is always valid.
			return true;
		}
	}
}