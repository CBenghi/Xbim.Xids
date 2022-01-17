using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Xbim.InformationSpecifications.FacetGroup;

namespace Xbim.InformationSpecifications
{
	

	public partial class Xids // basic definition file
    {
        private ILogger<Xids> _logger;

        public string IfcVersion { get; set; } = "";

		public static bool HasData(Xids xidsToTest)
		{
			if (xidsToTest == null)
				return false;
			if (xidsToTest.AllSpecifications().Any())
				return true;
			if (xidsToTest.FacetRepository.Collection.Any())
				return true;
			if (xidsToTest.SpecificationsGroups.Any())
				return true;
			return false;
		}

		[Obsolete("Use PrepareSpecification() instead")]
		public Specification NewSpecification(SpecificationsGroup containingCollection = null)
		{
			return PrepareSpecification(containingCollection);
		}

		/// <summary>
		/// Prepares a new specification taking care of target specification group if not provided.
		/// WARNING: this creates two new facetgroups if not provided.
		/// </summary>
		/// <param name="containingCollection">the desired parent collection</param>
		/// <returns>The initialised specification</returns>
		public Specification PrepareSpecification(
			SpecificationsGroup containingCollection = null,
			FacetGroup applicability = null,
			FacetGroup requirement = null
			)
		{
			if (applicability == null)
				applicability = new FacetGroup(FacetRepository);
			if (requirement == null)
				requirement = new FacetGroup(FacetRepository);

			var t = new Specification(this, containingCollection)
			{
				Applicability = applicability,
				Requirement = requirement
			};
			if (containingCollection == null)
			{
				containingCollection = SpecificationsGroups.FirstOrDefault();
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
		public Xids(ILogger<Xids> logger) : this()
		{
			_logger = logger;
		}

		public void Initialize(string ifcVersion)
		{
			IfcVersion = ifcVersion; 
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

		public IEnumerable<FacetGroup> FacetGroups(FacetUse use)
		{
			foreach (var fg in FacetRepository.Collection)
			{
				if (fg.IsUsed(this, use))
					yield return fg;
			}
		}

		public void Purge()
		{
			var unusedFG = FacetRepository.Collection.Except(FacetGroups(FacetUse.All)).ToList();
			foreach (var unused in unusedFG)
			{
				FacetRepository.Collection.Remove(unused);
			}
		}

		public Project Project { get; set; } = new Project();

		public FacetGroupRepository FacetRepository { get; set; }

		public List<SpecificationsGroup> SpecificationsGroups { get; set; } = new List<SpecificationsGroup>();

		internal FacetGroup GetFacetGroup(string guid)
		{
			return FacetRepository.FirstOrDefault(x => x.Guid.ToString() == guid);
		}

		internal FacetGroup GetFacetGroup(List<IFacet> fs)
		{
			return FacetRepository.FirstOrDefault(x => x.Facets.FilterMatch(fs));
		}



	}
}
