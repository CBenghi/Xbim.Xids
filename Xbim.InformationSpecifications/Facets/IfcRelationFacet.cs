using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Xbim.InformationSpecifications
{
	public class IfcRelationFacet : IFacet, IRepositoryRef, IEquatable<IfcRelationFacet>
	{
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

		[JsonIgnore]
		public FacetGroup Source { get; set; }


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
			return $"{sourceId}-{Relation}-{base.ToString()}";
		}


		private string sourceId;

		[JsonPropertyName("Source")]
		public string RelationFacetId
		{
			get => Source?.Guid.ToString();
			set => sourceId = value;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as IfcRelationFacet);
		}

		public bool Equals(IfcRelationFacet other)
		{
			if (other == null)
				return false;
			return (Source, Relation )
				.Equals((other.Source, other.Relation));
		}

		public bool IsValid()
		{
			if (Source == null)
				return false;
			if (GetRelation() == RelationType.Undefined)
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public string Short()
		{
			return ToString();
		}

		public void SetIds(Xids unpersisted)
		{
			var t = unpersisted.GetFacetGroup(sourceId);
			if (t != null)
				Source = t;
		}
	}
}
