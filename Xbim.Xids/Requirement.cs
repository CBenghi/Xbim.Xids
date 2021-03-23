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
    public partial class Requirement
    {
		private Xids ids;

		[Obsolete("Only for persistence, use the Xids.NewRequirement() method, instead.")]
		public Requirement()
		{
		}

		public Requirement(Xids ids)
		{
			this.ids = ids;
			Guid = System.Guid.NewGuid().ToString();
		}

		public List<string> Stages { get; set; }

		public Stakeholder Provider { get; set; }

		public List<Stakeholder> Consumer { get; set; }

		public string Name { get; set; }

		[JsonIgnore]
		public FacetGroup ModelSubset { get; set; }

		private string modelId;

		[JsonProperty("ModelSubset")]
		public string ModelSubsetId
		{
			get => ModelSubset?.Guid.ToString();
			set => modelId = value;
		}

		[JsonIgnore]
		public FacetGroup Need { get; set; }

		private string needId;

		[JsonProperty("Need")]
		public string NeedId {
			get => Need?.Guid.ToString();
			set => needId = value;
		}

		public string Guid { get; set; }

		internal void SetExpectations(List<IFacet> fs)
		{
			var existing = ids.GetExpectation(fs);
			if (existing != null)
			{
				Need = existing;
				return;
			}
			if (Need == null)
				Need = new FacetGroup(ids.ExpectationsRepository);
			foreach (var item in fs)
			{
				Need.Facets.Add(item);
			}
		}

		internal void SetFilters(List<IFacet> fs)
		{
			var existing = ids.GetModel(fs);
			if (existing != null)
			{
				ModelSubset = existing;
				return;
			}
			if (ModelSubset == null)
				ModelSubset = new FacetGroup(ids.ModelSetRepository);
			foreach (var item in fs)
			{
				ModelSubset.Facets.Add(item);
			}
		}

		internal void SetIds(Xids unpersisted)
		{
			ids = unpersisted;
			var t = unpersisted.GetExpectation(needId);
			if (t != null)
				Need = t;

			// collections
			var m = unpersisted.GetModel(modelId);
			if (m != null)
				ModelSubset = m;
			var f = unpersisted.GetExpectation(needId);
			if (f != null)
				Need = f;

			if (Guid == null)
				Guid = System.Guid.NewGuid().ToString();
		}
	}
}
