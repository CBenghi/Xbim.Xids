using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Xbim.InformationSpecifications
{
	public partial class Specification
    {
		private Xids ids;

		[Obsolete("Only for persistence, use the Xids.NewSpecification() method, instead.")]
		public Specification()
		{
		}

		public Specification(Xids ids)
		{
			this.ids = ids;
			Guid = System.Guid.NewGuid().ToString();
		}

		public List<string> Stages { get; set; }

		public Stakeholder Provider { get; set; }

		public List<Stakeholder> Consumer { get; set; }

		public string Name { get; set; }

		[JsonIgnore]
		public FacetGroup Applicability { get; set; }

		private string applicabilityId;

		[JsonPropertyName("Applicability")]
		public string ApplicabilityId
		{
			get => Applicability?.Guid.ToString();
			set => applicabilityId = value;
		}

		[JsonIgnore]
		public FacetGroup Requirement { get; set; }

		private string requirementId;

		[JsonPropertyName("Requirement")]
		public string RequirementId {
			get => Requirement?.Guid.ToString();
			set => requirementId = value;
		}

		public string Guid { get; set; }

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
