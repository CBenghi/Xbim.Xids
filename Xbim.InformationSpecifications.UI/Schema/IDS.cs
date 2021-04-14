using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xbim.IDS.Schema.Collections;

namespace Xbim.IDS
{
    public partial class IDS
    {
		public Requirement NewRequirement()
		{
			return new Requirement(this)
			{
				ModelSubset = new ModelPart(this),
				Need = new Expectation(this)
			};
		}

		public IDS()
		{
			ModelSetRepository = new ModelPartCollection(this);
		}

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

		public ModelPartCollection ModelSetRepository { get; set; } 

		public List<Expectation> ExpectationsRepository { get; set; } = new List<Expectation>();

		public List<RequirementsCollection> RequirementGroups { get; set; } = new List<RequirementsCollection>();

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
