using Microsoft.Extensions.Logging;
using System.Xml;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Useful in the determination of required cardinal constraints for aspects of the model.
    /// E.g. Any parts are expected or invalid (possibly how many are due).
    /// </summary>
    public interface ICardinality
    {
        /// <summary>
        /// Exports a representation of the instance to a valid bS IDS format
        /// </summary>
        /// <param name="xmlWriter">the writer used as target of the export</param>
        /// <param name="logger">optional logging environemnt</param>
        void ExportBuildingSmartIDS(XmlWriter xmlWriter, ILogger? logger);

        /// <summary>
        /// Determines if a requirement is expected for the specication, given the values in the ICardinality instance.
        /// </summary>
        bool ExpectsRequirements { get; }

        /// <summary>
        /// A string describing the nature of the cardinality.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// True if the cardinality requires the entire model for evaluation.
        /// </summary>
        bool IsModelConstraint { get; }

        /// <summary>
        /// True if the cardinality expects that there are no entities matching.
        /// </summary>
        bool NoMatchingEntities { get; }

        /// <summary>
        /// Evaluates whether the cardinality properties are meaningful and valid
        /// </summary>
        /// <returns></returns>
        bool IsValid();

        /// <summary>
        /// Evaluates if the cardinality is satisfied by the provided count
        /// </summary>
        /// <param name="count">the number to evaluate</param>
        /// <returns>true if satisfied, false otherwise</returns>
        bool IsSatisfiedBy(int count);
        
    }
}
