using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xbim.Xids
{
	public partial class Xids
    {
		public Specification NewSpecification(SpecificationsGroup containingCollection = null)
		{
			var t = new Specification(this)
			{
				Applicability = new FacetGroup(FacetRepository),
				Requirement = new FacetGroup(FacetRepository)
			};
			if (containingCollection == null)
			{
				containingCollection = this.SpecificationsGroups.FirstOrDefault();
			}
			if (containingCollection == null)
			{
				containingCollection = new SpecificationsGroup();
				SpecificationsGroups.Add(containingCollection);
			}
			containingCollection.Specifications.Add(t);
			return t;
		}

		public static Xids FromStream(Stream s)
		{
			return Xids.ImportBuildingSmartIDS(s);
		}

		public Xids()
		{
			FacetRepository = new FacetGroupRepository(this);
		}

		[Obsolete("Use AllSpecifications(), instead.")]
		public IEnumerable<Specification> AllRequirements()
		{
			return AllSpecifications();
		}

		public IEnumerable<Specification> AllSpecifications()
		{
			foreach (var rg in SpecificationsGroups)
			{
				foreach (var req in rg.Specifications)
				{
					yield return req;
				}
			}
		}

		public Project Project { get; set; } = new Project();

		public FacetGroupRepository FacetRepository { get; set; }


		public List<SpecificationsGroup> SpecificationsGroups { get; set; } = new List<SpecificationsGroup>();

		internal FacetGroup GetFacet(string guid)
		{
			return FacetRepository.FirstOrDefault(x => x.Guid.ToString() == guid);
		}

		internal FacetGroup GetFacet(List<IFacet> fs)
		{
			return FacetRepository.FirstOrDefault(x => x.Facets.FilterMatch(fs));
		}	
	}
}
