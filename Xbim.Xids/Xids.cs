using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xbim.Xids
{
	public partial class Xids
    {
		public Requirement NewRequirement(RequirementsGroup containingCollection = null)
		{
			var t = new Requirement(this)
			{
				ModelSubset = new FacetGroup(ModelSetRepository),
				Need = new FacetGroup(ExpectationsRepository)
			};
			if (containingCollection == null)
			{
				containingCollection = this.RequirementGroups.FirstOrDefault();
			}
			if (containingCollection == null)
			{
				containingCollection = new RequirementsGroup();
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
			ModelSetRepository = new FacetGroupRepository(this);
			ExpectationsRepository = new FacetGroupRepository(this);
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

		public FacetGroupRepository ModelSetRepository { get; set; }

		public FacetGroupRepository ExpectationsRepository { get; set; }

		public List<RequirementsGroup> RequirementGroups { get; set; } = new List<RequirementsGroup>();

		internal FacetGroup GetExpectation(string guid)
		{
			return ExpectationsRepository.FirstOrDefault(x => x.Guid.ToString() == guid);
		}

		internal FacetGroup GetExpectation(List<IFacet> fs)
		{
			return ExpectationsRepository.FirstOrDefault(x => x.Facets.FilterMatch(fs));
		}

		internal FacetGroup GetModel(string guid)
		{
			return ModelSetRepository.FirstOrDefault(x => x.Guid.ToString() == guid);
		}

		internal FacetGroup GetModel(List<IFacet> fs)
		{
			return ModelSetRepository.FirstOrDefault(x => x.Facets.FilterMatch(fs));
		}
	}
}
