using Microsoft.Extensions.Logging;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Core Interface for the definition of parts of a <see cref="ValueConstraint"/>
    /// </summary>
    public interface IValueConstraintComponent
    {
        /// <summary>
        /// Evaluates the constraint of the given <paramref name="candiatateValue"/>
        /// </summary>
        /// <param name="candiatateValue">the value to check</param>
        /// <param name="context">The parent constraint, not itself implementing <see cref="IValueConstraintComponent"/></param>
        /// <param name="ignoreCase">In case of evaluation of strings, defines if the letter case should be considered or ignored, true to ignore.</param>
        /// <param name="logger">logging context</param>
        /// <returns>true if satisfied by the value</returns>
        bool IsSatisfiedBy(object candiatateValue, ValueConstraint context, bool ignoreCase, ILogger? logger = null);
        
        /// <summary>
        /// a brief description of the component
        /// </summary>
        /// <returns>a string</returns>
        string Short();

        // todo: 2022 06 21 - add an IsValid method to the interface, e.g. patters and ranges can be invalid 
    }
}