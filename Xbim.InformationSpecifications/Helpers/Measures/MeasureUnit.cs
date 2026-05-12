using IdsLib.IfcSchema;
using Microsoft.Extensions.Logging;
using System;
using Xbim.InformationSpecifications.Generator.Measures;

namespace Xbim.InformationSpecifications.Helpers.Measures
{
	/// <summary>
	/// Main class to deal with unit conversion to and from constraint values.
	/// 
	/// Measurable units are stored in standard SI units in the schema, this class
	/// helps providing conversion methods against a variety of compatible units.
	/// </summary>
	public class MeasureUnit
	{
		/// <summary>
		/// Composition of the unit in the 7 fundamental units.
		/// </summary>
		public DimensionalExponents Exponent { get; }
		/// <summary>
		/// Conversion ratio between units
		/// </summary>
		public double Ratio { get; } = 1;
		/// <summary>
		/// Any scale offset (used for temperature conversions)
		/// </summary>
		public double Offset { get; } = 0;

		/// <summary>
		/// Evaluates if the Unit was correctly parsed during construction.
		/// </summary>
		public bool IsValid { get; private set; } = true;

		/// <summary>
		/// Gets the string representation of the unit associated with this converter.
		/// </summary>
		public string UnitRepresentation { get; }

		/// <summary>
		/// Attempts to retrieve a measure unit corresponding to the specified unit string and required measure information.
		/// </summary>
		/// <param name="unitString">The string representation of the unit to convert to.</param>
		/// <param name="requiredMeasure">The required measure information that defines the expected exponents for the unit.</param>
		/// <param name="logger">An optional logger used to record parsing or matching issues. May be null.</param>
		/// <param name="measureUnit">When this method returns, contains the matched measure unit if the operation succeeds; otherwise, null. This
		/// parameter is passed uninitialized.</param>
		/// <returns>true if a matching measure unit is found; otherwise, false.</returns>
		public static bool TryGetMeasureUnit(string unitString, IfcMeasureInformation requiredMeasure, ILogger? logger, out MeasureUnit? measureUnit)
		{
			return TryGetMeasureUnit(unitString, requiredMeasure.Exponents, logger, out measureUnit);
		}

		/// <summary>
		/// Attempts to create a measure unit from the specified unit string and verifies that its dimensional exponents match
		/// the required exponents.
		/// </summary>
		/// <remarks>If parsing fails or the exponents do not match, the method logs an error (if a logger is
		/// provided) and returns false. No exception is thrown for invalid input.</remarks>
		/// <param name="unitString">The string representation of the unit to convert to.</param>
		/// <param name="requiredExponents">The dimensional exponents that the resulting measure unit must match.</param>
		/// <param name="logger">An optional logger used to record errors encountered during parsing. May be null.</param>
		/// <param name="measureUnit">When this method returns, contains the resulting MeasureUnit if parsing and validation succeed; otherwise it would be null.</param>
		/// <returns>true if the unit string is successfully parsed into a valid MeasureUnit with matching exponents; otherwise, false.</returns>
		public static bool TryGetMeasureUnit(string unitString, DimensionalExponents requiredExponents, ILogger? logger, out MeasureUnit? measureUnit)
		{
			try
			{
				measureUnit = new MeasureUnit(unitString, logger);
				if (measureUnit.IsValid && (requiredExponents == measureUnit.Exponent))
					return true;
				logger?.LogError("Unit `{unitString}` has incompatible dimensions or invalid conversion. Expected {expected}, got {actual}", unitString, requiredExponents, measureUnit.Exponent);
				measureUnit = null;
				return false;
			}
			catch (Exception ex)
			{
				logger?.LogError(ex, "Error while creating MeasureUnit for `{unitString}`", unitString);
				measureUnit = null;
				return false;
			}
		}

		/// <summary>
		/// Constructor requiring a valid unit string.
		/// 
		/// If concerned with type compatibility, check that the <see cref="Exponent"/> property matches the required dimensional exponents 
		/// or use the static methods <see cref="TryGetMeasureUnit(string, DimensionalExponents, ILogger, out MeasureUnit?)"/> or 
		/// <see cref="TryGetMeasureUnit(string, IfcMeasureInformation, ILogger, out MeasureUnit?)"/> to ensure both validity and compatibility.
		/// </summary>
		/// <param name="unitString">The string representation of the unit to convert to, e.g. "lb/m2"; passing an unknown unit will mark the instance as invalid;
		/// an empty or null string will be treated as a dimensionless unit, but it will still be valid. 
		/// </param>
		/// <param name="logger">Optional logging provider</param>
		public MeasureUnit(string? unitString, ILogger? logger = null)
		{
			unitString = unitString?.Trim() ?? "";
			Exponent = new DimensionalExponents();
			if (unitString == "1")
				unitString = "";
			UnitRepresentation = unitString;
			bool hasComponents = false;
			foreach (var partialSymbol in UnitFactor.SymbolBreakDown(unitString))
			{
				hasComponents = true;
				if (partialSymbol.UnitSymbol == "1") // deals with cases such as 1/sec
					continue; // nothing to do
				if (partialSymbol.TryGetDimensionalExponents(out var exp, out var ratio, out var off))
				{
					Offset = off;
					Exponent = Exponent.Multiply(exp);
					Ratio *= ratio;
				}
				else
				{
					logger?.LogWarning("Unit {symbol} not found in conversion table", partialSymbol.UnitSymbol);
					IsValid = false;
				}
			}
			if (!hasComponents && !string.IsNullOrEmpty(unitString))
			{
				IsValid = false;
				logger?.LogWarning("Unit `{unitString}` has no valid components in the string.", unitString);
			}
			if (!Exponent.Equals(new DimensionalExponents(0, 0, 0, 0, 1, 0, 0)))
				Offset = 0;
		}

		/// <summary>
		/// Attempts conversion of the value to SI starting from the source unit
		/// </summary>
		/// <param name="valueSourceUnit">double value to convert</param>
		/// <param name="valueSI">out value in SI units</param>
		/// <returns>true if conversion is valid, false otherwise</returns>
		public bool TryConvertToSI(double valueSourceUnit, out double valueSI)
		{
			if (!IsValid)
			{
				valueSI = valueSourceUnit;
				return false;
			}
			valueSI = (valueSourceUnit + Offset) * Ratio;
			return true;
		}

		/// <summary>
		/// Attempts conversion of the value to source unit starting from the SI 
		/// </summary>
		/// <param name="valueSI">double value in SI units</param>
		/// <param name="valueSourceUnit">out double value converted</param>
		/// <returns>true if conversion is valid, false otherwise</returns>
		public bool TryConvertFromSI(double valueSI, out double valueSourceUnit)
		{
			if (!IsValid)
			{
				valueSourceUnit = valueSI;
				return false;
			}
			valueSourceUnit = valueSI / Ratio - Offset;
			return true;
		}
	}
}
