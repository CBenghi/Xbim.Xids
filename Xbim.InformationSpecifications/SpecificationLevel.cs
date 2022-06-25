namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// the hierarchical level of classes implementing <see cref="ISpecificationMetadata"/>
    /// </summary>
    public enum SpecificationLevel
    {
        /// <summary>
        /// The level of a <see cref="Xids"/> (2nd order group)
        /// </summary>
        SpecificationRepository,
        /// <summary>
        /// The level of <see cref="SpecificationsGroup"/> (1st order group)
        /// </summary>
        SpecificationGroup,
        /// <summary>
        /// The level of a single <see cref="Specification"/> (operational level)
        /// </summary>
        SingleSpecification
    }
}
