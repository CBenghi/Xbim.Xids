using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Xbim.IDS
{
    public partial class Ids
    {
		public IEnumerable<Requirement> AllRequirements()
		{
			foreach (var rg in RequirementGroups)
			{
				foreach (var req in rg.Requirements)
				{
					yield return req;
				}
			}
		}

		public Project Project { get; set; } = new Project();

		public ObservableCollection<ModelPart> ModelSetRepository { get; set; } = new ObservableCollection<ModelPart>();

		public ObservableCollection<Expectation> ExpectationsRepository { get; set; } = new ObservableCollection<Expectation>();

		public ObservableCollection<RequirementsCollection> RequirementGroups { get; set; } = new ObservableCollection<RequirementsCollection>();

		internal Expectation GetExpectation(string guid)
		{
			return ExpectationsRepository.FirstOrDefault(x => x.Guid.ToString() == guid);
		}

		internal Expectation GetExpectation(List<ExpectationFacet> fs)
		{
			return ExpectationsRepository.FirstOrDefault(x => x.Facets.FilterMatch(fs));
		}

		internal ModelPart GetModel(string guid)
		{
			return ModelSetRepository.FirstOrDefault(x => x.Guid.ToString() == guid);
		}

		internal ModelPart GetModel(List<IFilter> fs)
		{
			return ModelSetRepository.FirstOrDefault(x => x.Items.FilterMatch(fs));
		}
	}
}
