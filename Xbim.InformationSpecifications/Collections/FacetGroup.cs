using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;

namespace Xbim.InformationSpecifications
{

	public enum RequirementOptions
	{
		Expected,
		Prohibited
	}

	public partial class FacetGroup
	{
		[Obsolete("Use only for persistence and testing, otherwise prefer other constructors")]
		[JsonConstructor]
		public FacetGroup()
		{
		}

		/// <summary>
		/// Initializes a new instance and adds it to the owning repository.
		/// </summary>
		/// <param name="repository">The owning repository, already associated, no need to add.</param>
		public FacetGroup(FacetGroupRepository repository)
		{
			repository.Add(this);
			Guid = System.Guid.NewGuid().ToString();
		}

		public string? Guid { get; set; }
		public string? Name { get; set; }
		public string? Reference { get; set; }
		public string? Description { get; set; }

		/// <summary>
		/// Determines options associated with the collection of facets, when used as a requirement
		/// </summary>
		public ObservableCollection<RequirementOptions>? RequirementOptions { get; set; }

		public ObservableCollection<IFacet> Facets { get; set; } = new ObservableCollection<IFacet>();

		[Flags]
		public enum FacetUse
		{
			None = 0,
			Applicability = 1,
			Requirement = 2,
			RelationSource = 4,
			All = ~None
		}

		public bool IsUsed(SpecificationsGroup container, FacetUse mode)
		{
			if (mode.HasFlag(FacetUse.Applicability))
			{
				if (container.Specifications.Any(x => x.Applicability == this))
					return true;
			}
			if (mode.HasFlag(FacetUse.Requirement))
			{
				if (container.Specifications.Any(x => x.Requirement == this))
					return true;
			}
			if (mode.HasFlag(FacetUse.RelationSource))
			{
				// not tested
				if (container.UsedFacetGroups().OfType<IRepositoryRef>().Any(
					x => x.UsedGroups().Contains(this)
					))
					return true;
			}
			return false;
		}

		public bool IsUsed(Xids t, FacetUse mode)
		{
			if (mode.HasFlag(FacetUse.Applicability))
			{
				if (t.AllSpecifications().Any(x => x.Applicability == this))
					return true;
			}
			if (mode.HasFlag(FacetUse.Requirement))
			{
				if (t.AllSpecifications().Any(x => x.Requirement == this))
					return true;
			}
			if (mode.HasFlag(FacetUse.RelationSource))
			{
				// not tested
				if (t.FacetRepository.Collection.OfType<IRepositoryRef>().Any(
					x => x.UsedGroups().Contains(this)
					))
					return true;
			}
			return false;
		}

		public int UseCount(Xids t)
		{
			var directSpecificationUse = t.AllSpecifications().Count(x => x.Applicability == this || x.Requirement == this);
			var relatedUse = t.FacetRepository.Collection.SelectMany(x => x.Facets.OfType<IRepositoryRef>().Where(y => y.UsedGroups().Contains(this))).Count();
			return directSpecificationUse + relatedUse;
		}

		/// <returns>False if any of the facets is invalid or the list is empty.</returns>
		public bool IsValid()
		{
			if (!Facets.Any())
				return false;
			foreach (var facet in Facets)
			{
				if (!facet.IsValid())
					return false;
			}
			return true;
		}

		/// <summary>
		/// Ensure that a <see cref="FacetGroup"/>
		/// </summary>
		/// <param name="facetGroup">The grout to be checed, if null, returns false.</param>
		/// <returns>true if the <see cref="facetGroup"/> is not null and all its facets are valid.</returns>
		public static bool IsValid([NotNullWhen(true)] FacetGroup? facetGroup)
        {
			if (facetGroup is null)
				return false;
			return facetGroup.IsValid();	
        }

		public string Short()
		{
			if (!string.IsNullOrWhiteSpace(Name))
				return $"{Name} ({Facets.Count})";
			if (Facets.Any())
			{
				return string.Join(" and ", Facets.Select(x => x.Short()));
			}
			if (Description is not null && !string.IsNullOrWhiteSpace(Description))
				return Description;
			return "<undefined>";
		}

		
	}
}