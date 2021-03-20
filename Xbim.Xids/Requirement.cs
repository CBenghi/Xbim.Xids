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
		}

		public List<string> Stages { get; set; }

		public Stakeholder Provider { get; set; }

		public List<Stakeholder> Consumer { get; set; }

		public string Name { get; set; }

		[JsonIgnore]
		public ModelPart ModelSubset { get; set; }

		private string modelId;

		[JsonProperty("ModelSubset")]
		public string ModelSubsetId
		{
			get => ModelSubset?.Guid.ToString();
			set => modelId = value;
		}

		[JsonIgnore]
		public Expectation Need { get; set; }

		private string needId;

		[JsonProperty("Need")]
		public string NeedId {
			get => Need?.Guid.ToString();
			set => needId = value;
		}

		public string Guid { get; set; }

		internal void SetExpectations(List<ExpectationFacet> fs)
		{
			var existing = ids.GetExpectation(fs);
			if (existing != null)
			{
				Need = existing;
				return;
			}
			if (Need == null)
				Need = new Expectation(ids);
			foreach (var item in fs)
			{
				Need.Facets.Add(item);
			}
		}

		internal void SetFilters(List<IFilter> fs)
		{
			var existing = ids.GetModel(fs);
			if (existing != null)
			{
				ModelSubset = existing;
				return;
			}
			if (ModelSubset == null)
				ModelSubset = new ModelPart(ids);
			foreach (var item in fs)
			{
				ModelSubset.Items.Add(item);
			}
		}

		internal void SetIds(Xids unpersisted)
		{
			ids = unpersisted;
			var t = unpersisted.GetExpectation(needId);
			if (t != null)
				Need = t;

			var m = unpersisted.GetModel(modelId);
			if (m != null)
				ModelSubset = m;
		}
	}
}
