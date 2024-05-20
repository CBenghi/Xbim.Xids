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

        private static IList<Cardinality> AllOptions = new List<Cardinality>() { Cardinality.Expected, Cardinality.Prohibited, Cardinality.Optional };
        private static IList<Cardinality> NoOptional = new List<Cardinality>() { Cardinality.Expected, Cardinality.Prohibited };
        private static IList<Cardinality> ExpectedOnly = new List<Cardinality>() { Cardinality.Expected };

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
        [JsonIgnore]
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