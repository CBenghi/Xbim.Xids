using System.Collections.Generic;
using Xbim.InformationSpecifications.Cardinality;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// A requirement facet can either be expected, prohibited or optional.
    /// </summary>
    public class RequirementCardinalityOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="facet"></param>
        /// <param name="defaultValue"></param>
#pragma warning disable IDE0290 // Use primary constructor
        public RequirementCardinalityOptions(IFacet facet, Cardinality defaultValue)
#pragma warning restore IDE0290 // Use primary constructor
        {
            RelatedFacet = facet;
            RelatedFacetCardinality = defaultValue;
        }

        private static IList<Cardinality> AllOptions = new List<Cardinality>() { Cardinality.Expected, Cardinality.Prohibited, Cardinality.Optional };
        private static IList<Cardinality> NoOptional = new List<Cardinality>() { Cardinality.Expected, Cardinality.Prohibited };
        private static IList<Cardinality> ExpectedOnly = new List<Cardinality>() { Cardinality.Expected, Cardinality.Prohibited };

        /// <summary>
        /// Depending on the Type of <see cref="RelatedFacet"/>, the valid options for cardinaly might be affected
        /// </summary>
        /// <returns>The list of valid options</returns>
        public IList<Cardinality> GetAllowedCardinality()
        {
            return RelatedFacet switch
            {
                IfcTypeFacet _ => ExpectedOnly,
                PartOfFacet _ => NoOptional,
                _ => AllOptions
            };
        }

        /// <summary>
        /// The Facet the Cardinality belongs to
        /// </summary>
        public IFacet RelatedFacet { get; set; }

        /// <summary>
        /// The Cardinality of the Facet
        /// </summary>
        public Cardinality RelatedFacetCardinality { get; set; }

        /// <summary>
        /// The cardinality of a Facet
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