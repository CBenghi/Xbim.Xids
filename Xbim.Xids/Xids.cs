using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xbim.Xids
{
	public partial class Xids
    {
		public Requirement NewRequirement(RequirementsCollection containingCollection = null)
		{
			var t = new Requirement(this)
			{
				ModelSubset = new ModelPart(this),
				Need = new Expectation(this)
			};
			if (containingCollection == null)
			{
				containingCollection = this.RequirementGroups.FirstOrDefault();
			}
			if (containingCollection == null)
			{
				containingCollection = new RequirementsCollection();
				RequirementGroups.Add(containingCollection);
			}
			containingCollection.Requirements.Add(t);
			return t;
		}

		public static Xids FromStream(Stream s)
		{
			return Xids.ImportBuildingSmartIDS(s);
		}

		public Xids()
		{
			ModelSetRepository = new ModelPartCollection(this);
			ExpectationsRepository = new ExpectationCollection(this);
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

		public ExpectationCollection ExpectationsRepository { get; set; }

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
