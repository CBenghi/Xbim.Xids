using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// A group of facets, used to identify parts of models that match specifications, and 
    /// also their requirements.
    /// </summary>
	public partial class FacetGroup : IEnumerable<IFacet>
    {
        /// <summary>
        /// Use only for persistence and testing, otherwise prefer other constructors
        /// </summary>
		[Obsolete("Use only for persistence and testing, otherwise prefer other constructors")]
        [JsonConstructor]
        public FacetGroup()
        {
            Guid = System.Guid.NewGuid().ToString();
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

        /// <summary>
        /// An unique identifier is created in the constructor, but it can be set with this property.
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// The group can be optionally identified with a name.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// If the facet group defined in an external document, the reference can be specified here.
        /// </summary>
		public string? Reference { get; set; }
        /// <summary>
        /// An optional textual description of the group.
        /// </summary>
		public string? Description { get; set; }

        /// <summary>
        /// Determines options associated with the collection of facets, when used as a requirement
        /// </summary>
        public ObservableCollection<RequirementCardinalityOptions>? RequirementOptions { get; set; }

        /// <summary>
        /// Collection of the facets defined in the group.
        /// </summary>
		public ObservableCollection<IFacet> Facets { get; set; } = new ObservableCollection<IFacet>();
        // set is required for persistence.

        /// <summary>
        /// Identifies the possible ways in which a Facet may be used; see IsUsed methods.
        /// </summary>
		[Flags]
        public enum FacetUse
        {
            /// <summary>
            /// No use.
            /// </summary>
			None = 0,
            /// <summary>
            /// Used in determining the applicability of a specification, see <see cref="Specification.applicability"/>.
            /// </summary>
			Applicability = 1,
            /// <summary>
            /// Used for defining the requirements of a specification, see <see cref="Specification.Requirement"/>.
            /// </summary>
			Requirement = 2,
            /// <summary>
            /// Used as a source dataset for a relationship facet, <see cref="IRepositoryRef"/>.
            /// </summary>
			RelationSource = 4,
            /// <summary>
            /// All use flags combined.
            /// </summary>
			All = ~None
        }

        /// <summary>
        /// Provides information on the usage of the FacetGroup in a context.
        /// </summary>
        /// <param name="container">the context to seek</param>
        /// <param name="mode">Identifies the possible ways in which a Facet use should be considered in the search</param>
        /// <returns>True, if the specified use is found in the context; false otherwise.</returns>
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

        /// <summary>
        /// Provides information on the usage of the FacetGroup in a context.
        /// </summary>
        /// <param name="context">the context to seek</param>
        /// <param name="mode">Identifies the possible ways in which a Facet use should be considered in the search</param>
        /// <returns>True, if the specified use is found in the context; false otherwise.</returns>
        public bool IsUsed(Xids context, FacetUse mode)
        {
            if (mode.HasFlag(FacetUse.Applicability))
            {
                if (context.AllSpecifications().Any(x => x.Applicability == this))
                    return true;
            }
            if (mode.HasFlag(FacetUse.Requirement))
            {
                if (context.AllSpecifications().Any(x => x.Requirement == this))
                    return true;
            }
            if (mode.HasFlag(FacetUse.RelationSource))
            {
                // not tested
                if (context.FacetRepository.Collection.OfType<IRepositoryRef>().Any(
                    x => x.UsedGroups().Contains(this)
                    ))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Identifies the number of usages of the group in a context.
        /// </summary>
        /// <param name="context">The context being investigated</param>
        /// <returns>an integer of how many uses, this could be useful to provide warning for editing purposes.</returns>
		public int UseCount(Xids context)
        {
            var directSpecificationUse = context.AllSpecifications().Count(x => x.Applicability == this || x.Requirement == this);
            var relatedUse = context.FacetRepository.Collection.SelectMany(x => x.Facets.OfType<IRepositoryRef>().Where(y => y.UsedGroups().Contains(this))).Count();
            return directSpecificationUse + relatedUse;
        }


        /// <summary>
        /// Determines the validity of the instance for the purposes of IDS.
        /// Completed FacetGroups should not be invalid.
        /// </summary>
        /// <returns>False if any of the facets is invalid or the list is empty, or the requirementOptions collection does not match the facet count.</returns>
        public bool IsValid()
        {
            if (!Facets.Any())
                return false;
            if (RequirementOptions is not null)
            {
                if (RequirementOptions.Count != 0 && RequirementOptions.Count != Facets.Count)
                    return false;
            }
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
        /// <param name="facetGroup">The grout to be checked, if null, returns false.</param>
        /// <returns>true if the <paramref name="facetGroup"/> is not null and all its facets are valid.</returns>
        public static bool IsValid([NotNullWhen(true)] FacetGroup? facetGroup)
        {
            if (facetGroup is null)
                return false;
            return facetGroup.IsValid();
        }

        /// <summary>
        /// The string returned from <see cref="Short()"/>, in case of missing information.
        /// </summary>
        public const string Undefined = "<undefined>";

        /// <summary>
        /// Short textual description of the <see cref="FacetGroup"/>, automatically generated.
        /// </summary>
        /// <returns>A generated description string, if information is meaningful, otherwise the <see cref="Undefined"/> constant.</returns>
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

        /// <inheritdoc />
        public IEnumerator<IFacet> GetEnumerator()
        {
            return Facets.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}