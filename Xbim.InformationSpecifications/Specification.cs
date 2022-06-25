using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using Xbim.InformationSpecifications.Cardinality;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Closed list of Schema names
    /// </summary>
    public enum IfcSchemaVersion
    {
        /// <summary>
        /// When no information is defined
        /// </summary>
        Undefined,
        /// <summary>
        /// Ifc2x3 Schema
        /// </summary>
        IFC2X3,
        /// <summary>
        /// Ifc4 schema
        /// </summary>
        IFC4,
        /// <summary>
        /// Ifc4x3 schema
        /// </summary>
        IFC4X3,
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Specification : ISpecificationMetadata
    {
        private Xids GetIds() => Parent.GetParent();
        
        /// <summary>
        /// Only for persistence, use one of the other methods:
        /// <see cref="Xids.PrepareSpecification(IEnumerable{IfcSchemaVersion}, FacetGroup?, FacetGroup?)"/>, 
        /// <see cref="Xids.PrepareSpecification(IfcSchemaVersion, FacetGroup?, FacetGroup?)"/>, 
        /// <see cref="Xids.PrepareSpecification(SpecificationsGroup, IfcSchemaVersion, FacetGroup?, FacetGroup?)"/>, 
        /// <see cref="Xids.PrepareSpecification(SpecificationsGroup?, IEnumerable{IfcSchemaVersion}, FacetGroup?, FacetGroup?)"/>
        /// </summary>
        [Obsolete("Only for persistence, use the Xids.NewSpecification() method, instead.")]
        public Specification()
        {
            Parent = new SpecificationsGroup();
            // ids = new Xids();
            Cardinality = new SimpleCardinality();
        }

        /// <summary>
        /// Provides ways to constrain the the amount of entities related to the Specification
        /// </summary>
        public ICardinality Cardinality { get; set; }

        [JsonIgnore]
        internal SpecificationsGroup Parent { get; set; }

        /// <summary>
        /// optional list of IFC versions compabile with the specification
        /// </summary>
        public List<IfcSchemaVersion>? IfcVersion { get; set; } // bS

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parent">Owning specification group</param>
        /// 
        public Specification(SpecificationsGroup parent)
        {
            Parent = parent;
            Guid = System.Guid.NewGuid().ToString();
            Cardinality = new SimpleCardinality();
        }

        /// <summary>
        /// Used to set the provider directly on this instance, otherwise inherited.
        /// Use <see cref="GetProvider"/>.
        /// </summary>
        public string? Provider { get; set; }

        /// <summary>
        /// Get the planned provider of the specification (direct or inherited) 
        /// </summary>
        /// <returns>A string identifying the provider</returns>
        public string GetProvider()
        {
            if (!string.IsNullOrWhiteSpace(Provider))
                return Provider!;
            return Parent.GetProvider();
        }

        /// <summary>
        /// Used to set the consumers directly on this instance, otherwise inherited.
        /// Use <see cref="GetConsumers"/>.
        /// </summary>
        public IList<string>? Consumers { get; set; } 

        /// <summary>
        /// Get consumers of the specification (direct or inherited) 
        /// </summary>
        /// <returns>A list of strings identifying the consumers</returns>
        public IEnumerable<string> GetConsumers()
        {
            if (Consumers != null)
                return Consumers;
            var temp = Parent.GetConsumers();
            if (temp != null)
                return temp;
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Used to set the stages directly on this instance, otherwise inherited.
        /// Use <see cref="GetStages"/>.
        /// </summary>
        public IList<string>? Stages { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> GetStages()
        {
            if (Stages != null)
                return Stages;
            var temp = Parent.GetStages();
            if (temp != null)
                return temp;
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Specification name is available also in buildingSmart format
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Specification Description is available also in buildingSmart format
        /// </summary>
        public string? Description { get; set; } // bS

        private FacetGroup? applicability;

        /// <summary>
        /// Defines the subset of models that are affected by the specification.
        /// It can be set also via the <see cref="ApplicabilityId"/> property.
        /// </summary>
        [JsonIgnore]
        [AllowNull] // allows setting null, but always return not null
        public FacetGroup Applicability
        {
            get
            {
                if (applicability == null)
                    applicability = new FacetGroup(GetIds().FacetRepository);
                return applicability;
            }
            set => applicability = value;
        }

        private string? applicabilityId;


        /// <summary>
        /// Is the guid of the <see cref="Applicability"/> facet group.
        /// </summary>
        [JsonPropertyName("Applicability")]
        public string? ApplicabilityId
        {
            get => Applicability.Guid?.ToString();
            set => applicabilityId = value;
        }

        /// <summary>
        /// Optionally defines the requirements of the the subset of models that are affected by the specification.
        /// It can be set also via the <see cref="RequirementId"/> property.
        /// </summary>
        [JsonIgnore]
        public FacetGroup? Requirement { get; set; }

        private string? requirementId;
        
        /// <summary>
        /// Gets or sets the reuquirement ID
        /// </summary>
        [JsonPropertyName("Requirement")]
        public string? RequirementId
        {
            get => Requirement?.Guid?.ToString();
            set
            {
                requirementId = value;
                if (GetIds() is not null)
                {
                    Requirement = GetIds().GetFacetGroup(value);
                }
            }
        }

        /// <summary>
        /// Optional string providing instructions for the end users.
        /// </summary>
        public string? Instructions { get; set; }

        private string? guid;

        /// <summary>
        /// Unique identification of the Specification
        /// </summary>
        public string Guid
        {
            get
            {
                if (string.IsNullOrEmpty(guid))
                {
                    guid = System.Guid.NewGuid().ToString();
                }
                return guid!;
            }

            set
            {
                guid = value;
            }
        }

        /// <inheritdoc />
        public SpecificationLevel Level => SpecificationLevel.SingleSpecification;

        internal void SetExpectations(List<IFacet> fs)
        {
            var existing = GetIds().GetFacetGroup(fs);
            if (existing != null)
            {
                Requirement = existing;
                return;
            }
            if (Requirement == null)
                Requirement = new FacetGroup(GetIds().FacetRepository);
            foreach (var item in fs)
            {
                Requirement.Facets.Add(item);
            }
        }

        internal void SetFilters(List<IFacet> fs)
        {
            var existing = GetIds().GetFacetGroup(fs);
            if (existing != null)
            {
                Applicability = existing;
                return;
            }
            if (Applicability == null)
                Applicability = new FacetGroup(GetIds().FacetRepository);
            foreach (var item in fs)
            {
                Applicability.Facets.Add(item);
            }
        }

        /// <returns>False if any of the specification's data is not suitable for testing.</returns>
        public bool IsValid()
        {
            if (!Applicability.IsValid())
                return false;
            if (!Cardinality.IsValid())
                return false;
            if (
                Cardinality.ExpectsRequirements
                &&
                    (Requirement == null
                    ||
                    !Requirement.IsValid())
                )
                return false;
            return true;
        }

        /// <summary>
        /// Short text description
        /// </summary>
        /// <returns>a the name string if defined, or the valid GUID</returns>
        public string Short()
        {
            if (Name is not null && !string.IsNullOrWhiteSpace(Name))
                return Name;
            return Guid;
        }

        internal void SetParent(SpecificationsGroup parent)
        {
            Parent = parent;
            // collections
            var m = GetIds().GetFacetGroup(applicabilityId);
            if (m != null)
                Applicability = m;
            var f = GetIds().GetFacetGroup(requirementId);
            if (f != null)
                Requirement = f;
        }
    }
}
