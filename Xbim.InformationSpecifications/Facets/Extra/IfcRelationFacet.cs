using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// Constrain model parts on the ground of a related facet group.
	/// </summary>
	public class IfcRelationFacet : FacetBase, IFacet, IRepositoryRef, IEquatable<IfcRelationFacet>
	{

		// todo: the relationtype can be expanded to start directly from the same set (e.g., building up a constraint from summing other facetGroups).

		public enum RelationType
		{
			Undefined,
			ContainedElements,   // from: IfcSpatialElement -> ContainsElements (IfcRelContainedInSpatialStructure) -> RelatedElements
								 //       potentially recursive
								 // from: IfcElement        ->   HasOpenings (IfcRelVoidsElement) -> RelatedOpeningElement -> HasFillings (IfcRelFillsElement) -> RelatedBuildingElement
								 //       conceptually recursive

			Voids,               // from: IfcElement        ->   HasOpenings (IfcRelVoidsElement) -> RelatedOpeningElement
								 //       Recursive ignored

		}

		private string? sourceId;

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

        [JsonIgnore]
		public FacetGroup? Source { get; set; }

		private string relation = RelationType.Undefined.ToString();

		public RelationType GetRelation()
		{
			if (Enum.TryParse<RelationType>(relation, out var loc))
			{
				return loc;
			}
			return RelationType.Undefined;

		}
		public void SetRelation(RelationType loc)
		{
			relation = loc.ToString();
		}

		public string Relation
		{
			get { return relation; }
			set {
				if (Enum.TryParse<RelationType>(value, out _))
					relation = value;
			}
		}

		public override string ToString()
		{
			return $"{SourceId}-{Relation}-{base.ToString()}";
		}

		public override bool Equals(object? obj)
		{
			return this.Equals(obj as IfcRelationFacet);
		}

		public bool Equals(IfcRelationFacet? other)
		{
			if (other == null)
				return false;
			var thisEq = (sourceId, Relation)
				.Equals((other.SourceId, other.Relation));
			if (!thisEq)
				return false;
			return base.Equals(other);
		}

		public bool IsValid()
		{
			if (Source == null)
				return false;
			if (GetRelation() == RelationType.Undefined)
				return false;
			return true;
		}

		public override int GetHashCode() => 23 + 31 * (sourceId, relation).GetHashCode() + 53 * base.GetHashCode();


		public string Short()
		{
			return ToString();
		}

		// IRepositoryRef
		public void SetContextIds(Xids unpersisted)
		{
			Source = unpersisted.GetFacetGroup(sourceId);
		}

		// IRepositoryRef
		public IEnumerable<FacetGroup> UsedGroups()
		{
			if (Source is not null)
				yield return Source;
		}
	}
}
