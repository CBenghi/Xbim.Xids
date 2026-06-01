using System.Collections.Generic;
using System.Text.Json.Serialization;
using Xbim.InformationSpecifications.Cardinality;

namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// A requirement facet can either be expected, prohibited or optional.
	/// </summary>
	public class RequirementCardinalityOptions
	{
		/// <summary>
		/// The default valud of the cardinality if not speciried.
		/// </summary>
		public static Cardinality DefaultCardinality => Cardinality.Expected;

		/// <summary>
		/// Serialization constructor
		/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public RequirementCardinalityOptions()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			RelatedFacetCardinality = DefaultCardinality;
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="facet">The related facet</param>
		/// <param name="requiredValue">the relevant cardinality</param>
		public RequirementCardinalityOptions(IFacet facet, Cardinality requiredValue)
		{
			RelatedFacet = facet;
			RelatedFacetCardinality = requiredValue;
		}

		private static readonly IList<Cardinality> AllOptions = new List<Cardinality>() { Cardinality.Expected, Cardinality.Prohibited, Cardinality.Optional };
		private static readonly IList<Cardinality> NoOptional = new List<Cardinality>() { Cardinality.Expected, Cardinality.Prohibited };
		private static readonly IList<Cardinality> ExpectedOnly = new List<Cardinality>() { Cardinality.Expected };


		/// <summary>
		/// Determines the valid options for cardinaly of a given facet
		/// </summary>
		/// <returns>The list of valid options</returns>
		public static IList<Cardinality> GetAllowedCardinality(IFacet relevantFacet)
		{
			return relevantFacet switch
			{
				IfcTypeFacet _ => ExpectedOnly,
				IfcRelationFacet _ => ExpectedOnly, // custom xbim
				PartOfFacet _ => NoOptional,
				DocumentFacet _ => NoOptional, // custom xbim
				_ => AllOptions
			};
		}

		/// <summary>
		/// Depending on the Type of <see cref="RelatedFacet"/>, the valid options for cardinaly might be affected
		/// </summary>
		/// <returns>The list of valid options</returns>
		public IList<Cardinality> GetAllowedCardinality()
		{
			return GetAllowedCardinality(RelatedFacet);
		}

		/// <summary>
		/// The Facet the Cardinality belongs to
		/// </summary>
		[JsonIgnore]
		public IFacet RelatedFacet { get; set; }

		/// <summary>
		/// The Cardinality of the Facet. The possible value of the enumeration depends on the type of the related facet; 
		/// for example, if the facet is an IfcTypeFacet, the only valid value is Expected, 
		/// while if the facet is a PartOfFacet, the valid values are Expected and Prohibited.
		/// Programmatic access cen be done using the one of the <see cref="GetAllowedCardinality()"/> methods, which returns the valid 
		/// options for a facet.
		/// </summary>
		public Cardinality RelatedFacetCardinality { get; set; }

		/// <summary>
		/// The cardinality of a Facet can be 
		/// set via the <see cref="FacetGroup.SetRequirementCardinalityOption(IFacet, Cardinality)"/> method helper and gotten
		/// via the <see cref="FacetGroup.GetRequirementCardinalityOption(IFacet, out Cardinality?)"/>.
		/// </summary>
		public enum Cardinality
		{
			/// <summary>
			/// The <see cref="IFacet"/> requirement must be met
			/// </summary>
			Expected,
			/// <summary>
			/// The <see cref="IFacet"/> requirement is prohibited
			/// </summary>
			Prohibited,
			/// <summary>
			/// If the element is present the <see cref="IFacet"/> requirement must be met
			/// </summary>
			Optional
		}
	}
}
