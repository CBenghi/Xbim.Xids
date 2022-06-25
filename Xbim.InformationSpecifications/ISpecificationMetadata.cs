using System.Collections.Generic;

namespace Xbim.InformationSpecifications
{
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
        string GetProvider();

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
        IEnumerable<string> GetConsumers();

        /// <summary>
        /// Identifies the parties that will need the information (Optional)
        /// If set on any instance, it overrides any higher level setting.
        /// </summary>
        public IList<string>? Consumers { get; set; }

        /// <summary>
        /// Identifies the stages where the information will be needed (Optional)
        /// </summary>
        /// <returns>A list of strings identifying the consumers</returns>
        IEnumerable<string> GetStages();

        /// <summary>
        /// Identifies the stages where the information will be needed (Optional)
        /// If set on any instance, it overrides any higher level setting.
        /// </summary>
        public IList<string>? Stages { get; set; }

        /// <summary>
        /// Short description to allow the identification of the spec.
        /// </summary>
        /// <returns>A non empty string</returns>
        public string Short();

        /// <summary>
        /// Returns the hierarchical level of the specificaiton.
        /// </summary>
        SpecificationLevel Level { get; }

    }
}
