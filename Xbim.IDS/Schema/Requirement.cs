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

namespace Xbim.IDS
{
    public partial class Requirement
    {
		private Ids ids;

		public Requirement()
		{
		}

		public Requirement(Ids ids)
		{
			this.ids = ids;
		}

		public ObservableCollection<string> Stages { get; set; }

		public Stakeholder Provider { get; set; }

		public ObservableCollection<Stakeholder> Consumer { get; set; }

		public string Name { get; set; }

		public ModelPart ModelSubset { get; set; }

		[JsonIgnore]
		public Expectation Need { get; set; }

		private string needId;

		public string NeedId {
			get => Need == null ? null : Need.Guid.ToString();
			set => needId = value;
		}

		public string Guid { get; set; }

		internal void AddExpectations(List<ExpectationFacet> fs)
		{
			if (Need == null)
				Need = new Expectation(ids);
			foreach (var item in fs)
			{
				Need.Facets.Add(item);
			}
		}

		internal void SetIds(Ids unpersisted)
		{
			ids = unpersisted;
			var t = unpersisted.GetExpectation(needId);
			if (t != null)
				Need = t;
			
		}
	}
}
