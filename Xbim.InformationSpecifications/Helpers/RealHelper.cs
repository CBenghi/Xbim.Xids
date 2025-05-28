using System;

namespace Xbim.InformationSpecifications.Helpers
{
	/// <summary>
	/// Helper for Real number calculations
	/// </summary>
	public static class RealHelper
	{
		/// <summary>
		/// The Default tolerance to use for Real equality testing
		/// </summary>
		/// <remarks>This is not an absolute amount of tolerance but applies some adjustments based on magnitude of the values</remarks>
		internal const double DefaultRealPrecision = 1e-6;

		/// <summary>
		/// Calculates upper and lower precision of Reals to a precision accounting for the magnitude of the numbers
		/// </summary>
		/// <remarks>Based on https://github.com/buildingSMART/IDS/issues/78#issuecomment-1594479489</remarks>
		/// <param name="value">The real value to adjust</param>
		/// <param name="tolerance">the precision. Defaults to 1e-06</param>
		/// <returns>a tuple containing the lowerbound and the highbound values</returns>
		public static (double, double) GetPrecisionBounds(double value, double tolerance = DefaultRealPrecision)
		{
			var lowerBound = Math.Round(value - (Math.Abs(value) * tolerance) - tolerance, 15);
			var upperBound = Math.Round(value + (Math.Abs(value) * tolerance) + tolerance, 15);
			return new(lowerBound, upperBound);
		}
	}
}
