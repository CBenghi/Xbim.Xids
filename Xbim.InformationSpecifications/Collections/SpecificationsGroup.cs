using System;
using System.Collections.Generic;
using static Xbim.InformationSpecifications.FacetGroup;

namespace Xbim.InformationSpecifications
{
	public partial class SpecificationsGroup : ISpecificationMetadata
	{
		public string Name { get; set; }

		public string Provider { get; set; }

		public List<string> Consumers { get; set; }

		public List<string> Stages { get; set; }

		public List<Specification> Specifications { get; set; } = new List<Specification>();

		internal IEnumerable<FacetGroup> UsedFacetGroups()
		{
			foreach (var spec in Specifications)
			{
				yield return spec.Applicability;
				yield return spec.Requirement;
			}
		}

		/// <summary>
		/// returns facetgroups used in the SpecificationGroup
		/// </summary>
		/// <param name="use">What filters to use for inclusion criteria</param>
		/// <returns>a distinct enumerable</returns>
		public IEnumerable<FacetGroup> FacetGroups(FacetUse use)
		{

			// todo: 2021: improve documentation to clarify the use paramter (only starting from applic and requirement).

			HashSet<FacetGroup> returned = new HashSet<FacetGroup>();
			foreach (var fg in UsedFacetGroups())
			{
				if (fg.IsUsed(this, use))
				{
					if (!returned.Contains(fg))
					{
						returned.Add(fg);
						yield return fg;
					}
				}
			}
		}
	}
}