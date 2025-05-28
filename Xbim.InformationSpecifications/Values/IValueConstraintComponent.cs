using Microsoft.Extensions.Logging;

namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// Core Interface for the definition of parts of a <see cref="ValueConstraint"/>
	/// </summary>
	public interface IValueConstraintComponent
	{
		/// <summary>
		/// Evaluates the constraint of the given <paramref name="candidateValue"/>
		/// </summary>
		/// <param name="candidateValue">the value to check</param>
		/// <param name="context">The parent constraint, not itself implementing <see cref="IValueConstraintComponent"/></param>
		/// <param name="ignoreCase">When <c>true</c> any strings will be compared case insensitively ignoring accents; otherwise if <c>false</c> an exact match is required.</param>
		/// <param name="logger">logging context</param>
		/// <returns>true if satisfied by the value</returns>
		bool IsSatisfiedBy(object candidateValue, ValueConstraint context, bool ignoreCase, ILogger? logger = null);

		/// <summary>
		/// a brief description of the component
		/// </summary>
		/// <returns>a string</returns>
		string Short();

		/// <summary>
		/// Come constraints could be internally invalid. E.g. patters and ranges.
		/// </summary>
		/// <param name="context">The value constraint in which the validity is checked</param>
		/// <returns>True if valid, false if invalid.</returns>
		bool IsValid(ValueConstraint context);


	}
}