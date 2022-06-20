using System.Collections.Generic;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// LOIN related Specification metadata
    /// </summary>
    public interface ISpecificationMetadata
    {
        /// <summary>
        /// Identifies the party responsibile for providing the information (Optional)
        /// </summary>
        /// <returns>A string identifying the provider</returns>
        string? GetProvider();

        /// <summary>
        /// Identifies the parties that will need the information (Optional)
        /// </summary>
        /// <returns>A list of strings identifying the consumers</returns>
        IEnumerable<string>? GetConsumers();

        /// <summary>
        /// Identifies the stages where the information will be needed (Optional)
        /// </summary>
        /// <returns>A list of strings identifying the consumers</returns>
        IEnumerable<string>? GetStages();
    }
}
