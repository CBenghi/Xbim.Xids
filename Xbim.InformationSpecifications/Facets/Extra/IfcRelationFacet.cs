using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Constrain model parts on the ground of a related facet group.
    /// </summary>
    public class IfcRelationFacet : FacetBase, IFacet, IRepositoryRef, IEquatable<IfcRelationFacet>
    {
        // todo: the relationtype can be expanded to start directly from the same set (e.g., building up a constraint adding other facetGroups).

        /// <summary>
        /// The kind of relation that expands from the original <see cref="Source"/> facet group
        /// </summary>
        public enum RelationType
        {
            /// <summary>
            /// Not suitable for testing
            /// </summary>
            Undefined,
            /// <summary>
            /// Contained elements are found by:
            /// - from: IfcSpatialElement -> ContainsElements (IfcRelContainedInSpatialStructure) -> RelatedElements (potentially recursive)
            /// - from: IfcElement        ->   HasOpenings (IfcRelVoidsElement) -> RelatedOpeningElement -> HasFillings (IfcRelFillsElement) -> RelatedBuildingElement (conceptually recursive)
            /// </summary>
            ContainedElements,

            /// <summary>
            /// Contained elements are found by:
            /// from: IfcElement        ->   HasOpenings (IfcRelVoidsElement) -> RelatedOpeningElement (Recursive ignored)
            /// </summary>
            Voids,

        }

        private string? sourceId;

        /// <summary>
        /// Id of the starting element set 
        /// </summary>
        [JsonPropertyName("Source")]
        public string? SourceId
        {
            get
            {
                if (Source == null)
                    return sourceId;
                return Source.Guid?.ToString();
            }
            set => sourceId = value;
        }

        /// <summary>
        /// The resolved facet Group of the starting set;
        /// </summary>
        [JsonIgnore]
        public FacetGroup? Source { get; set; }

        private string relation = RelationType.Undefined.ToString();

        /// <summary>
        /// Enum resolved from the <see cref="Relation"/> string
        /// </summary>
        public RelationType GetRelation()
        {
            if (Enum.TryParse<RelationType>(relation, out var rel))
            {
                return rel;
            }
            return RelationType.Undefined;
        }

        /// <summary>
        /// Helper setting the string from the enum
        /// </summary>
        public void SetRelation(RelationType relationType)
        {
            relation = relationType.ToString();
        }

        /// <summary>
        /// Getter and setter of the relation string, the setter ignores strings that do represent a valid <see cref="RelationType"/>.
        /// </summary>
        public string Relation
        {
            get { return relation; }
            set
            {
                if (Enum.TryParse<RelationType>(value, out _))
                    relation = value;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{SourceId}-{Relation}-{base.ToString()}";
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return this.Equals(obj as IfcRelationFacet);
        }

        /// <inheritdoc />
        public bool Equals(IfcRelationFacet? other)
        {
            if (other == null)
                return false;
            var thisEq = (SourceId, Relation)
                .Equals((other.SourceId, other.Relation));
            if (!thisEq)
                return false;
            return base.Equals(other);
        }

        /// <summary>
        /// Valid (see <see cref="IFacet.IsValid"/>) if 
        /// <see cref="Source"/> and
        /// <see cref="Relation"/>        
        /// are meaningful.
        /// </summary>
        /// <returns>true if valid</returns>        
        [MemberNotNullWhen(true, nameof(Source))]
        [MemberNotNullWhen(true, nameof(Relation))]
        public bool IsValid()
        {
            if (Source == null)
                return false;
            if (GetRelation() == RelationType.Undefined)
                return false;
#pragma warning disable CS8775 // Member must have a non-null value when exiting in some condition, ensured by GetRelation()
            return true;
#pragma warning restore CS8775 // Member must have a non-null value when exiting in some condition.
        }

        /// <inheritdoc />
        public override int GetHashCode() => 23 + 31 * (sourceId, relation).GetHashCode() + 53 * base.GetHashCode();

        /// <inheritdoc />
        public string Short()
        {
            return ToString();
        }

        /// <inheritdoc />
        public void SetContextIds(Xids unpersisted)
        {
            Source = unpersisted.GetFacetGroup(sourceId);
        }

        /// <inheritdoc />
		public IEnumerable<FacetGroup> UsedGroups()
        {
            if (Source is not null)
                yield return Source;
        }
    }
}
