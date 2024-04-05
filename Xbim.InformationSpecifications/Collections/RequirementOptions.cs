namespace Xbim.InformationSpecifications
{
    // TODO: This actually relates to a Facet not the Facet group. Should review/refactor
    /// <summary>
    /// A requirement can either be expected, prohibited or optional.
    /// </summary>
    public class RequirementCardinalityOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="facet"></param>
        /// <param name="defaultValue"></param>
        public RequirementCardinalityOptions(IFacet facet, Cardinality defaultValue)
        {
            RelatedFacet = facet;
            RelatedFacetCardinality = defaultValue;
        }

        /// <summary>
        /// The Facet the Cardinality belongs to
        /// </summary>
        public IFacet? RelatedFacet { get; set; }

        /// <summary>
        /// The Cardinality of the Facet
        /// </summary>
        public Cardinality? RelatedFacetCardinality { get; set; }

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