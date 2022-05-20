using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using Xbim.InformationSpecifications.Cardinality;

namespace Xbim.InformationSpecifications
{
	public enum IfcSchemaVersion
    {
        Undefined,
		IFC2X3,
		IFC4,
		IFC4X3,
	}

    public partial class Specification : ISpecificationMetadata
    {
        private Xids ids;

        [Obsolete("Only for persistence, use the Xids.NewSpecification() method, instead.")]
        public Specification()
        {
            Parent = new SpecificationsGroup();
            ids = new Xids();
            Cardinality = new SimpleCardinality();
        }

        public ICardinality Cardinality { get; set; } 

        [JsonIgnore]
        SpecificationsGroup Parent { get; set; }

        public List<IfcSchemaVersion>? IfcVersion { get; set; } // bS

        public Specification(Xids ids, SpecificationsGroup parent)
        {
            this.ids = ids;
            Parent = parent;
            Guid = System.Guid.NewGuid().ToString();
            Cardinality = new SimpleCardinality();
        }

        /// <summary>
        /// Used to set the provider directly on this instance, otherwise inherited.
        /// Use <see cref="GetProvider"/>.
        /// </summary>
        public string? Provider { get; set; }

        public string? GetProvider()
        {
            if (!string.IsNullOrWhiteSpace(Provider))
                return Provider;
            if (Parent != null)
                return Parent.Provider;
            return Provider;
        }

        /// <summary>
        /// Used to set the consumers directly on this instance, otherwise inherited.
        /// Use <see cref="GetConsumers"/>.
        /// </summary>
        public List<string>? Consumers { get; set; }

        public IEnumerable<string> GetConsumers()
        {
            if (Consumers != null && Consumers.Any())
                return Consumers;
            if (Parent?.Consumers != null)
                return Parent.Consumers;
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Used to set the stages directly on this instance, otherwise inherited.
        /// Use <see cref="GetStages"/>.
        /// </summary>
        public List<string>? Stages { get; set; }

        public IEnumerable<string> GetStages()
        {
            if (Stages != null && Stages.Any())
                return Stages;
            if (Parent?.Stages != null)
                return Parent.Stages;
            return Enumerable.Empty<string>();
        }

        public string? Name { get; set; } // bS
        public string? Description { get; set; } // bS


        private FacetGroup? applicability;

        [JsonIgnore]
        [AllowNull] // allows setting null, but always return not null
        public FacetGroup Applicability {
            get => applicability ?? new FacetGroup(ids.FacetRepository);
            set => applicability = value;
        }

        private string? applicabilityId;

        [JsonPropertyName("Applicability")]
        public string? ApplicabilityId
        {
            get => Applicability.Guid?.ToString();
            set => applicabilityId = value;
        }

        [JsonIgnore]
        public FacetGroup? Requirement { get; set; }

        private string? requirementId;
        
        [JsonPropertyName("Requirement")]
        public string? RequirementId
        {
            get => Requirement?.Guid?.ToString();
            set => requirementId = value;
        }

        public string? Instructions { get; set; }

        public string? Guid { get; set; }

        internal void SetExpectations(List<IFacet> fs)
        {
            var existing = ids.GetFacetGroup(fs);
            if (existing != null)
            {
                Requirement = existing;
                return;
            }
            if (Requirement == null)
                Requirement = new FacetGroup(ids.FacetRepository);
            foreach (var item in fs)
            {
                Requirement.Facets.Add(item);
            }
        }

        internal void SetFilters(List<IFacet> fs)
        {
            var existing = ids.GetFacetGroup(fs);
            if (existing != null)
            {
                Applicability = existing;
                return;
            }
            if (Applicability == null)
                Applicability = new FacetGroup(ids.FacetRepository);
            foreach (var item in fs)
            {
                Applicability.Facets.Add(item);
            }
        }

        public string Short()
        {
            if (Name is not null && !string.IsNullOrWhiteSpace(Name))
                return Name;
            return "<Unnamed>";
        }

        internal void SetIds(Xids unpersisted)
        {
            ids = unpersisted;
            var t = unpersisted.GetFacetGroup(requirementId);
            if (t != null)
                Requirement = t;
            // collections
            var m = unpersisted.GetFacetGroup(applicabilityId);
            if (m != null)
                Applicability = m;
            var f = unpersisted.GetFacetGroup(requirementId);
            if (f != null)
                Requirement = f;

            if (Guid == null)
                Guid = System.Guid.NewGuid().ToString();
        }
    }
}
