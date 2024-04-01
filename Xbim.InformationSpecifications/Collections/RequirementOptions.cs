namespace Xbim.InformationSpecifications
{
    // TODO: This actually relates to a Facet not the Facet group. Should review/refactor
    /// <summary>
    /// A requirement can either be expected, prohibited or optional.
    /// </summary>
    public class RequirementCardinalityOptions
    {
        public RequirementCardinalityOptions(IFacet facet, Cardinality defaultValue)
        {
            RelatedFacet = facet;
            RelatedFacetCardinality = defaultValue;
        }

        public IFacet? RelatedFacet { get; set; }
        public Cardinality? RelatedFacetCardinality { get; set; }

        public enum Cardinality
        {
            /// <summary>
            /// Should match all the facets in a <see cref="FacetGroup"/> 
            /// </summary>
            Expected,
            /// <summary>
            /// Cannot match all the facets in a <see cref="FacetGroup"/> 
            /// </summary>
            Prohibited,
            /// <summary>
            /// Should match if the facet is present regardless of value
            /// </summary>
            Optional
        }
    }
}