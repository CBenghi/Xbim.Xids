namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// A requirement can either be expected or prohibited.
    /// </summary>
    public enum RequirementOptions
    {
        /// <summary>
        /// Should match all the facets in a <see cref="FacetGroup"/> 
        /// </summary>
		Expected,
        /// <summary>
        /// Cannot match all the facets in a <see cref="FacetGroup"/> 
        /// </summary>
		Prohibited
    }
}