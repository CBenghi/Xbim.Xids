using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xbim.Xids
{
	public partial class FacetGroup
	{
		public FacetGroup()
		{ }

		public FacetGroup(FacetGroupRepository repository)
		{
			repository.Add(this);
			Guid = System.Guid.NewGuid().ToString();
		}

		public ObservableCollection<IFacet> Facets { get; set; } = new ObservableCollection<IFacet>();

		public int UseCount(Xids t)
		{
			return t.AllSpecifications().Count(x => x.Applicability == this || x.Requirement == this);
		}

		public bool IsValid()
		{
			if (!Facets.Any())
				return false;
			foreach (var facet in Facets)
			{
				if (!facet.IsValid())
					return false;
			}
			return true;
		}

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