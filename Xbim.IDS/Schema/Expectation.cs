using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xbim.IDS
{
	public partial class Expectation
	{
		public Expectation()
		{

		}

		public Expectation(Ids ids)
		{
			ids.ExpectationsRepository.Add(this);
			Guid = System.Guid.NewGuid().ToString();
		}

		
		public ObservableCollection<ExpectationFacet> Facets { get; set; } = new ObservableCollection<ExpectationFacet>();

		private List<string> unresolvedIds;

		private IEnumerable<string> FacetIds
		{
			get
			{
				return Facets.Select(x => x.Guid.ToString());
			}
			set
			{
				unresolvedIds = value.ToList();
			}
		}


		public string Reference { get; set; }

		public string Guid { get; set; }

		public string Description { get; set; }
	}
}