using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xbim.Xids
{
	public partial class Expectation
	{
		public Expectation()
		{ }

		public Expectation(Xids ids)
		{
			ids.ExpectationsRepository.Add(this);
			Guid = System.Guid.NewGuid().ToString();
		}

		public ObservableCollection<ExpectationFacet> Facets { get; set; } = new ObservableCollection<ExpectationFacet>();

		public string Short()
		{
			if (!string.IsNullOrWhiteSpace(Name))
				return $"{Name} ({Facets.Count})";
			if (Facets.Any())
			{
				return string.Join(" and ", Facets.Select(x => x.Short()));
			}
			if (!string.IsNullOrWhiteSpace(Description))
				return Description;
			return "<undefined>";
		}

		public string Reference { get; set; }

		public string Guid { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}
}