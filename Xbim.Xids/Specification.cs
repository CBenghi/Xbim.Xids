using System;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections;
using System.Xml.Schema;
using System.ComponentModel;
using System.Xml;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Xbim.Xids
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

		[JsonProperty("Applicability")]
		public string ApplicabilityId
		{
			get => Applicability?.Guid.ToString();
			set => applicabilityId = value;
		}

		[JsonIgnore]
		public FacetGroup Requirement { get; set; }

		private string requirementId;

		[JsonProperty("Requirement")]
		public string RequirementId {
			get => Requirement?.Guid.ToString();
			set => requirementId = value;
		}

		public string Guid { get; set; }

		internal void SetExpectations(List<IFacet> fs)
		{
			var existing = ids.GetFacet(fs);
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
			var existing = ids.GetFacet(fs);
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
			var t = unpersisted.GetFacet(requirementId);
			if (t != null)
				Requirement = t;

			// collections
			var m = unpersisted.GetFacet(applicabilityId);
			if (m != null)
				Applicability = m;
			var f = unpersisted.GetFacet(requirementId);
			if (f != null)
				Requirement = f;

			if (Guid == null)
				Guid = System.Guid.NewGuid().ToString();
		}
	}
}
