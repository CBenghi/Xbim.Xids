using System;
using System.Collections.Generic;
using static Xbim.InformationSpecifications.FacetGroup;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// A specification group is virtually capable of containing an entire bS IDS.
    /// Conceptually it applies to a singe model.
    /// It's then expanded to support extra LOIN features for XIDS.
    /// 
    /// Beyond rich metadata it contains a colleciton of <see cref="Specifications"/>, each having applicability and requirements.
    /// </summary>
    public partial class SpecificationsGroup : ISpecificationMetadata
    {
        // main properties

        /// <summary>
        /// Default parameterless constructor, prefer the <see cref="SpecificationsGroup(Xids)"/> constructor instead.
        /// </summary>
        [Obsolete("Used only for testing, prefer the SpecificationsGroup(Xids) constructor instead.")]
        public SpecificationsGroup()
        {
            parent = new Xids();
        }

        internal Xids GetParent() => parent;


        private Xids parent;
        /// <summary>
        /// Default parameterless constructor
        /// </summary>
        public SpecificationsGroup(Xids parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Optional name of the SpecificationsGroup, also in bS (Title)
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Any optional copyright of the data, also in bS
        /// </summary>
        public string? Copyright { get; set; } // bS
        /// <summary>
        /// Optional Version, expressed in a free string.
        /// </summary>
        public string? Version { get; set; } // bS
        /// <summary>
        /// Optional Textual description
        /// </summary>
        public string? Description { get; set; } // bS
        /// <summary>
        /// Optional author of the specifications
        /// </summary>
        public string? Author { get; set; } // bS
        /// <summary>
        /// Optional date of editing
        /// </summary>
        public DateTime? Date { get; set; } // bS
        /// <summary>
        /// Optional Purpose of the document, expressed as a string
        /// </summary>
        public string? Purpose { get; set; }// bS
        /// <summary>
        /// Relevant optional project milestone
        /// </summary>
        public string? Milestone { get; set; }// bS

        /// <summary>
        /// Property is needed for Data editing, but for presentation, prefer the <see cref="GetProvider()"/> method.
        /// </summary>
        public string? Provider { get; set; }
        /// <summary>
        /// Property is needed for Data editing, but for presentation, prefer the <see cref="GetConsumers()"/> method
        /// </summary>
        public IList<string>? Consumers { get; set; }
        /// <summary>
        /// Property is needed for Data editing, but for presentation, prefer the <see cref="GetStages()"/> method
        /// </summary>
        public IList<string>? Stages { get; set; }

        /// <summary>
        /// The set of specifications in the group.
        /// </summary>
        public IList<Specification> Specifications { get; set; } = new List<Specification>();

        private string? guid;

        /// <inheritdoc />
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

        internal IEnumerable<FacetGroup> UsedFacetGroups()
        {
            foreach (var spec in Specifications)
            {
                if (spec.Applicability is not null)
                    yield return spec.Applicability;
                if (spec.Requirement is not null)
                    yield return spec.Requirement;
            }
        }

        /// <summary>
        /// returns facetgroups used in the SpecificationGroup
        /// </summary>
        /// <param name="use">What filters to use for inclusion criteria</param>
        /// <returns>a distinct enumerable</returns>
        public IEnumerable<FacetGroup> FacetGroups(FacetUse use)
        {
            var returned = new HashSet<FacetGroup>();
            foreach (var fg in UsedFacetGroups())
            {
                if (fg.IsUsed(this, use))
                {
                    if (!returned.Contains(fg))
                    {
                        returned.Add(fg);
                        yield return fg;
                    }
                }
            }
        }

        /// <inheritdoc />
        public string? GetProvider()
        {
            return Provider;
        }

        /// <inheritdoc />
        public IEnumerable<string>? GetConsumers()
        {
            if (Consumers != null)
                return Consumers;
            return null;
        }

        /// <inheritdoc />
        public IEnumerable<string>? GetStages()
        {
            return Stages;
        }

        internal void SetParent(Xids newParent)
        {
            parent = newParent;
            foreach (var spec in Specifications)
            {
                spec.SetParent(this);
            }
        }
    }
}