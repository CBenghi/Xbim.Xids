using System.Collections.Generic;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// the hierarchical level of classes implementing <see cref="ISpecificationMetadata"/>
    /// </summary>
    public enum SpecificationContextType
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

    /// <summary>
    /// LOIN related Specification metadata
    /// </summary>
    public interface ISpecificationMetadata
    {
        /// <summary>
        /// Unique identification, must always return a value 
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Object name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Retuns the party responsibile for providing the information (Optional)
        /// Including value from the grouping inheritance.
        /// </summary>
        /// <returns>A string identifying the provider, can be null.</returns>
        string? GetProvider();

        /// <summary>
        /// Identifies the party responsibile for providing the information (Optional)
        /// If set on any instance, it overrides any higher level setting.
        /// </summary>
        string? Provider { get; set; }

        /// <summary>
        /// Returns the parties that will need the information (Optional)
        /// Including values from the grouping inheritance.
        /// </summary>
        /// <returns>A list of strings identifying the consumers</returns>
        IEnumerable<string>? GetConsumers();

        /// <summary>
        /// Identifies the parties that will need the information (Optional)
        /// If set on any instance, it overrides any higher level setting.
        /// </summary>
        public IList<string>? Consumers { get; set; }

        /// <summary>
        /// Identifies the stages where the information will be needed (Optional)
        /// </summary>
        /// <returns>A list of strings identifying the consumers</returns>
        IEnumerable<string>? GetStages();

        /// <summary>
        /// Identifies the stages where the information will be needed (Optional)
        /// If set on any instance, it overrides any higher level setting.
        /// </summary>
        public IList<string>? Stages { get; set; }

        // todo: add testing of the GetXXX functions.
    }
}
