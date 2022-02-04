using System;
using System.Collections.Generic;
using static Xbim.InformationSpecifications.FacetGroup;

namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// A specification group is virtually capable of containing an entire bS IDS.
	/// Conceptually it applies to a singe model.
	/// It's then expanded to support extra LOIN features for XIDS.
	/// 
	/// Beyond rich metadata it contains a colleciton of <see cref="Specifications"/>, each having applicability and requirements.
	/// </summary>
	public partial class SpecificationsGroup : ISpecificationMetadata
	{
		// main properties
		public string Name { get; set; } // also in bS -> Title
		public string Copyright { get; set; } // bS
		public string Version { get; set; } // bS
		public string Description { get; set; } // bS
		public string Author { get; set; } // bS
		public DateTime? Date { get; set; } // bS
		public string Purpose { get; set; } // bS
		public string Milestone { get; set; } // bS

		// useful for LOIN
		public string Provider { get; set; }
		public List<string> Consumers { get; set; }
		public List<string> Stages { get; set; }

		// now the hierarchycal data
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