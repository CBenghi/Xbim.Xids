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
		/// Prepares a new specification,
		/// the function takes care of creating a destinationGroup if one suitable for schema is not found in the data.
		/// WARNING: this creates new facetgroups if applicability and requirement are not provided.
		/// </summary>
		/// <param name="ifcVersion">the desired parent collection</param>
		/// <param name="applicability"></param>
		/// <param name="requirement"></param>
		/// <returns>The initialised specification</returns>
		public Specification PrepareSpecification(
			string ifcVersion,
			FacetGroup applicability = null,
			FacetGroup requirement = null
			)
        {
			var destinationGroup = SpecificationsGroups.FirstOrDefault(x=>x.IfcVersion == ifcVersion);
			if (destinationGroup == null)
			{
				destinationGroup = new SpecificationsGroup();
				destinationGroup.IfcVersion = ifcVersion;
				SpecificationsGroups.Add(destinationGroup);
			}
			return PrepareSpecification(destinationGroup, applicability, requirement);
		}

		/// <summary>
		/// Prepares a new specification, inside the specified destinationGroup
		/// WARNING: this creates new facetgroups if applicability and requirement are not provided.
		/// </summary>
		/// <param name="destinationGroup">the desired owning collection</param>
		/// <param name="applicability"></param>
		/// <param name="requirement"></param>
		/// <returns>The initialised specification</returns>
		public Specification PrepareSpecification(
			SpecificationsGroup destinationGroup,
			FacetGroup applicability = null,
			FacetGroup requirement = null
			)
		{
			if (applicability == null)
				applicability = new FacetGroup(FacetRepository);
			if (requirement == null)
				requirement = new FacetGroup(FacetRepository);

			// checks and/or prepares destination
			if (destinationGroup == null) // if one exists get that
				destinationGroup = SpecificationsGroups.FirstOrDefault();
			if (destinationGroup == null)
			{
				destinationGroup = new SpecificationsGroup();
				SpecificationsGroups.Add(destinationGroup);
			}

			// creates new specification
			var t = new Specification(this, destinationGroup)
			{
				Applicability = applicability,
				Requirement = requirement
			};
			
			destinationGroup.Specifications.Add(t);
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
